# Система для контроля работы на строительных объектах :construction:
## Для запуска Development
1. `docker-compose up -d database` -- запуск базы данных в докер контейнере
2. Запустить проект **LLCStroyCom.Api**
3. Доступ по следующим url:    
   > http://localhost:5068    

   > https://localhost:7272
## Для запуска Production
1. *Первый запуск* `docker-compose up --build` 
2. *Не первый запуск* `docker-compose up -d`
3. Доступ по следующим url:    
   > http://localhost:5000   

   > https://localhost:5001
