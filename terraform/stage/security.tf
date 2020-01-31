# alb public security group
resource "aws_security_group" "public" {
  name_prefix = "${var.project}-${var.environment}-public"
  description = "Allow inbound traffic from City machines and gatekeeper"
  vpc_id      = "${var.vpc_id}"

  ingress {
    # TLS (change to whatever ports you need)
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = "${var.CityTrafficCIDRBlocks}"
    description = "allow https from the City machines and gatekeeper"
  }

  ingress {
    # TLS (change to whatever ports you need)
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = "${var.CityTrafficCIDRBlocks}"
    description = "allow http from the City machines and gatekeeper"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  lifecycle {
    create_before_destroy = true
  }

  tags = {
    Name = "${var.project}-${var.environment}-public"
  }
}

# Document API
# allow access to the front end servers from the load balancer
resource "aws_security_group" "dpd_document_ecs" {
  name        = "${var.project}-${var.environment}-ecs"
  description = "Allow inbound traffic from the load balancer"
  vpc_id      = "${var.vpc_id}"

  ingress {
    # TLS (change to whatever ports you need)
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = ["${aws_security_group.public.id}"]
    description     = "allow https from the load balancer"
  }

  ingress {
    # TLS (change to whatever ports you need)
    from_port       = 80
    to_port         = 80
    protocol        = "tcp"
    security_groups = ["${aws_security_group.public.id}"]
    description     = "allow https from the load balancer"
  }

  ingress {
    # TLS (change to whatever ports you need)
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    security_groups = ["${aws_security_group.public.id}"]
    description     = "allow https from the load balancer"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  lifecycle {
    create_before_destroy = true
  }

  tags = {
    Name = "${var.project}-${var.environment}-ecs"
  }
}

# allow access to the ApplicationXTender from the api servers
resource "aws_security_group" "dpd_document_applicationXTender" {
  name        = "${var.project}-${var.environment}-db"
  description = "Allow inbound traffic from servers"
  vpc_id      = "${var.vpc_id}"

  ingress {
    # TLS (change to whatever ports you need)
    from_port       = 1521
    to_port         = 1521
    protocol        = "tcp"
    security_groups = ["${aws_security_group.dpd_document_ecs.id}"]
    cidr_blocks     = "${var.ApplicationXTenderCIDRBlocks}"
    description     = "allow connections from AX DB"
  }

  ingress {
    # TLS (change to whatever ports you need)
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = ["${aws_security_group.dpd_document_ecs.id}"]
    cidr_blocks     = "${var.ApplicationXTenderCIDRBlocks}"
    description     = "allow connections from AX API"
  }

  egress {
    from_port       = 1521
    to_port         = 1521
    protocol        = "tcp"
    security_groups = ["${aws_security_group.dpd_document_ecs.id}"]
    cidr_blocks     = "${var.ApplicationXTenderCIDRBlocks}"
  }

  egress {
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = ["${aws_security_group.dpd_document_ecs.id}"]
    cidr_blocks     = "${var.ApplicationXTenderCIDRBlocks}"
  }

  lifecycle {
    create_before_destroy = true
  }

  tags = {
    Name = "${var.project}-${var.environment}-database"
  }
}
