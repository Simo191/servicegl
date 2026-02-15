# 🚀 MultiServices Platform

**Plateforme multi-services complète** — Restaurants 🍔, Services à Domicile 🛠️, Courses en Ligne 🛒

> Architecture moderne, scalable, prête pour la production. **~550+ fonctionnalités**.

---

## 📋 Table des Matières

- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Structure du Projet](#-structure-du-projet)
- [Installation](#-installation)
- [Démarrage Rapide](#-démarrage-rapide)
- [Modules](#-modules)
- [API Endpoints](#-api-endpoints)
- [Comptes de Démo](#-comptes-de-démo)
- [Déploiement](#-déploiement)

---

## 🏗️ Architecture

```
┌─────────────┐  ┌──────────────┐  ┌────────────────┐
│  App Mobile  │  │ Admin Panel  │  │ Partner Panel  │
│  .NET MAUI   │  │  Angular 21  │  │  Angular 21    │
└──────┬───────┘  └──────┬───────┘  └───────┬────────┘
       │                 │                   │
       └────────────┬────┴───────────────────┘
                    │
              ┌─────▼──────┐
              │   Nginx     │
              │  (Reverse   │
              │   Proxy)    │
              └─────┬───────┘
                    │
              ┌─────▼──────┐
              │  .NET 9 API │ ◄── Clean Architecture / DDD
              │  + SignalR  │
              └──┬──────┬──┘
                 │      │
          ┌──────▼┐  ┌──▼─────┐
          │Postgre│  │ Redis  │
          │  SQL  │  │ Cache  │
          └───────┘  └────────┘
```

**Patterns :** Clean Architecture, DDD, CQRS, Repository, Unit of Work, Mediator

---

## 🛠️ Technologies

| Composant | Technologie |
|-----------|-------------|
| **Backend API** | .NET 9, ASP.NET Core, Entity Framework Core, SignalR |
| **Base de données** | PostgreSQL 16, Redis 7 |
| **Admin Panel** | Angular 21, TypeScript, Tailwind CSS |
| **Partner Panel** | Angular 21, TypeScript, Tailwind CSS |
| **App Mobile** | .NET MAUI (iOS, Android), CommunityToolkit.Mvvm |
| **Paiement** | Stripe API |
| **Auth** | ASP.NET Identity, JWT, OAuth 2.0 |
| **Temps réel** | SignalR WebSockets |
| **Conteneurs** | Docker, Docker Compose |
| **Reverse Proxy** | Nginx |
| **Logs** | Seq (Serilog) |

---

## 📁 Structure du Projet

```
MultiServicesApp/
├── src/
│   ├── Backend/                              # API .NET 9
│   │   ├── MultiServices.Domain/             # Entités, Value Objects, Interfaces
│   │   │   ├── Entities/                     # Restaurant, Service, Grocery, Identity...
│   │   │   ├── Enums/                        # OrderStatus, PaymentMethod, UserRole...
│   │   │   ├── ValueObjects/                 # Money, Address, GeoLocation...
│   │   │   └── Interfaces/                   # IRepository, IUnitOfWork, IDomainServices
│   │   ├── MultiServices.Application/        # CQRS Commands/Queries, DTOs
│   │   │   ├── Features/                     # Restaurants, Services, Grocery, Delivery, Identity
│   │   │   └── Interfaces/                   # Application service interfaces
│   │   ├── MultiServices.Infrastructure/     # EF Core, Auth, Payments, Notifications
│   │   │   ├── Data/                         # DbContext, Configurations, Migrations
│   │   │   ├── Repositories/                 # GenericRepository, UnitOfWork
│   │   │   └── Services/                     # Auth, Payment, Storage, Notification
│   │   └── MultiServices.API/               # Controllers, Middleware, Hubs
│   │       ├── Controllers/                  # REST API Controllers
│   │       └── Hubs/                         # SignalR Hubs (tracking temps réel)
│   ├── Web/
│   │   ├── admin-panel/                      # Angular 21 - Panel Administrateur
│   │   │   ├── src/app/                      # Components, Services, Guards
│   │   │   └── Dockerfile                    # Build multi-stage
│   │   └── partner-panel/                    # Angular 21 - Panel Partenaire
│   │       ├── src/app/                      # Restaurant/Service/Store management
│   │       └── Dockerfile
│   └── Mobile/
│       └── MultiServices.Maui/              # .NET MAUI Mobile App
│           ├── Models/                       # DTOs, Cart models
│           ├── ViewModels/                   # MVVM ViewModels
│           ├── Views/                        # XAML Pages
│           ├── Services/                     # API, Auth, Location, Notification
│           ├── Converters/                   # Value Converters
│           └── Platforms/                    # Android, iOS configs
├── scripts/
│   ├── init-db.sql                           # Seed data (users, restaurants, produits...)
│   └── deploy.sh                             # Script de déploiement
├── nginx/
│   └── nginx.conf                            # Reverse proxy configuration
├── docker-compose.yml                        # Orchestration services
├── Makefile                                  # Commandes raccourcies
├── .env.example                              # Variables d'environnement
└── .gitignore
```

---

## ⚡ Installation

### Prérequis

- **Docker** & **Docker Compose** (recommandé)
- Ou pour le développement local :
  - .NET 9 SDK
  - Node.js 20+ & npm
  - PostgreSQL 16
  - Redis 7

### Installation Docker (Recommandé)

```bash
# 1. Cloner le projet
git clone <repo-url> MultiServicesApp
cd MultiServicesApp

# 2. Configurer l'environnement
cp .env.example .env
# Éditez .env avec vos clés (Stripe, etc.)

# 3. Démarrer
./scripts/deploy.sh up
```

### Installation Locale (Développement)

```bash
# 1. Setup
make setup

# 2. Démarrer l'API
make dev-api

# 3. Démarrer l'Admin Panel (nouveau terminal)
make dev-admin

# 4. Démarrer le Partner Panel (nouveau terminal)
make dev-partner
```

---

## 🚀 Démarrage Rapide

```bash
# Démarrer tous les services
./scripts/deploy.sh up

# Voir les logs
./scripts/deploy.sh logs

# Arrêter
./scripts/deploy.sh down

# Backup base de données
./scripts/deploy.sh backup

# Status des services
./scripts/deploy.sh status
```

**URLs après démarrage :**

| Service | URL |
|---------|-----|
| API (Swagger) | http://localhost:5000/swagger |
| Admin Panel | http://localhost:4200 |
| Partner Panel | http://localhost:4300 |
| Seq (Logs) | http://localhost:5341 |

---

## 📦 Modules

### 🍔 Module Restaurants (~150 fonctionnalités)

**Client :** Recherche restaurants, filtres (cuisine, prix, note, distance), menu complet, personnalisation plats, panier, code promo, pourboire, livraison immédiate/programmée, suivi GPS temps réel, chat livreur.

**Restaurant :** Gestion menu (catégories, plats, tailles, extras), gestion commandes temps réel, tableau de bord, statistiques, promotions, horaires d'ouverture.

### 🛠️ Module Services à Domicile (~170 fonctionnalités)

**Client :** 8 catégories (Plomberie, Électricité, Ménage, Peinture, Jardinage, Climatisation, Déménagement, Réparation), profil prestataire, portfolio, réservation multi-étapes, photos problème, choix créneaux, suivi intervention, avis.

**Prestataire :** Inscription KYC, gestion services/tarifs, calendrier disponibilités, gestion équipe, devis, suivi intervention (photos avant/après), finances, statistiques.

### 🛒 Module Courses en Ligne (~160 fonctionnalités)

**Client :** 5 enseignes (Marjane, Carrefour, Aswak Assalam, Acima, Label'Vie), parcours rayons, scanner code-barre, filtres (Bio, Halal, Promo), listes de courses partagées, produits de remplacement, créneaux livraison 2h.

**Magasin :** Import produits CSV/Excel, gestion stock temps réel, alertes stock bas, picking/préparation, promotions, statistiques.

### Modules Transversaux (~230 fonctionnalités)

**Auth :** Email, Google, Facebook, Apple, biométrie, SMS OTP.
**Paiement :** CB, Apple/Google Pay, PayPal, espèces, portefeuille virtuel, 3D Secure.
**Fidélité :** Points, tiers (Bronze/Silver/Gold/Platinum), parrainage.
**Admin :** Dashboard global, gestion utilisateurs/prestataires/livreurs, commandes tous types, finances, marketing, configuration.
**Livreur :** Multi-courses, GPS, gains, bouton SOS.

---

## 🔗 API Endpoints

### Authentication
```
POST   /api/auth/register          Inscription
POST   /api/auth/login             Connexion
POST   /api/auth/refresh           Rafraîchir token
POST   /api/auth/forgot-password   Mot de passe oublié
POST   /api/auth/social-login      Connexion sociale
```

### Restaurants
```
GET    /api/restaurants             Liste restaurants
GET    /api/restaurants/{id}        Détail restaurant
GET    /api/restaurants/{id}/menu   Menu complet
POST   /api/restaurants/orders      Créer commande
GET    /api/restaurants/orders/{id} Détail commande
PUT    /api/restaurants/orders/{id}/status  Changer statut
```

### Services
```
GET    /api/services/providers      Liste prestataires
GET    /api/services/providers/{id} Détail prestataire
GET    /api/services/providers/{id}/slots  Créneaux disponibles
POST   /api/services/interventions  Réserver intervention
GET    /api/services/interventions/{id}  Détail intervention
```

### Grocery
```
GET    /api/grocery/stores          Liste magasins
GET    /api/grocery/stores/{id}     Détail magasin
GET    /api/grocery/stores/{id}/products  Produits
POST   /api/grocery/orders          Créer commande
GET    /api/grocery/shopping-lists   Listes de courses
```

### Profil & Commun
```
GET    /api/profile                 Mon profil
PUT    /api/profile                 Modifier profil
GET    /api/profile/addresses       Mes adresses
GET    /api/profile/orders          Mes commandes (tous types)
GET    /api/wallet                  Mon portefeuille
GET    /api/notifications           Mes notifications
```

---

## 🔑 Comptes de Démo

| Rôle | Email | Mot de passe |
|------|-------|-------------|
| Super Admin | admin@multiservices.ma | Password@2025! |
| Client | amine@demo.ma | Client@2025! |
| Restaurant | karim@demo.ma | Client@2025! |
| Prestataire | omar@demo.ma | Client@2025! |
| Magasin | fatima@demo.ma | Client@2025! |

---

## 🌐 Déploiement Production

### Docker Compose

```bash
./scripts/deploy.sh up
```

### Commandes utiles

```bash
make help       # Toutes les commandes
make migrate    # Appliquer migrations
make seed       # Seed base de données
make test       # Lancer les tests
make logs       # Voir les logs
make clean      # Nettoyer tout
```

---

## 📊 Statistiques du Projet

| Métrique | Valeur |
|----------|--------|
| Fonctionnalités totales | ~550+ |
| Fichiers Backend (.cs) | ~135 |
| Fichiers Web (.ts/.html) | ~55 |
| Fichiers Mobile (.cs/.xaml) | ~85 |
| Tables base de données | ~45 |
| API Endpoints | ~60+ |
| Modules | 3 + transversaux |

---

## 📄 Licence

Projet propriétaire. Tous droits réservés.

---

**Développé avec ❤️ pour le marché marocain 🇲🇦**
