.PHONY: help setup dev up down build clean migrate seed test

help: ## Afficher l'aide
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

setup: ## Installation initiale
	cp .env.example .env
	cd src/Web/admin-panel && npm install
	cd src/Web/partner-panel && npm install
	cd src/Backend && dotnet restore
	@echo "✅ Setup terminé. Éditez .env avec vos clés."

dev-api: ## Lancer l'API en dev
	cd src/Backend/MultiServices.API && dotnet watch run

dev-admin: ## Lancer Admin Panel en dev
	cd src/Web/admin-panel && ng serve --port 4200

dev-partner: ## Lancer Partner Panel en dev
	cd src/Web/partner-panel && ng serve --port 4300

up: ## Démarrer tous les services Docker
	docker-compose up -d

down: ## Arrêter tous les services
	docker-compose down

build: ## Build Docker images
	docker-compose build

clean: ## Nettoyer volumes Docker
	docker-compose down -v --remove-orphans

migrate: ## Appliquer migrations EF Core
	cd src/Backend && dotnet ef database update --project MultiServices.Infrastructure --startup-project MultiServices.API

seed: ## Seed base de données
	cd src/Backend/MultiServices.API && dotnet run -- --seed

test: ## Lancer les tests
	cd src/Backend && dotnet test

logs: ## Voir les logs
	docker-compose logs -f

status: ## Status des services
	docker-compose ps
