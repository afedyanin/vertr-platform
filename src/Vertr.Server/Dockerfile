# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8008

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/Vertr.Server/Vertr.Server.csproj", "src/Vertr.Server/"]
COPY ["src/Vertr.Adapters.DataAccess/Vertr.Adapters.DataAccess.csproj", "src/Vertr.Adapters.DataAccess/"]
COPY ["src/Vertr.Adapters.Prediction/Vertr.Adapters.Prediction.csproj", "src/Vertr.Adapters.Prediction/"]
COPY ["src/Vertr.Adapters.Tinvest/Vertr.Adapters.Tinvest.csproj", "src/Vertr.Adapters.Tinvest/"]
COPY ["src/Vertr.Application/Vertr.Application.csproj", "src/Vertr.Application/"]
COPY ["src/Vertr.Domain/Vertr.Domain.csproj", "src/Vertr.Domain/"]

RUN dotnet restore "./src/Vertr.Server/Vertr.Server.csproj"
COPY . .
WORKDIR "/src/src/Vertr.Server"
RUN dotnet build "./Vertr.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vertr.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vertr.Server.dll"]
