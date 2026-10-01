# -------------------------------------------------------------
# Base runtime image with Linux native dependencies for OCR & Fonts
# -------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Install native packages required by Tesseract OCR and QuestPDF (SkiaSharp / fonts)
RUN apt-get update && apt-get install -y --no-install-recommends \
    tesseract-ocr \
    libtesseract-dev \
    liblept5 \
    libfontconfig1 \
    fonts-freefont-ttf \
    fonts-dejavu-core \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:${PORT}
ENV TESSDATA_PREFIX=/app/wwwroot/assets/tessdata

# -------------------------------------------------------------
# Build stage using .NET 10 SDK
# -------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first (for optimal Docker layer caching)
COPY ["TRSAppUpdated/TRSAppUpdated.csproj", "TRSAppUpdated/"]
RUN dotnet restore "TRSAppUpdated/TRSAppUpdated.csproj"

# Copy the rest of the application files
COPY . .
WORKDIR "/src/TRSAppUpdated"
RUN dotnet build "TRSAppUpdated.csproj" -c Release -o /app/build

# -------------------------------------------------------------
# Publish stage
# -------------------------------------------------------------
FROM build AS publish
RUN dotnet publish "TRSAppUpdated.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------------------------------------------------------------
# Final production image
# -------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TRSAppUpdated.dll"]

