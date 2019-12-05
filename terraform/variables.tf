provider "aws" {
  region = "${var.aws_region}"
}

variable "project" {
  default = "dpd-document-api"
}

variable "department" {
  default = "dpd"
}

variable "environment" {
  default = "prod"
}

variable "aws_region" {
  default = "us-east-1"
}

variable "vpc_id" {
  default = "vpc-0b5d7aefdcf5d2a7c"
}

variable "priv_subnet1" {
  default = "subnet-00c0eb1522ceef2eb"
}

variable "priv_subnet2" {
  default = "subnet-05479a00b2c80c001"
}

variable "priv_subnet3" {
  default = "subnet-0d457b51ad472d882"
}

variable "priv_subnet4" {
  default = "subnet-007593c9b70024d7a"
}

variable "pub_subnet1" {
  default = "subnet-01b6d0f9d9f316d2b"
}

variable "pub_subnet2" {
  default = "subnet-07cbe86f23bb1004d"
}

variable "pub_subnet3" {
  default = "subnet-0ba3d76c195fe3c8c"
}

variable "pub_subnet4" {
  default = "subnet-0da21664b0a623486"
}

variable "enviro_name" {
  default = "Prod"
}

variable "certificate_arn" {
  default = "arn:aws:acm:us-east-1:922311303087:certificate/e67f832a-9203-4d1b-a769-3566e4c9db06"
}

variable "pipe_owner" {
  default = ""
}

variable "pipe_token" {
  default = ""
}

variable "pipe_repo" {
  default = ""
}

variable "pipe_branch" {
  default = ""
}

variable "min_task_count" {
  default = 1
}

variable "max_task_count" {
  default = 2
}

variable "dns_zone_id_api" {
  default = ""
}

variable "cf_aliases" {
  default =""
}