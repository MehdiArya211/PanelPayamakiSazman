FROM mcr.microsoft.com/dotnet/aspnet:7.0

ARG APP_VERSION
ARG BUILD_DATE

ENV ASPNETCORE_URLS=http://0.0.0.0:9090 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0 \
    APP_VERSION=${APP_VERSION} 

WORKDIR /app
COPY . .

EXPOSE 80

LABEL org.opencontainers.image.title="samp-core-back-end-api" \
    org.opencontainers.image.version="${APP_VERSION}" \
    org.opencontainers.image.created="${BUILD_DATE}"

ENTRYPOINT ["dotnet", "PanelSMS.dll"]
