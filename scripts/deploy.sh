#!/bin/bash
set -e

# =====================================================
# MultiServices Platform - Deployment Script
# =====================================================

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}🚀 MultiServices Platform - Déploiement${NC}"
echo "============================================="

# Check prerequisites
command -v docker >/dev/null 2>&1 || { echo -e "${RED}❌ Docker requis.${NC}"; exit 1; }
command -v docker-compose >/dev/null 2>&1 || COMPOSE_CMD="docker compose" || COMPOSE_CMD="docker-compose"
COMPOSE_CMD=${COMPOSE_CMD:-"docker-compose"}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_DIR"

# Environment setup
if [ ! -f ".env" ]; then
    echo -e "${YELLOW}📋 Création du fichier .env depuis .env.example...${NC}"
    cp .env.example .env
    echo -e "${YELLOW}⚠️  Éditez .env avec vos clés avant de relancer.${NC}"
    exit 0
fi

# Parse command
ACTION=${1:-"up"}

case $ACTION in
    up|start)
        echo -e "${GREEN}▶ Démarrage des services...${NC}"
        $COMPOSE_CMD up -d --build
        echo ""
        echo -e "${GREEN}✅ Services démarrés !${NC}"
        echo ""
        echo "📍 URLs :"
        echo "   API       : http://localhost:5000"
        echo "   Admin     : http://localhost:4200"
        echo "   Partner   : http://localhost:4300"
        echo "   Swagger   : http://localhost:5000/swagger"
        echo "   Seq Logs  : http://localhost:5341"
        echo ""
        echo "🔑 Comptes de démo :"
        echo "   Admin    : admin@multiservices.ma / Admin@2025!"
        echo "   Client   : amine@demo.ma / Client@2025!"
        echo "   Restaurant: karim@demo.ma / Client@2025!"
        echo "   Service  : omar@demo.ma / Client@2025!"
        echo "   Magasin  : fatima@demo.ma / Client@2025!"
        ;;
    down|stop)
        echo -e "${YELLOW}⏹ Arrêt des services...${NC}"
        $COMPOSE_CMD down
        echo -e "${GREEN}✅ Services arrêtés.${NC}"
        ;;
    restart)
        echo -e "${YELLOW}🔄 Redémarrage...${NC}"
        $COMPOSE_CMD down
        $COMPOSE_CMD up -d --build
        echo -e "${GREEN}✅ Services redémarrés.${NC}"
        ;;
    logs)
        $COMPOSE_CMD logs -f ${2:-""}
        ;;
    status)
        $COMPOSE_CMD ps
        ;;
    clean)
        echo -e "${RED}🗑 Nettoyage complet (données incluses)...${NC}"
        read -p "Confirmer ? (y/N) " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            $COMPOSE_CMD down -v --remove-orphans
            echo -e "${GREEN}✅ Nettoyé.${NC}"
        fi
        ;;
    migrate)
        echo -e "${BLUE}📦 Migration de la base de données...${NC}"
        docker exec multiservices-api dotnet ef database update --project /app
        echo -e "${GREEN}✅ Migration appliquée.${NC}"
        ;;
    backup)
        BACKUP_FILE="backup_$(date +%Y%m%d_%H%M%S).sql"
        echo -e "${BLUE}💾 Backup -> $BACKUP_FILE${NC}"
        docker exec multiservices-db pg_dump -U msadmin multiservices > "./scripts/$BACKUP_FILE"
        echo -e "${GREEN}✅ Backup créé : scripts/$BACKUP_FILE${NC}"
        ;;
    restore)
        if [ -z "$2" ]; then echo -e "${RED}Usage: ./deploy.sh restore <fichier.sql>${NC}"; exit 1; fi
        echo -e "${BLUE}📥 Restauration depuis $2...${NC}"
        docker exec -i multiservices-db psql -U msadmin multiservices < "$2"
        echo -e "${GREEN}✅ Restauré.${NC}"
        ;;
    *)
        echo "Usage: ./deploy.sh [up|down|restart|logs|status|clean|migrate|backup|restore]"
        echo ""
        echo "  up       Démarrer tous les services"
        echo "  down     Arrêter tous les services"
        echo "  restart  Redémarrer tous les services"
        echo "  logs     Voir les logs (optionnel: nom du service)"
        echo "  status   Voir le statut des services"
        echo "  clean    Supprimer tout (volumes inclus)"
        echo "  migrate  Appliquer les migrations EF Core"
        echo "  backup   Sauvegarder la base de données"
        echo "  restore  Restaurer la base de données"
        ;;
esac
