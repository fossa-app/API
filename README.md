# FossaApp API
[![Developed by Tigran (TIKSN) Torosyan](https://img.shields.io/badge/Developed%20by-%20Tigran%20%28TIKSN%29%20Torosyan-orange.svg)](https://tiksn.com/)
[![Docker Image Version (latest semver)](https://img.shields.io/docker/v/tiksn/fossa-api?sort=semver)](https://hub.docker.com/r/tiksn/fossa-api)
[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)

The FossaApp API provides backend services for a business management application. It handles entities such as Companies, Employees, and Branches, and includes features for authentication, authorization, and software licensing.

## Getting Started

The easiest way to run the API is by using Docker.

### Prerequisites

*   Docker

### Running with Docker

1.  **Create a `.env` file** with the required configuration variables (see the Configuration section below).
2.  **Run the Docker container:**

    ```sh
    docker run --rm -it -p 8080:8080 --env-file .env tiksn/fossa-api:latest
    ```

## Configuration

The API is configured using environment variables. The following table lists the available variables:

| Variable                 | Description                                                                                                   | Default Value              |
| ------------------------ | ------------------------------------------------------------------------------------------------------------- | -------------------------- |
| `ConnectionStrings__MongoDB` | The connection string for the MongoDB database.                                                               | (none)                     |
| `Identity__RootAddress`  | The root URL of the identity provider (e.g., FusionAuth).                                                       | `http://localhost:9011/`     |
| `Identity__Audience`     | The audience for the JWT tokens.                                                                              | (none)                     |
| `Identity__ApiKey`       | The API key for communicating with the identity provider.                                                       | (none)                     |
| `Paging__MaximumPageSize`  | The maximum number of items that can be requested in a single page.                                           | `100`                      |
| `GeneratorId`            | The ID of the instance for distributed ID generation. Should be unique for each running instance of the API. | `0`                        |
| `ASPNETCORE_ENVIRONMENT`   | The runtime environment. Set to `Development` for development-specific features.                              | `Production`               |
| `ASPNETCORE_URLS`        | The URLs the web host will listen on.                                                                         | `http://+:8080`            |

## API Documentation

The API includes built-in documentation, which is available when the application is running.

*   **Swagger UI**: `http://localhost:8080/swagger`
*   **Scalar**: `http://localhost:8080/scalar`

## Health Checks

The application provides a health check endpoint to monitor its status.

*   **Endpoint**: `http://localhost:8080/healthchecks`

## Building from Source

### Prerequisites

*   .NET 10 SDK
*   An identity provider like FusionAuth running.

### Instructions

1.  **Set up User Secrets:**
    The application requires secrets for the database connection string and identity configuration.

    ```sh
    dotnet user-secrets set "ConnectionStrings:MongoDB" "your-mongodb-connection-string"
    dotnet user-secrets set "Identity:Audience" "your-jwt-audience"
    dotnet user-secrets set "Identity:ApiKey" "your-fusionauth-api-key"
    ```

2.  **Restore dependencies and run the application:**

    ```sh
    dotnet restore
    dotnet run --project src/API.Web
    ```

3.  The API will be available at `http://localhost:5000` (or as configured in `launchSettings.json`).