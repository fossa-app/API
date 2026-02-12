FROM --platform=$TARGETARCH mcr.microsoft.com/dotnet/sdk:10.0.103 AS build
ARG TARGETARCH
WORKDIR /source
COPY --link . /source
RUN dotnet restore --runtime linux-$TARGETARCH
WORKDIR /source/src/API.Web
RUN dotnet publish --no-restore --configuration Release --runtime linux-$TARGETARCH --no-self-contained --output /app

FROM --platform=$TARGETARCH mcr.microsoft.com/dotnet/aspnet:10.0.3 AS runtime
RUN apt-get update && apt-get upgrade -y && apt-get install -y curl=8.5.0-2ubuntu10.6 && apt-get clean
WORKDIR /app
COPY --link --from=build /app .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.Web.dll"]
HEALTHCHECK \
  CMD curl -f http://localhost:8080/healthchecks || exit 1
