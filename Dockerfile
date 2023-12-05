#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app



EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
# Specify your DevExpress NuGet Feed URL as the package source
RUN dotnet nuget add source https://nuget.devexpress.com/pT8hP28fJhQmmhPfxCgOhqTB6IGdr8y2Pm2lGDA8yyZL1GySQW/api
 

WORKDIR /src
COPY ["RPFO.API/RPFO.API.csproj", "RPFO.API/"]
COPY ["RPFO.Utilities/RPFO.Utilities.csproj", "RPFO.Utilities/"]
COPY ["RPFO.Data/RPFO.Data.csproj", "RPFO.Data/"]
COPY ["RPFO.Application/RPFO.Application.csproj", "RPFO.Application/"]
RUN dotnet restore "RPFO.API/RPFO.API.csproj"
COPY . .
WORKDIR "/src/RPFO.API"
RUN dotnet build "RPFO.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPFO.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPFO.API.dll"]