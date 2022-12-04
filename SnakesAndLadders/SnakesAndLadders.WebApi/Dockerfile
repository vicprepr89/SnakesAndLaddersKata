#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SnakesAndLadders.WebApi/SnakesAndLadders.WebApi.csproj", "SnakesAndLadders.WebApi/"]
RUN dotnet restore "SnakesAndLadders.WebApi/SnakesAndLadders.WebApi.csproj"
COPY . .
WORKDIR "/src/SnakesAndLadders.WebApi"
RUN dotnet build "SnakesAndLadders.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SnakesAndLadders.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SnakesAndLadders.WebApi.dll"]