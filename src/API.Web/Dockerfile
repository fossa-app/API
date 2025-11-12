FROM mcr.microsoft.com/dotnet/aspnet:10.0.0
RUN apt-get update && apt-get upgrade -y && apt-get install -y curl=8.5.0-2ubuntu10.6 && apt-get clean
WORKDIR /App
COPY . ./
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.Web.dll"]
HEALTHCHECK \
  CMD curl -f http://localhost:8080/healthchecks || exit 1
