# nuget
dotnet pack

# default platform (hardcoded tag)
docker build . --file src/SnsTestReceiver.Api/Dockerfile --tag ignassakalauskas/sns-test-receiver:3.0.1

# multi-platform (hardcoded tag)
docker buildx create --name mybuilder --bootstrap --use
docker buildx use --builder mybuilder
docker buildx build --platform linux/amd64,linux/arm64,linux/arm64/v7 --tag ignassakalauskas/sns-test-receiver:3.0.1 . --file ./src/SnsTestReceiver.Api/Dockerfile --no-cache --progress=plain --push

docker buildx build --load --tag ignassakalauskas/sns-test-receiver:3.0.1 . --file ./src/SnsTestReceiver.Api/Dockerfile

# environment variables for local development
export AWS_ACCESS_KEY_ID=xx
export AWS_SECRET_ACCESS_KEY=xx
export AWS_DEFAULT_REGION=eu-west-1
