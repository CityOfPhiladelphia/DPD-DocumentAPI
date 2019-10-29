terraform {
  backend "atlas" {
  }
}

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
  default = "vpc-b71df3cc"
}

variable "priv_subnet1" {
  default = "subnet-04fcc79737372193b"
}

variable "priv_subnet2" {
  default = "subnet-017066bae8ead689a"
}

variable "priv_subnet3" {
  default = "subnet-0167b96a0dd79cc41"
}

variable "priv_subnet4" {
  default = "subnet-0435129fa7e7b0e70"
}

variable "pub_subnet1" {
  default = "subnet-0ffee11e2c8c68032"
}

variable "pub_subnet2" {
  default = "subnet-0483b33f74e96bb0c"
}

variable "pub_subnet3" {
  default = "subnet-0aec0cacb2ea97e9d"
}

variable "pub_subnet4" {
  default = "subnet-004b86d7a79e2e030"
}

variable "enviro_name" {
  default = "Test"
}

variable "certificate_arn" {
  default = "arn:aws:acm:us-east-1:102658671810:certificate/d8b8e135-aa30-4fda-b963-2fffc8e48d6d"
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

variable "dns_zone_id_ui" {
  default = ""
}


variable "cf_aliases" {
  default =""
}