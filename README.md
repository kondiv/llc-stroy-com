# Система для контроля работы на строительных объектах
## Для запуска (dev)
ВАЖНО: Все данные для токенов, строки подключения хранятся в .env файле, недоступном в публичном репозитории
1. `docker-compose up -d` -- запуск базы данных в докер контейнере
2. `dotnet ef migration add Init --startup-project LLCStroyCom.Api --project LLCStroyCom.Infrastructure` -- создание миграции по актуальному контексту
3. `dotnet ef database update --startup-project LLCStroyCom.Api --project LLCStroyCom.Infrastructure` -- обновление базы данных
4. Запустить проект **LLCStroyCom.Api** (http/https)
