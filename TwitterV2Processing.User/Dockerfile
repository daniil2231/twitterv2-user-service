FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TwitterV2Processing.User/TwitterV2Processing.User.csproj", "TwitterV2Processing.User/"]
RUN dotnet restore "./TwitterV2Processing.User/TwitterV2Processing.User.csproj"
COPY . .
WORKDIR "/src/TwitterV2Processing.User"
RUN dotnet build "./TwitterV2Processing.User.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TwitterV2Processing.User.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitterV2Processing.User.dll"]