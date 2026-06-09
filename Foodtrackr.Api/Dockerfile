FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Foodtrackr.Api/Foodtrackr.Api.csproj", "Foodtrackr.Api/"]
RUN dotnet restore "./Foodtrackr.Api/Foodtrackr.Api.csproj"
COPY . .
WORKDIR "/src/Foodtrackr.Api"
RUN dotnet build "./Foodtrackr.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Foodtrackr.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Foodtrackr.Api.dll"]