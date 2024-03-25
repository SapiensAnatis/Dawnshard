FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DragaliaAPI/DragaliaAPI.MemoryPack/DragaliaAPI.MemoryPack.csproj", "DragaliaAPI/DragaliaAPI.MemoryPack/"]
RUN dotnet restore "DragaliaAPI/DragaliaAPI.MemoryPack/DragaliaAPI.MemoryPack.csproj"
COPY . .
WORKDIR "/src/DragaliaAPI/DragaliaAPI.MemoryPack"
RUN dotnet build "DragaliaAPI.MemoryPack.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DragaliaAPI.MemoryPack.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DragaliaAPI.MemoryPack.dll"]
