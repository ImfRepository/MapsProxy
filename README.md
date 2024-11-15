# Overview
ProxyAPI count requests, limit amount of over all amount of requests send by user to service. Sadly, its also make a delay about 50-500ms, can be > 1s on cold start.  

# Installation
1. ```git clone https://github.com/ImfRepository/MapsProxy.git```
2. `cd MapsProxy`
3. ```docker-compose up```
4. change urls in client's file ```src\ArcGIS-Components\MapLayers.jsx```
    1. old one ```https://portaltest.gismap.by/arcservertest/rest/services/...```
    2. new one ```https://localhost:5001/api/Proxy/testservices/...```
  
# Endpoints
- localhost:5011 - (UI) Service requests statistics and reset button
- localhost:5001/api/Proxy/testservices/{service}/... - (API) Proxy
- localhost:5001/api/stats/{service} - (API) Collecting unused requests for {service} from app

# SSL SERT
- App uses sertificate from ```https/aspnetapp.pfx```.
