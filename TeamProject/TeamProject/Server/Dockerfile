# docker build -t nazwa:tag -f Server/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ["Client/TeamProject.Client.csproj", "Client/"]
COPY ["Shared/TeamProject.Shared.csproj", "Shared/"]
COPY ["Server/TeamProject.Server.csproj", "Server/"]

RUN dotnet restore "Server/TeamProject.Server.csproj"
COPY . .
WORKDIR /app/Server
RUN dotnet build "TeamProject.Server.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "TeamProject.Server.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamProject.Server.dll"]