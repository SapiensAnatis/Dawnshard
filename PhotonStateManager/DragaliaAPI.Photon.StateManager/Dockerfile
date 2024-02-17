#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PhotonStateManager/DragaliaAPI.Photon.StateManager/DragaliaAPI.Photon.StateManager.csproj", "PhotonStateManager/DragaliaAPI.Photon.StateManager/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
RUN dotnet restore "PhotonStateManager/DragaliaAPI.Photon.StateManager/DragaliaAPI.Photon.StateManager.csproj"
COPY . .
WORKDIR "/src/PhotonStateManager/DragaliaAPI.Photon.StateManager"
RUN dotnet build "DragaliaAPI.Photon.StateManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DragaliaAPI.Photon.StateManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DragaliaAPI.Photon.StateManager.dll"]