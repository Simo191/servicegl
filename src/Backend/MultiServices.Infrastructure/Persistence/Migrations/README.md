# Database Migrations

## Génération des migrations

```bash
cd src/Backend
dotnet ef migrations add InitialCreate \
  --project MultiServices.Infrastructure \
  --startup-project MultiServices.API \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project MultiServices.Infrastructure \
  --startup-project MultiServices.API
```
