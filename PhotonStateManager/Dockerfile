FROM mcr.microsoft.com/dotnet/aspnet:8.0.5-jammy-chiseled AS base
USER app
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS build
WORKDIR /src
COPY ["PhotonStateManager/DragaliaAPI.Photon.StateManager/DragaliaAPI.Photon.StateManager.csproj", "PhotonStateManager/DragaliaAPI.Photon.StateManager/"]
COPY ["PhotonStateManager/Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
RUN dotnet restore "PhotonStateManager/DragaliaAPI.Photon.StateManager/DragaliaAPI.Photon.StateManager.csproj"
COPY [".editorconfig", ".editorconfig"]
COPY ["PhotonStateManager/", "PhotonStateManager/"]
COPY ["Shared/", "Shared/"]
WORKDIR "/src/PhotonStateManager/DragaliaAPI.Photon.StateManager"
RUN dotnet publish "DragaliaAPI.Photon.StateManager.csproj" -c Release -o /app/publish/ /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "DragaliaAPI.Photon.StateManager.dll"]