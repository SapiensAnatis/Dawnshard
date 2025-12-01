FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-noble-chiseled AS base
USER app
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0.100 AS build
WORKDIR /src
COPY ["DragaliaAPI/Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["Aspire/Dawnshard.ServiceDefaults/Dawnshard.ServiceDefaults.csproj", "Aspire/Dawnshard.ServiceDefaults/"]
COPY ["Shared/DragaliaAPI.Photon.Shared/DragaliaAPI.Photon.Shared.csproj", "Shared/DragaliaAPI.Photon.Shared/"]
COPY ["DragaliaAPI/DragaliaAPI/DragaliaAPI.csproj", "DragaliaAPI/DragaliaAPI/"]
COPY ["DragaliaAPI/DragaliaAPI.Database/DragaliaAPI.Database.csproj", "DragaliaAPI/DragaliaAPI.Database/"]
COPY ["DragaliaAPI/DragaliaAPI.Shared/DragaliaAPI.Shared.csproj", "DragaliaAPI/DragaliaAPI.Shared/"]
COPY ["DragaliaAPI/DragaliaAPI.Shared.SourceGenerator/DragaliaAPI.Shared.SourceGenerator.csproj", "DragaliaAPI/DragaliaAPI.Shared.SourceGenerator/"]
COPY ["DragaliaAPI/DragaliaAPI.MasterAssetConverter/DragaliaAPI.MasterAssetConverter.csproj", "DragaliaAPI/DragaliaAPI.MasterAssetConverter/"]
COPY ["DragaliaAPI/DragaliaAPI.MissionDesigner/DragaliaAPI.MissionDesigner.csproj", "DragaliaAPI/DragaliaAPI.MissionDesigner/"]
RUN dotnet restore "DragaliaAPI/DragaliaAPI/DragaliaAPI.csproj"
COPY [".editorconfig", ".editorconfig"]
COPY ["DragaliaAPI/", "DragaliaAPI/"]
COPY ["Aspire/", "Aspire/"]
COPY ["Shared/", "Shared/"]
WORKDIR "/src/DragaliaAPI/DragaliaAPI"
RUN dotnet publish "DragaliaAPI.csproj" -c Release -o /app/publish/ /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "DragaliaAPI.dll"]

