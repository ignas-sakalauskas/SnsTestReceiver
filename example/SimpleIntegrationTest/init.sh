#!/bin/sh
echo "Starting init.sh script..."

awslocal sns create-topic --name notifications
awslocal sns subscribe --topic-arn "arn:aws:sns:eu-west-1:000000000000:notifications" --protocol http --notification-endpoint http://sns-test-receiver:5000/messages

echo "Exiting init.sh script..."
