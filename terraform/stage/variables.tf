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
  default = "test"
}

variable "aws_region" {
  default = "us-east-1"
}

variable "vpc_id" {
  default = "vpc-06646e2e25a0733fb"
}

variable "priv_subnet1" {
  default = "subnet-0c7be3e16f90c7e70"
}

variable "priv_subnet2" {
  default = "subnet-0de1e42f3a598e844"
}

variable "priv_subnet3" {
  default = "subnet-0848ae13242c33d69"
}

variable "priv_subnet4" {
  default = "subnet-07ea165e6ad904991"
}

variable "pub_subnet1" {
  default = "subnet-0c148092c0246575d"
}

variable "pub_subnet2" {
  default = "subnet-0d016ab8d193f330a"
}

variable "pub_subnet3" {
  default = "subnet-049834c34a74c4e3d"
}

variable "pub_subnet4" {
  default = "subnet-0a9e29a49fd8d4817"
}

variable "certificate_arn" {
  default = "arn:aws:acm:us-east-1:922311303087:certificate/e67f832a-9203-4d1b-a769-3566e4c9db06"
}

terraform {
  backend "s3" {
    bucket = "dpd-document-api-tf-state"
    key    = "stage/terraform.tfstate"
    region = "us-east-1"

    dynamodb_table = "dpd-document-api-tf-state-locks"
    encrypt        = true
  }
}
