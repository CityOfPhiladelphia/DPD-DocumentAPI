# alb public security group
resource "aws_security_group" "public" {
  name        = "${var.project}-${var.environment}-public"
  description = "Allow inbound traffic from the internet"
  vpc_id      = "${var.vpc_id}"

  ingress {
    # TLS (change to whatever ports you need)
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "allow https from the internet"
  }

  ingress {
    # TLS (change to whatever ports you need)
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "allow https from the internet"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
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



# allow access to the database from the api servers
# this is not automatically applied to the database
resource "aws_security_group" "dpd_document_database" {
  name        = "${var.project}-${var.environment}-db"
  description = "Allow inbound traffic from servers"
  vpc_id      = "${var.vpc_id}"

  ingress {
    # TLS (change to whatever ports you need)
    from_port   = 1521
    to_port     = 1521
    protocol    = "tcp"
    cidr_blocks = ["10.8.100.0/22"]
    description = "allow connections from citynet"
  }

  ingress {
    # TLS (change to whatever ports you need)
    from_port       = 1521
    to_port         = 1521
    protocol        = "tcp"
    security_groups = ["${aws_security_group.dpd_document_ecs.id}"]
    description     = "allow connections from the api servers"
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
    Name = "${var.project}-${var.environment}-database"
  }
}
