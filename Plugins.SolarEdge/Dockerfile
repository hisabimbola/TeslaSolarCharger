#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim-amd64 AS build
WORKDIR /src
COPY ["Plugins.SolarEdge/Plugins.SolarEdge.csproj", "Plugins.SolarEdge/"]
RUN dotnet restore "Plugins.SolarEdge/Plugins.SolarEdge.csproj"
COPY . .
WORKDIR "/src/Plugins.SolarEdge"
RUN dotnet build "Plugins.SolarEdge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Plugins.SolarEdge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plugins.SolarEdge.dll"]