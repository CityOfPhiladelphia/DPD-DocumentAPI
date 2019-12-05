# dpd-document-api
# application load balancer
resource "aws_lb" "dpd_document_public" {
  name               = "${var.project}-${var.environment}-api-alb"
  internal           = false
  load_balancer_type = "application"
  idle_timeout       = 600
  security_groups    = ["${aws_security_group.public.id}"]
  subnets            = ["${var.pub_subnet1}", "${var.pub_subnet2}", "${var.pub_subnet3}", "${var.pub_subnet4}"]

  tags = {
    Name        = "${var.project}-${var.environment}-api-alb"
    Department  = "${var.department}"
    Project     = "${var.project}"
    Environment = "${var.environment}"
  }
}

resource "aws_lb_target_group" "dpd_front_end" {
  lifecycle {
    create_before_destroy = true
  }
  name = "dpd-document-target-group"

  port        = 80
  protocol    = "HTTP"
  vpc_id      = "${var.vpc_id}"
  target_type = "ip"
  health_check {
    healthy_threshold   = 3
    unhealthy_threshold = 2
    protocol            = "HTTP"
    timeout             = 60
    path                = "/"
    interval            = 65
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
  ssl_policy        = "ELBSecurityPolicy-FS-1-2-2019-08"
  certificate_arn   = "${var.certificate_arn}"

  default_action {
    target_group_arn = "${aws_lb_target_group.dpd_front_end.arn}"
    type             = "forward"
  }
}

# DNS alias record
# resource "aws_route53_record" "dpd_document_api" {
#   zone_id = "${var.dns_zone_id_api}"
#   name    = ""
#   type    = "A"

#   alias {
#     name                   = "${aws_lb.dpd_document_public.dns_name}"
#     zone_id                = "${aws_lb.dpd_document_public.zone_id}"
#     evaluate_target_health = false
#   }
# }

