# Overview
ProxyAPI count requests, limit amount of over all amount of requests send by user to service. Sadly, its also make a delay about 50-500ms, can be > 1s on cold start.  

# Installation
1. ```git clone https://github.com/ImfRepository/MapsProxy.git```
2. `cd MapsProxy`
3. add default.env
4. ```docker-compose up```

default.env file expects following format:
```
# ASP.Net Proxy API
POSTGRES_CONNECTION_STRING: Host=postgres-server;Port=5432;Database=postgres;Username=<USER>;Password=<PASSWORD>
REDIS_CONNECTION_STRING: redis-server:6379
MAPS_URL: https://<HOST>/arcservertest/rest/services/

# Redis
REDIS_PASSWORD=<PASSWORD>
REDIS_USER=<USER>
REDIS_USER_PASSWORD=<PASSWORD>

# Redis Commander
REDIS_HOSTS=redis-server:6379

# PostgreSQL
POSTGRES_USER: <USER>
POSTGRES_PASSWORD: <PASSWORD>

# pgAdmin
PGADMIN_DEFAULT_EMAIL: <EMAIL>
PGADMIN_DEFAULT_PASSWORD: <PASSWORD>
PGADMIN_LISTEN_PORT: <PORT>
```
