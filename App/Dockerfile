﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS certs
RUN dotnet dev-certs https -ep aspnetapp.pfx -p docker_cert_password
RUN dotnet dev-certs https --trust

FROM certs AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VerticalSlice.csproj", "./"]
RUN dotnet restore "VerticalSlice.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "VerticalSlice.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VerticalSlice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VerticalSlice.dll", "--launch-profile https-prod"]
