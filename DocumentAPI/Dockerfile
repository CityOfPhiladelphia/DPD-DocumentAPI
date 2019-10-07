FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DocumentAPI/DocumentAPI.csproj", "DocumentAPI/"]
COPY ["DocumentAPI.Services/DocumentAPI.Services.csproj", "DocumentAPI.Services/"]
COPY ["DocumentAPI.Infrastructure/DocumentAPI.Infrastructure.csproj", "DocumentAPI.Infrastructure/"]
RUN dotnet restore "DocumentAPI/DocumentAPI.csproj"
COPY . .
WORKDIR "/src/DocumentAPI"
RUN dotnet build "DocumentAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DocumentAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DocumentAPI.dll"]