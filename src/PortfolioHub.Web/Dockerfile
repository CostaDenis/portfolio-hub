FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/PortfolioHub.Web/ ./src/PortfolioHub.Web/
RUN dotnet publish src/PortfolioHub.Web/PortfolioHub.Web.csproj -c Release -o /app/publish

FROM cgr.dev/chainguard/nginx:latest

COPY src/PortfolioHub.Web/nginx.conf /etc/nginx/conf.d/nginx.default.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
