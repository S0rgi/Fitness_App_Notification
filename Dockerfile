# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем проект и восстанавливаем зависимости
COPY Fitness_App_Notification/*.csproj ./Fitness_App_Notification/
WORKDIR /src/Fitness_App_Notification
RUN dotnet restore

# Копируем все файлы и публикуем
COPY . .
RUN dotnet publish -c Release -o /out

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Рабочая директория
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=build /out ./

# Открываем порт 8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Fitness_App_Notification.dll"]
