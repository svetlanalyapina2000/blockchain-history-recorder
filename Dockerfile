FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 30122

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BlockchainHistoryRecorder/BlockchainHistoryRecorder.csproj", "src/BlockchainHistoryRecorder/"]
RUN dotnet restore "src/BlockchainHistoryRecorder/BlockchainHistoryRecorder.csproj"
COPY . .
WORKDIR "/src/src/BlockchainHistoryRecorder"
RUN dotnet build "BlockchainHistoryRecorder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BlockchainHistoryRecorder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlockchainHistoryRecorder.dll"]
