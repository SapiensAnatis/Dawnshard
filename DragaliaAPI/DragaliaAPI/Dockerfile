ARG BASE_DOTNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:8.0.3

FROM $BASE_DOTNET_IMAGE AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0.203 AS build

ARG CI

WORKDIR /src
COPY ["DragaliaAPI/DragaliaAPI.MissionDesigner/DragaliaAPI.MissionDesigner.csproj", "DragaliaAPI/DragaliaAPI.MissionDesigner/"]
COPY ["DragaliaAPI/DragaliaAPI.MasterAssetConverter/DragaliaAPI.MasterAssetConverter.csproj", "DragaliaAPI/DragaliaAPI.MasterAssetConverter/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
RUN dotnet restore "DragaliaAPI/DragaliaAPI.MissionDesigner/DragaliaAPI.MissionDesigner.csproj"
RUN dotnet restore "DragaliaAPI/DragaliaAPI.MasterAssetConverter/DragaliaAPI.MasterAssetConverter.csproj"
COPY ["DragaliaAPI/DragaliaAPI/DragaliaAPI.csproj", "DragaliaAPI/DragaliaAPI/"]
RUN dotnet restore "DragaliaAPI/DragaliaAPI/DragaliaAPI.csproj"
COPY [".editorconfig", ".editorconfig"]
COPY ["DragaliaAPI/", "DragaliaAPI/"]
COPY ["Shared/", "Shared/"]
WORKDIR "/src/DragaliaAPI/DragaliaAPI"
RUN dotnet publish "DragaliaAPI.csproj" -c Release -o /app/publish/ /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "DragaliaAPI.dll"]