# =========================
# Build Blazor WebAssembly
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
COPY ./GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj

COPY ./GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
COPY ./GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj -c Release -o out

# =========================
# Serveur Nginx pour le Front
# =========================
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

# Nettoyage et copie des fichiers statiques générés par Blazor
RUN rm -rf ./*
COPY --from=build /app/out/wwwroot .

# Copie de la configuration Nginx spécifique pour écouter sur le port 5000
COPY ./GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5000

CMD ["nginx", "-g", "daemon off;"]