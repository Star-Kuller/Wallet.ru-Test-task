# Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Копирование проектных файлов
COPY ["TestTask.Client/TestTask.Client.csproj", "TestTask.Client/"]
RUN dotnet restore "TestTask.Client/TestTask.Client.csproj"

# Копирование остальных файлов
COPY . .

# Публикация проекта
WORKDIR "/app/TestTask.Client"
RUN dotnet publish -c Release -o /app/output

# Финальный образ с Nginx
FROM nginx:alpine AS final
WORKDIR /var/www/web

# Копирование статических файлов из папки wwwroot
COPY --from=build-env /app/output/wwwroot .

# Копирование конфигурации Nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Открытие порта
EXPOSE 80

# Запуск Nginx
CMD ["nginx", "-g", "daemon off;"]