# 1. Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

# Install native graphics libraries and Unicode fonts for QuestPDF and ImageSharp
USER root
RUN apt-get update && apt-get install -y --no-install-recommends \
    libfontconfig1 \
    libfreetype6 \
    fonts-dejavu-core \
    fonts-liberation \
    && rm -rf /var/lib/apt/lists/*

# 2. Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["DbQLBH_65132292.csproj", "./"]
RUN dotnet restore "DbQLBH_65132292.csproj"
COPY . .
RUN dotnet build "DbQLBH_65132292.csproj" -c Release -o /app/build

# 3. Publish stage
FROM build AS publish
RUN dotnet publish "DbQLBH_65132292.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN mkdir -p /app/wwwroot/images/sanpham && chown -R app:app /app
USER app
ENTRYPOINT ["dotnet", "DbQLBH_65132292.dll"]
