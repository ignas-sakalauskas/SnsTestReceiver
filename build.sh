# nuget
dotnet pack

# default platform (hardcoded tag)
docker build . --file src/SnsTestReceiver.Api/Dockerfile --tag ignassakalauskas/sns-test-receiver:2.0-beta3

# multi-platform (hardcoded tag)
docker buildx create --name mybuilder --bootstrap --use
docker buildx build --platform linux/amd64,linux/arm64 --tag ignassakalauskas/sns-test-receiver:2.0-beta3 . --file ./src/SnsTestReceiver.Api/Dockerfile --push
