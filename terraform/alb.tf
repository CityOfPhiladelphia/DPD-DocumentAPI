# dpd-document-api
module "dpd_document_api_tg" {
  source                    = "app.terraform.io/philadelphia/alb/aws"
  version                   = "0.2.1"
  vpc                       = "${var.vpc_id}"
  dept                      = "${var.department}"
  env                       = "${var.environment}"
  proj                      = "${var.project}"
  infra                     = "api"
  response                  = "200,301"
  path                      = "/api/health-check"
  listen                    = "${aws_alb_listener.dpd_document_public2.arn}"
  priority                  = 200
  pattern                   = ["/*"]
  target_port               = 80
  target_proto              = "HTTP"
  target_typ                = "ip"
  target_dereg              = 60
  health_thsh               = 3
  health_unthsh             = 2
  health_intv               = 7
  health_port               = "traffic-port"
  health_time               = 5
  listen_type               = "forward"
  listen_field              = "path-pattern"
}


# application load balancer
resource "aws_lb" "dpd_document_public" {
    name                    = "${var.project}-${var.environment}-api-alb"
    internal                = false
    load_balancer_type      = "application"
    idle_timeout            = 600
    security_groups         = ["${aws_security_group.public.id}"]
    subnets                 = ["${var.pub_subnet1}", "${var.pub_subnet2}", "${var.pub_subnet3}", "${var.pub_subnet4}"]

    tags = {
      Name                  = "${var.project}-${var.environment}-api-alb"
      Department            = "${var.department}"
      Project               = "${var.project}"
      Environment           = "${var.environment}"
    }
}

# http listener redirect to https
resource "aws_alb_listener" "dpd_document_public" {
    load_balancer_arn = "${aws_lb.dpd_document_public.arn}"
    port              = "80"
    protocol          = "HTTP"

    default_action {
    type = "redirect"

    redirect {
      port        = "443"
      protocol    = "HTTPS"
      status_code = "HTTP_301"
    }
  }
}

# default https listener
resource "aws_alb_listener" "dpd_document_public2" {
    load_balancer_arn = "${aws_lb.dpd_document_public.arn}"
    port              = "443"
    protocol          = "HTTPS"
    ssl_policy        = "ELBSecurityPolicy-2016-08"
    certificate_arn   = "${var.certificate_arn}"

    default_action {
        target_group_arn = "${module.dpd_document_api_tg.tgb_arn}"
        type             = "forward"
    }
}

# DNS alias record
resource "aws_route53_record" "dpd_document_api" {
  zone_id = "${var.dns_zone_id_api}"
  name    = ""
  type    = "A"

  alias {
    name                   = "${aws_lb.dpd_document_public.dns_name}"
    zone_id                = "${aws_lb.dpd_document_public.zone_id}"
    evaluate_target_health = false
  }
} 

resource "aws_route53_record" "dpd_document_ui" {
  zone_id = "${var.dns_zone_id_ui}"
  name    = ""
  type    = "A"

  alias {
    name                   = "${aws_cloudfront_distribution.dpd_document_cf_dist.domain_name}"
    zone_id                = "${aws_cloudfront_distribution.dpd_document_cf_dist.hosted_zone_id}"
    evaluate_target_health = false
  }
} 
