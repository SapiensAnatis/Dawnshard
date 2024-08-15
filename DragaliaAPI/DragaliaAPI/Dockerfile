FROM mcr.microsoft.com/dotnet/aspnet:8.0.7-jammy-chiseled AS base
USER app
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.303 AS build
WORKDIR /src
COPY ["DragaliaAPI/DragaliaAPI.MissionDesigner/DragaliaAPI.MissionDesigner.csproj", "DragaliaAPI/DragaliaAPI.MissionDesigner/"]
COPY ["DragaliaAPI/DragaliaAPI.MasterAssetConverter/DragaliaAPI.MasterAssetConverter.csproj", "DragaliaAPI/DragaliaAPI.MasterAssetConverter/"]
COPY ["DragaliaAPI/Directory.Build.props", "."]
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
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "DragaliaAPI.dll"]

ARG GIT_COMMIT_SHA
ARG AUTHOR

LABEL io.dawnshard.commit-sha=${GIT_COMMIT_SHA}
LABEL org.opencontainers.image.authors=${AUTHOR}