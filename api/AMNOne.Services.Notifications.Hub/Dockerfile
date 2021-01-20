#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Hub/AMNOne.Services.Notifications.Hub/AMNOne.Services.Notifications.Hub.csproj", "src/Hub/AMNOne.Services.Notifications.Hub/"]
COPY ["src/Infrastructure/AMNOne.Services.Notifications.Infrastructure/AMNOne.Services.Notifications.Infrastructure.csproj", "src/Infrastructure/AMNOne.Services.Templates.Infrastructure/"]
COPY ["src/Core/AMNOne.Services.Notifications.Application/AMNOne.Services.Notifications.Application.csproj", "src/Core/AMNOne.Services.Notifications.Application/"]
COPY ["src/Core/AMNOne.Services.Notifications.Domain/AMNOne.Services.Notifications.Domain.csproj", "src/Core/AMNOne.Services.Notifications.Domain/"]
RUN dotnet restore "src/Hub/AMNOne.Services.Notifications.Hub/AMNOne.Services.Notifications.Hub.csproj"
COPY . .
WORKDIR "/src/src/Hub/AMNOne.Services.Notifications.Hub"
RUN dotnet build "AMNOne.Services.Notifications.Hub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AMNOne.Services.Notifications.Hub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AMNOne.Services.Notifications.Hub.dll"]