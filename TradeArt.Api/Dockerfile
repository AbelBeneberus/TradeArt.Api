
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG PORT=5000:5000	
WORKDIR /app
EXPOSE $PORT

ENV ASPNETCORE_URLS http://*:$PORT
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TradeArt.Api/TradeArt.Api.csproj", "TradeArt.Api/"]
RUN dotnet restore "TradeArt.Api/TradeArt.Api.csproj"
COPY . .
WORKDIR "/src/TradeArt.Api"
RUN dotnet build "TradeArt.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeArt.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradeArt.Api.dll"]