# Overview
ProxyAPI count requests, limit amount of over all amount of requests send by user to service. Sadly, its also make a delay about 50-500ms, can be > 1s on cold start.  

# Installation
1. ```git clone https://github.com/ImfRepository/MapsProxy.git```
2. `cd MapsProxy`
3. ```docker-compose up```
4. change url in .env file to ```REACT_APP_BASE_URL=https://localhost:5000```
  
# Endpoints
- localhost:5011 - (UI) Service requests statistics and reset button
- localhost:5003/api/limits - (API) Get all limits
- localhost:5003/api/limits/{service} - (API) Change limit for service
- localhost:5003/api/limits/resetall - (API) Clear used requests stat 
- localhost:5000... - (API) Proxy
- localhost:5000/api/stats/available/{service} - (API) Collecting unused requests for {service} from app

# SSL SERT
- App uses sertificate from ```https/aspnetapp.pfx```.
