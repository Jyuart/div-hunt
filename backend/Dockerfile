# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.sln .
COPY *.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
FROM build AS publish
RUN dotnet publish -c Release -o publish  /p:UseAppHost=false

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=publish /src/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "DivHunt.dll"]