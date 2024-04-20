FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

ARG TARGETARCH
ARG VERSION=3.0.0

# copy and publish app and libraries
COPY ./src/ ./src/
WORKDIR ./src/SnsTestReceiver.Api/
RUN dotnet publish --arch $TARGETARCH --output /app /p:Version=$VERSION

# final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build /app .

# create non-root user
ARG APP_UID=1000
RUN adduser --uid $APP_UID --disabled-password docker
USER $APP_UID

# configure the entry point
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["./SnsTestReceiver.Api"]