#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TeamProject/Server/TeamProject.Server.csproj", "TeamProject/Server/"]
COPY ["TeamProject/Client/TeamProject.Client.csproj", "TeamProject/Client/"]
COPY ["TeamProject/Shared/TeamProject.Shared.csproj", "TeamProject/Shared/"]
RUN dotnet restore "TeamProject/Server/TeamProject.Server.csproj"
COPY . .
WORKDIR "/src/TeamProject/Server"
RUN dotnet build "TeamProject.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamProject.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamProject.Server.dll"]