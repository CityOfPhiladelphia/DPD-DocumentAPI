# cluster
resource "aws_ecs_cluster" "ecs_cluster" {
  name = "${var.project}-${var.environment}-cluster"
}

# tasks
# API

data "template_file" "dpd_document_api" {
  template = "${file("task_definition/dpd-document-api.json")}"
  vars = {
    project                = "${var.project}"
    environment            = "${var.environment}"
    infra                  = "api"
    log_group_name         = "${aws_cloudwatch_log_group.dpd_document_api_log_group.name}"
    log_group_region       = "${var.aws_region}"
    Credentials            = "${var.Credentials}"
    RequestBasePath        = "${var.RequestBasePath}"
    AdHocQueryResultsPath  = "${var.AdHocQueryResultsPath}"
    SelectIndexLookupPath  = "${var.SelectIndexLookupPath}"
    QueryAppsPath          = "${var.QueryAppsPath}"
    RetrieveDocumentPath   = "${var.RetrieveDocumentPath}"
    ExportDocumentPath     = "${var.ExportDocumentPath}"
    OracleConnectionString = "${var.OracleConnectionString}"
  }
}

resource "aws_ecs_task_definition" "dpd_document_api_task" {
  family                   = "dpd-document-api-task"
  execution_role_arn       = "${aws_iam_role.ecs_task_role.arn}"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 1024
  container_definitions    = "${data.template_file.dpd_document_api.rendered}"
}

resource "aws_ecs_service" "main" {
  name            = "document-api-service"
  cluster         = "${aws_ecs_cluster.ecs_cluster.id}"
  task_definition = "${aws_ecs_task_definition.dpd_document_api_task.arn}"
  desired_count   = 2
  launch_type     = "FARGATE"

  network_configuration {
    security_groups  = ["${aws_security_group.public.id}", "${aws_security_group.dpd_document_ecs.id}"]
    subnets          = ["${var.priv_subnet1}", "${var.priv_subnet2}", "${var.priv_subnet3}", "${var.priv_subnet4}"]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = "${aws_lb_target_group.dpd_front_end.arn}"
    container_name   = "${var.project}-${var.environment}-api"
    container_port   = 80
  }

  depends_on = [aws_alb_listener.dpd_document_public2, aws_iam_role.ecs_task_role]
}
