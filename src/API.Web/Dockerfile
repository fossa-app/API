FROM mcr.microsoft.com/dotnet/aspnet:8.0.0
WORKDIR /App
COPY . ./
USER app
ENTRYPOINT ["dotnet", "API.Web.dll"]
HEALTHCHECK \
  CMD curl -f http://localhost/ || exit 1
