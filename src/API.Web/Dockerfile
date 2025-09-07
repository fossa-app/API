FROM mcr.microsoft.com/dotnet/aspnet:9.0.8
RUN apt-get update && apt-get upgrade -y && apt-get install -y curl=7.88.1-10+deb12u14 && apt-get clean
WORKDIR /App
COPY . ./
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.Web.dll"]
HEALTHCHECK \
  CMD curl -f http://localhost:8080/healthchecks || exit 1
