# Inflation Archive / Grocery web scraper

.NET Core continuation of https://github.com/Ioan01/productScraperBackend

A very basic frontend showcasing the basic features is available [here](https://ioan01.github.io/InflationArchive/#/)

## Features

- Daily tracking of product prices
- Product querying by name, category, manufacturer and pagination
- Viewing product price history
- Authorization using JWTs
- Adding products to your favorites 

## Tech
- .NET Core and Entity Framework Core with PostgreSQL for the backend
- Vue 2 and Vuetify for the frontend

## Appsettings json
##
```
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "userContext": "Host=host;Database=<db>;Username=<user>;Password=<password>",
        "scraperContext": "Host=host;Database=<db>;Username=<user>;Password=<password>"
    },
    "JWT": {
        "Issuer": <issuer>,
        "Audience": <audience>,
        "IssuerSigningKey": <key>
    },

}
```
