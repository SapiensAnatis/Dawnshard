ARG BASE_DOTNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:8.0

FROM $BASE_DOTNET_IMAGE AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG CI

WORKDIR /src
COPY ["DragaliaAPI/DragaliaAPI/DragaliaAPI.csproj", "DragaliaAPI/DragaliaAPI/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
RUN dotnet restore "DragaliaAPI/DragaliaAPI/DragaliaAPI.csproj"
COPY ["DragaliaAPI/", "DragaliaAPI/"]
COPY ["Shared/", "Shared/"]
WORKDIR "/src/DragaliaAPI/DragaliaAPI"
RUN dotnet publish "DragaliaAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DragaliaAPI.dll"]