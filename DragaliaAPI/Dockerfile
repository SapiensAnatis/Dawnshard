#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DragaliaAPI/DragaliaAPI.csproj", "DragaliaAPI/"]
RUN dotnet restore "DragaliaAPI/DragaliaAPI.csproj"
COPY . .
WORKDIR "/src/DragaliaAPI"
RUN dotnet build "DragaliaAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DragaliaAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false
WORKDIR "/src/DragaliaAPI"
RUN dotnet tool restore
RUN dotnet ef migrations script -o /app/publish/migrate-script.sql
RUN dotnet ef migrations bundle -o /app/publish/migrate-bundle --self-contained -r linux-x64 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DragaliaAPI.dll"]