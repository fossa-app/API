FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0.101 AS build
ARG TARGETARCH
WORKDIR /source
COPY --link . /source
RUN dotnet restore -a $TARGETARCH
WORKDIR /source/src/API.Web
RUN dotnet publish -a $TARGETARCH --no-restore -o /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0.4 AS runtime
RUN apt-get update && apt-get upgrade -y && apt-get install -y curl=7.88.1-10+deb12u8 && apt-get clean
WORKDIR /app
COPY --link --from=build /app .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.Web.dll"]
HEALTHCHECK \
  CMD curl -f http://localhost:8080/healthchecks || exit 1
