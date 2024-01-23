#!/bin/sh
echo "Starting init.sh script..."

awslocal sns create-topic --name test-notifications
awslocal sns subscribe --topic-arn "arn:aws:sns:eu-west-1:000000000000:test-notifications" --protocol http --notification-endpoint http://sns-test-receiver:5000/messages

echo "Exiting init.sh script..."
