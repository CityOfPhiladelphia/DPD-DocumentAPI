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
  default = "vpc-0200766a34fd6eb89"
}

variable "priv_subnet1" {
  default = "subnet-0cfa5c7ad7906503f"
}

variable "priv_subnet2" {
  default = "subnet-055665485afa3f3b9"
}

variable "priv_subnet3" {
  default = "subnet-03de5e74c20ff1dad"
}

variable "priv_subnet4" {
  default = "subnet-0748ba7c561d484f3"
}

variable "pub_subnet1" {
  default = "subnet-0399cdb2d462450ef"
}

variable "pub_subnet2" {
  default = "subnet-08ed747a342323e3f"
}

variable "pub_subnet3" {
  default = "subnet-070ef19ef7b257410"
}

variable "pub_subnet4" {
  default = "subnet-0c1ff8fe33d79f9d7"
}

variable "certificate_arn" {
  default = "arn:aws:acm:us-east-1:922311303087:certificate/e67f832a-9203-4d1b-a769-3566e4c9db06"
}

terraform {
  backend "s3" {
    bucket = "dpd-document-api-tf-state"
    key    = "prod/terraform.tfstate"
    region = "us-east-1"

    dynamodb_table = "dpd-document-api-tf-state-locks"
    encrypt        = true
  }
}
