#!/bin/sh
echo "Starting init.sh script..."

awslocal sns create-topic --name test-notifications
awslocal sqs create-queue --queue-name notifications --attributes file:///etc/localstack/init/ready.d/sqs.json
awslocal sns subscribe --topic-arn "arn:aws:sns:eu-west-1:000000000000:test-notifications" --protocol sqs --notification-endpoint arn:aws:sqs:eu-west-1:000000000000:notifications

echo "Exiting init.sh script..."
