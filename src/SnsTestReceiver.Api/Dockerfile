FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

# copy everything else and build app
COPY ./src/ ./src/
WORKDIR ./src/SnsTestReceiver.Api/
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=build /src/SnsTestReceiver.Api/out ./

# configure and run
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "SnsTestReceiver.Api.dll"]