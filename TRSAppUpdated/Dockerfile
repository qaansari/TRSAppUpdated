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

# Copy all source files from the build context
COPY . .

# Publish the application (automatically handles root or subfolder contexts)
RUN if [ -f "TRSAppUpdated/TRSAppUpdated.csproj" ]; then \
        dotnet publish "TRSAppUpdated/TRSAppUpdated.csproj" -c Release -o /app/publish /p:UseAppHost=false; \
    elif [ -f "TRSAppUpdated.csproj" ]; then \
        dotnet publish "TRSAppUpdated.csproj" -c Release -o /app/publish /p:UseAppHost=false; \
    else \
        dotnet publish -c Release -o /app/publish /p:UseAppHost=false; \
    fi

# -------------------------------------------------------------
# Final production image
# -------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TRSAppUpdated.dll"]

