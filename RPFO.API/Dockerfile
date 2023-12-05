#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["RPFO.Application/nuget.config", "RPFO.Application/"]
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