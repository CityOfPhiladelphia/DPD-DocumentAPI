# logs.tf

# Set up CloudWatch group and log stream and retain logs for 30 days
resource "aws_cloudwatch_log_group" "dpd_document_api_log_group" {
  name              = "${var.project}-${var.environment}-api"
  retention_in_days = 30

  tags = {
    Name = "dpd-document-api-log-group"
  }
}

resource "aws_cloudwatch_log_stream" "dpd_document_api_log_stream" {
  name           = "dpd-document-api-log-stream"
  log_group_name = "${aws_cloudwatch_log_group.dpd_document_api_log_group.name}"
}
