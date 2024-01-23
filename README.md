# SnsTestReceiver
Simple API to receive and manage SNS test notifications. It could be very useful in integration tests to assert if SNS subscribers are receiving the expected notifications. The introductional blog post can be found [here](https://ignas.me/tech/snstestreceiver-introduction/).

## Setup

The setup consists of 3 simple steps.

### 1. Pull the Docker image
Add the following into your `docker-compose.yml` file:
```
sns-test-receiver:
  image: ignassakalauskas/sns-test-receiver:latest
  container_name: sns-test-receiver
  ports:
    - "5000:5000"
```
### 2. Setup SNS subscriber
Add `SnsTestReceiver` as SNS subscriber using HTTP protocol. For example, when using Localstack and AWS CLI:
```
awslocal sns create-topic --name api-notifications
awslocal sns subscribe --topic-arn "arn:aws:sns:eu-west-1:000000000000:notifications" --protocol http --notification-endpoint http://sns-test-receiver:5000/messages
```

### 3. Install the SDK
Install the [SnsTestReceiver.Sdk](https://www.nuget.org/packages/SnsTestReceiver.Sdk/) NuGet package into your test project.
```
dotnet add package SnsTestReceiver.Sdk
```
## Examples
See `example` folder.

## Notes
- If you use a different programming language, you can still use the `SnsTestReceiver`, however you need to write your own SDK.

## Upgrade localstack from 0.x to 3.x
- Specify `AuthenticationRegion` in AWS config (your `appsettings.json` for example)
- Make sure to confirm SNS subscriptions, e.g. use `ConfirmSubscriptionAsync()` in the `SnsTestReceiver.Sdk` nuget package.