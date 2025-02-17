FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ["TestTask.Client.csproj", "./"]
RUN dotnet restore "TestTask.Client.csproj"

COPY . .
COPY ./wwwroot/appsettings.docker.json ./wwwroot/appsettings.json

WORKDIR "/app"
RUN dotnet publish -c Release -o /app/output

FROM nginx:alpine AS final
WORKDIR /var/www/web

COPY --from=build-env /app/output/wwwroot .

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]