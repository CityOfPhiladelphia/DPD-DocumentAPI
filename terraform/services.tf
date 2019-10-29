# cluster
resource "aws_ecs_cluster" "ecs_cluster" {
    name = "${var.project}-${var.environment}-cluster"
}

# tasks
# API
module "dpd_document_api_td" {
  source                    = "app.terraform.io/philadelphia/ecs-task/aws"
  version                   = "0.1.15"
  #vpc                       = "${var.vpc_id}"
  dept                      = "${var.department}"
  env                       = "${var.environment}"
  proj                      = "${var.project}"
  infra                     = "api"
  region                    = "${var.aws_region}"
  temp_file                 = "${file("task_definition/dpd-document-api.json")}"
  task_role                 = "${aws_iam_role.ecs_task_role.arn}"
  req_comp                  = ["FARGATE"]
  launch_type               = "FARGATE"
  ecs_clus                  = "${aws_ecs_cluster.ecs_cluster.id}"
  ecs_clus2                 = "${aws_ecs_cluster.ecs_cluster.name}"
  ecs_role                  = "${aws_iam_role.ecs_service_role.name}"
  net_mode                  = "awsvpc"
  task_subnets              = ["${var.priv_subnet1}", "${var.priv_subnet2}", "${var.priv_subnet3}", "${var.priv_subnet4}"]
  task_security             = ["${aws_security_group.public.id}", "${aws_security_group.dpd_document_ecs.id}"]
  desired                   = 1
  depl_max                  = 200
  depl_min                  = 100
  health_grace              = 300
  #depends                   = ["${aws_iam_role_policy.ecs_service_role_policy}"]
  place_type                = "spread"
  place_field               = "host"
  depl_type                 = "ECS"
  tar_grp                   = "${module.dpd_document_api_tg.tgb_arn}"
  cont_port                 = 80
  cpu_val                   = 256
  mem_val                   = 1024
  adj_type                  = "ChangeInCapacity"
  adj_val1                  = 1
  adj_val2                  = -1
  scale_cool                = 300
  scale_pol                 = "SimpleScaling"
  comp_op1                  = "GreaterThanOrEqualToThreshold"
  comp_op2                  = "LessThanOrEqualToThreshold"
  per_eval1                 = 4
  per_eval2                 = 4
  met_name                  = "CPUUtilization"
  name_spc                  = "AWS/ECS"
  per_len                   = 60
  met_stat                  = "Average"
  per_alrm1                 = 2
  per_alrm2                 = 3
  met_thr1                  = 80
  met_thr2                  = 50
  #dim_name                  = "ServiceName"
  #dim_val                   = "${aws_ecs_service.this.name}"
  min_count                 = "${var.min_task_count}"
  max_count                 = "${var.max_task_count}"
  ecs_scale_role            = "arn:aws:iam::102658671810:role/ecsAutoscaleRole"
}
