resource "aws_s3_bucket" "dpd_document_bucket" {
  bucket = "${var.project}-${var.environment}-ui"
  acl    = "private"
}

locals {
  s3_origin_id = "${var.project}-${var.environment}-ui"
}

resource "aws_cloudfront_origin_access_identity" "origin_access_identity" {
  comment = "For DPD-Document-API"
}

resource "aws_cloudfront_distribution" "dpd_document_cf_dist" {
  origin {
    domain_name = "${aws_s3_bucket.dpd_document_bucket.bucket_regional_domain_name}"
    origin_id   = "${local.s3_origin_id}"

    s3_origin_config {
      origin_access_identity = "${aws_cloudfront_origin_access_identity.origin_access_identity.cloudfront_access_identity_path}"
    }
  }

  enabled             = true
  is_ipv6_enabled     = true
  comment             = ""
  default_root_object = "index.html"

  custom_error_response {
      error_code = 404
      error_caching_min_ttl = 300
      response_code = 200
      response_page_path = "/"
  }

  aliases = ["${var.cf_aliases}"]

  default_cache_behavior {
    allowed_methods  = ["HEAD", "GET"]
    cached_methods   = ["HEAD", "GET"]
    target_origin_id = "${local.s3_origin_id}"

    forwarded_values {
      query_string = false

      cookies {
        forward = "none"
      }
    }

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 0
    max_ttl                = 0
  }

  price_class = "PriceClass_100"

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    acm_certificate_arn = "${var.certificate_arn}"
    ssl_support_method = "sni-only"
    minimum_protocol_version = "TLSv1"
  }
}

# bucket policy
data "aws_iam_policy_document" "s3_policy" {
  statement {
    actions   = ["s3:GetObject"]
    resources = ["${aws_s3_bucket.dpd_document_bucket.arn}/*"]

    principals {
      type        = "AWS"
      identifiers = ["${aws_cloudfront_origin_access_identity.origin_access_identity.iam_arn}"]
    }
  }

  statement {
    actions   = ["s3:ListBucket"]
    resources = ["${aws_s3_bucket.dpd_document_bucket.arn}"]

    principals {
      type        = "AWS"
      identifiers = ["${aws_cloudfront_origin_access_identity.origin_access_identity.iam_arn}"]
    }
  }
}

resource "aws_s3_bucket_policy" "dpd_document_bucket_pol" {
  bucket = "${aws_s3_bucket.dpd_document_bucket.id}"
  policy = "${data.aws_iam_policy_document.s3_policy.json}"
}