FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Fitness_App_Notification/*.csproj ./Fitness_App_Notification/
WORKDIR /src/Fitness_App_Notification
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /out

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Устанавливаем RabbitMQ и supervisord
RUN apt-get update && apt-get install -y netcat-openbsd

RUN apt-get update && \
    apt-get install -y \
    rabbitmq-server \
    supervisor

# Рабочая директория
WORKDIR /app

# Копируем билд .NET-приложения
COPY --from=build /out /app

# Копируем конфиг supervisord
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 8080 5672 15672

CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]
