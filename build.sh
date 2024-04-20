# nuget
dotnet pack

# default platform (hardcoded tag)
docker build . --file src/SnsTestReceiver.Api/Dockerfile --tag ignassakalauskas/sns-test-receiver:2.0-beta3

# multi-platform (hardcoded tag)
docker buildx create --name mybuilder --bootstrap --use
docker buildx use --builder mybuilder
docker buildx build --platform linux/amd64,linux/arm64 --tag ignassakalauskas/sns-test-receiver:3.0-beta1 . --file ./src/SnsTestReceiver.Api/Dockerfile --no-cache --progress=plain --push

docker buildx build --load --tag ignassakalauskas/sns-test-receiver:3.0-beta1 . --file ./src/SnsTestReceiver.Api/Dockerfile
