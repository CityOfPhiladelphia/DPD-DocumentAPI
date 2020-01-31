resource "aws_iam_role" "ecs_task_role" {
    name                = "${var.project}-${var.environment}-task-role"
    path                = "/"
    assume_role_policy  = "${data.aws_iam_policy_document.ecs_task_policy.json}"
}

resource "aws_iam_role_policy_attachment" "ecs_task_role_attachment1" {
    role       = "${aws_iam_role.ecs_task_role.name}"
    policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy_attachment" "ecs_task_role_attachment2" {
    role       = "${aws_iam_role.ecs_task_role.name}"
    policy_arn = "arn:aws:iam::aws:policy/AmazonECS_FullAccess"
}

resource "aws_iam_role_policy_attachment" "ecs_task_role_attachment3" {
    role       = "${aws_iam_role.ecs_task_role.name}"
    policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryFullAccess"
}

data "aws_iam_policy_document" "ecs_task_policy" {
    statement {
        actions = ["sts:AssumeRole"]

        principals {
            type        = "Service"
            identifiers = ["ecs-tasks.amazonaws.com"]
        }
    }
}