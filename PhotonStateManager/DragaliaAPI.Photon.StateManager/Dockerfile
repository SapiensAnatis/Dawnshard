FROM mcr.microsoft.com/dotnet/aspnet:9.0.1-noble-chiseled AS base
USER app
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0.102 AS build
WORKDIR /src
COPY ["PhotonStateManager/Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["PhotonStateManager/DragaliaAPI.Photon.StateManager/DragaliaAPI.Photon.StateManager.csproj", "PhotonStateManager/DragaliaAPI.Photon.StateManager/"]
COPY ["Aspire/Dawnshard.ServiceDefaults/Dawnshard.ServiceDefaults.csproj", "Aspire/Dawnshard.ServiceDefaults/"]
COPY ["Shared/DragaliaAPI.Photon.Shared/DragaliaAPI.Photon.Shared.csproj", "Shared/DragaliaAPI.Photon.Shared/"]
RUN dotnet restore "PhotonStateManager/DragaliaAPI.Photon.StateManager/DragaliaAPI.Photon.StateManager.csproj"
COPY [".editorconfig", ".editorconfig"]
COPY ["PhotonStateManager/", "PhotonStateManager/"]
COPY ["Aspire/", "Aspire/"]
COPY ["Shared/", "Shared/"]
WORKDIR "/src/PhotonStateManager/DragaliaAPI.Photon.StateManager"
RUN dotnet publish "DragaliaAPI.Photon.StateManager.csproj" -c Release -o /app/publish/ /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "DragaliaAPI.Photon.StateManager.dll"]
