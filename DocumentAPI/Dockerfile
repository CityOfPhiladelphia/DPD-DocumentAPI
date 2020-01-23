
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

ENV TZ=America/New_York
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM microsoft/dotnet:2.2-sdk AS build
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

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "DocumentAPI.dll"]
