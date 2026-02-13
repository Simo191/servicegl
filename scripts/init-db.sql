-- =====================================================
-- MultiServices Platform - Database Initialization
-- =====================================================

-- Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- =====================================================
-- SEED DATA
-- =====================================================

-- Roles
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "Description", "ConcurrencyStamp")
VALUES
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'SuperAdmin', 'SUPERADMIN', 'Super Administrateur', uuid_generate_v4()::text),
    ('b2c3d4e5-f6a7-8901-bcde-f12345678901', 'Admin', 'ADMIN', 'Administrateur', uuid_generate_v4()::text),
    ('c3d4e5f6-a7b8-9012-cdef-123456789012', 'Client', 'CLIENT', 'Client', uuid_generate_v4()::text),
    ('d4e5f6a7-b8c9-0123-defa-234567890123', 'RestaurantOwner', 'RESTAURANTOWNER', 'Propriétaire de Restaurant', uuid_generate_v4()::text),
    ('e5f6a7b8-c9d0-1234-efab-345678901234', 'ServiceProvider', 'SERVICEPROVIDER', 'Prestataire de Services', uuid_generate_v4()::text),
    ('f6a7b8c9-d0e1-2345-fabc-456789012345', 'StoreManager', 'STOREMANAGER', 'Gestionnaire de Magasin', uuid_generate_v4()::text),
    ('a7b8c9d0-e1f2-3456-abcd-567890123456', 'Deliverer', 'DELIVERER', 'Livreur', uuid_generate_v4()::text)
ON CONFLICT DO NOTHING;

-- Admin User (password: Admin@2025!)
INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "Email", "NormalizedEmail", "UserName", "NormalizedUserName", "EmailConfirmed", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "IsActive", "PreferredLanguage", "CreatedAt")
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'Admin', 'System',
    'admin@multiservices.ma', 'ADMIN@MULTISERVICES.MA',
    'admin@multiservices.ma', 'ADMIN@MULTISERVICES.MA',
    true, false, false, true, 0,
    'AQAAAAIAAYagAAAAEKn7DHjPfKG5tnhYLxyE3H3OkE+zNJQ5UlBl6xjRCF5q9wN8CxX2ZT2F5h1N0k3Ug==',
    uuid_generate_v4()::text, uuid_generate_v4()::text,
    true, 'fr', NOW()
) ON CONFLICT DO NOTHING;

-- Assign SuperAdmin role
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES ('11111111-1111-1111-1111-111111111111', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890')
ON CONFLICT DO NOTHING;

-- Demo Client (password: Client@2025!)
INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "Email", "NormalizedEmail", "UserName", "NormalizedUserName", "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "IsActive", "PreferredLanguage", "ReferralCode", "CreatedAt")
VALUES (
    '22222222-2222-2222-2222-222222222222',
    'Amine', 'Benali',
    'amine@demo.ma', 'AMINE@DEMO.MA',
    'amine@demo.ma', 'AMINE@DEMO.MA',
    true, '+212600000001', true, false, true, 0,
    'AQAAAAIAAYagAAAAEKn7DHjPfKG5tnhYLxyE3H3OkE+zNJQ5UlBl6xjRCF5q9wN8CxX2ZT2F5h1N0k3Ug==',
    uuid_generate_v4()::text, uuid_generate_v4()::text,
    true, 'fr', 'AMINE2025', NOW()
) ON CONFLICT DO NOTHING;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES ('22222222-2222-2222-2222-222222222222', 'c3d4e5f6-a7b8-9012-cdef-123456789012')
ON CONFLICT DO NOTHING;

-- Client Addresses
INSERT INTO "UserAddresses" ("Id", "UserId", "Label", "Street", "City", "PostalCode", "Country", "Latitude", "Longitude", "IsDefault", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), '22222222-2222-2222-2222-222222222222', 'Maison', '15 Rue Mohammed V, Quartier Hassan', 'Casablanca', '20000', 'Maroc', 33.5731, -7.5898, true, false, NOW()),
    (uuid_generate_v4(), '22222222-2222-2222-2222-222222222222', 'Travail', '45 Boulevard Zerktouni, Maârif', 'Casablanca', '20100', 'Maroc', 33.5886, -7.6266, false, false, NOW())
ON CONFLICT DO NOTHING;

-- Client Wallet
INSERT INTO "Wallets" ("Id", "UserId", "Balance", "Currency", "IsActive", "IsDeleted", "CreatedAt")
VALUES (uuid_generate_v4(), '22222222-2222-2222-2222-222222222222', 150.00, 'MAD', true, false, NOW())
ON CONFLICT DO NOTHING;

-- Client Loyalty
INSERT INTO "LoyaltyAccounts" ("Id", "UserId", "TotalPoints", "AvailablePoints", "Tier", "IsDeleted", "CreatedAt")
VALUES (uuid_generate_v4(), '22222222-2222-2222-2222-222222222222', 1250, 850, 'Silver', false, NOW())
ON CONFLICT DO NOTHING;

-- Demo Restaurant Owner
INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "Email", "NormalizedEmail", "UserName", "NormalizedUserName", "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "IsActive", "PreferredLanguage", "CreatedAt")
VALUES (
    '33333333-3333-3333-3333-333333333333',
    'Karim', 'El Fassi',
    'karim@demo.ma', 'KARIM@DEMO.MA',
    'karim@demo.ma', 'KARIM@DEMO.MA',
    true, '+212600000002', true, false, true, 0,
    'AQAAAAIAAYagAAAAEKn7DHjPfKG5tnhYLxyE3H3OkE+zNJQ5UlBl6xjRCF5q9wN8CxX2ZT2F5h1N0k3Ug==',
    uuid_generate_v4()::text, uuid_generate_v4()::text,
    true, 'fr', NOW()
) ON CONFLICT DO NOTHING;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES ('33333333-3333-3333-3333-333333333333', 'd4e5f6a7-b8c9-0123-defa-234567890123')
ON CONFLICT DO NOTHING;

-- Demo Restaurants
INSERT INTO "Restaurants" ("Id", "OwnerId", "Name", "Slug", "Description", "CuisineType", "PriceRange", "Phone", "Street", "City", "Latitude", "Longitude", "AverageRating", "TotalReviews", "MinOrderAmount", "DeliveryFee", "FreeDeliveryThreshold", "EstimatedDeliveryMinutes", "CommissionRate", "IsActive", "IsVerified", "IsFeatured", "AcceptsOnlinePayment", "AcceptsCashPayment", "IsDeleted", "CreatedAt")
VALUES
    ('aaaa1111-aaaa-1111-aaaa-111111111111', '33333333-3333-3333-3333-333333333333', 'Chez Hassan', 'chez-hassan', 'Restaurant marocain traditionnel avec tagines, couscous et grillades', 'Marocain', 2, '+212522334455', '23 Rue de Fès, Habous', 'Casablanca', 33.5807, -7.6194, 4.5, 234, 50.00, 15.00, 150.00, 35, 15.00, true, true, true, true, true, false, NOW()),
    ('aaaa2222-aaaa-2222-aaaa-222222222222', '33333333-3333-3333-3333-333333333333', 'Pizza Bella', 'pizza-bella', 'Pizzeria italienne artisanale, pâte faite maison', 'Italien', 2, '+212522445566', '8 Rue Moulay Youssef, Centre Ville', 'Casablanca', 33.5950, -7.6187, 4.3, 189, 40.00, 10.00, 120.00, 30, 15.00, true, true, false, true, true, false, NOW()),
    ('aaaa3333-aaaa-3333-aaaa-333333333333', '33333333-3333-3333-3333-333333333333', 'Dragon d''Or', 'dragon-dor', 'Cuisine asiatique authentique: sushi, wok, noodles', 'Asiatique', 3, '+212522556677', '15 Boulevard Massira, Maârif', 'Casablanca', 33.5886, -7.6280, 4.7, 312, 60.00, 20.00, 200.00, 40, 15.00, true, true, true, true, true, false, NOW())
ON CONFLICT DO NOTHING;

-- Menu Categories
INSERT INTO "MenuCategories" ("Id", "RestaurantId", "Name", "Description", "SortOrder", "IsActive", "IsDeleted", "CreatedAt")
VALUES
    ('bb110000-0000-0000-0000-000000000001', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Entrées', 'Salades et entrées chaudes', 1, true, false, NOW()),
    ('bb110000-0000-0000-0000-000000000002', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Tagines', 'Nos tagines traditionnels', 2, true, false, NOW()),
    ('bb110000-0000-0000-0000-000000000003', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Couscous', 'Couscous du vendredi et plus', 3, true, false, NOW()),
    ('bb110000-0000-0000-0000-000000000004', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Grillades', 'Brochettes et viandes grillées', 4, true, false, NOW()),
    ('bb110000-0000-0000-0000-000000000005', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Desserts', 'Pâtisseries marocaines', 5, true, false, NOW()),
    ('bb110000-0000-0000-0000-000000000006', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Boissons', 'Thés, jus et boissons', 6, true, false, NOW())
ON CONFLICT DO NOTHING;

-- Menu Items - Chez Hassan
INSERT INTO "MenuItems" ("Id", "CategoryId", "RestaurantId", "Name", "Description", "Price", "IsAvailable", "IsPopular", "PreparationTime", "SortOrder", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000001', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Salade Marocaine', 'Tomates, oignons, poivrons, concombres, huile d''olive', 25.00, true, false, 5, 1, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000001', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Briouates', 'Briouates viande hachée et fromage (6 pièces)', 35.00, true, true, 10, 2, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000001', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Harira', 'Soupe traditionnelle aux lentilles et pois chiches', 20.00, true, true, 5, 3, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000002', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Tagine Poulet Citron', 'Poulet fermier, citrons confits, olives vertes', 65.00, true, true, 25, 1, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000002', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Tagine Kefta', 'Boulettes de viande hachée, oeufs, tomates', 55.00, true, true, 20, 2, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000002', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Tagine Agneau Pruneaux', 'Épaule d''agneau, pruneaux, amandes grillées', 85.00, true, false, 30, 3, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000003', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Couscous Royal', 'Semoule, légumes, poulet, agneau, merguez', 95.00, true, true, 30, 1, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000003', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Couscous Tfaya', 'Semoule, poulet, oignons caramélisés, raisins secs', 75.00, true, false, 25, 2, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000004', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Brochettes Mixtes', 'Poulet, kefta, agneau (3 brochettes)', 70.00, true, true, 15, 1, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000005', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Pastilla au Lait', 'Feuilles de brick, crème pâtissière, amandes', 40.00, true, true, 5, 1, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000005', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Cornes de Gazelle', 'Pâte d''amande et eau de fleur d''oranger (6 pièces)', 45.00, true, false, 5, 2, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000006', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Thé à la Menthe', 'Thé vert à la menthe fraîche', 15.00, true, true, 5, 1, false, NOW()),
    (uuid_generate_v4(), 'bb110000-0000-0000-0000-000000000006', 'aaaa1111-aaaa-1111-aaaa-111111111111', 'Jus d''Orange Frais', 'Oranges pressées du jour', 20.00, true, false, 5, 2, false, NOW())
ON CONFLICT DO NOTHING;

-- Demo Service Provider
INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "Email", "NormalizedEmail", "UserName", "NormalizedUserName", "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "IsActive", "PreferredLanguage", "CreatedAt")
VALUES (
    '44444444-4444-4444-4444-444444444444',
    'Omar', 'Tazi',
    'omar@demo.ma', 'OMAR@DEMO.MA',
    'omar@demo.ma', 'OMAR@DEMO.MA',
    true, '+212600000003', true, false, true, 0,
    'AQAAAAIAAYagAAAAEKn7DHjPfKG5tnhYLxyE3H3OkE+zNJQ5UlBl6xjRCF5q9wN8CxX2ZT2F5h1N0k3Ug==',
    uuid_generate_v4()::text, uuid_generate_v4()::text,
    true, 'fr', NOW()
) ON CONFLICT DO NOTHING;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES ('44444444-4444-4444-4444-444444444444', 'e5f6a7b8-c9d0-1234-efab-345678901234')
ON CONFLICT DO NOTHING;

-- Service Providers
INSERT INTO "ServiceProviders" ("Id", "OwnerId", "CompanyName", "Slug", "Description", "PrimaryCategory", "Phone", "City", "YearsOfExperience", "AverageRating", "TotalReviews", "TotalInterventions", "CommissionRate", "HasInsurance", "IsActive", "IsVerified", "IsFeatured", "IsDeleted", "CreatedAt")
VALUES
    ('cccc1111-cccc-1111-cccc-111111111111', '44444444-4444-4444-4444-444444444444', 'Plomberie Express Casa', 'plomberie-express-casa', 'Plombier professionnel avec 15 ans d''expérience. Urgences 24h/24.', 'Plomberie', '+212661223344', 'Casablanca', 15, 4.8, 523, 890, 20.00, true, true, true, true, false, NOW()),
    ('cccc2222-cccc-2222-cccc-222222222222', '44444444-4444-4444-4444-444444444444', 'Éclat Nettoyage', 'eclat-nettoyage', 'Service de ménage et nettoyage professionnel. Produits bio.', 'Ménage', '+212661334455', 'Casablanca', 8, 4.6, 387, 1240, 20.00, true, true, true, false, false, NOW())
ON CONFLICT DO NOTHING;

-- Service Offerings
INSERT INTO "ServiceOfferings" ("Id", "ProviderId", "Name", "Description", "Category", "PricingType", "HourlyRate", "FixedPrice", "TravelFee", "EstimatedDurationMinutes", "MaterialIncluded", "IsActive", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), 'cccc1111-cccc-1111-cccc-111111111111', 'Réparation Fuite', 'Diagnostic et réparation de fuites d''eau', 'Plomberie', 'Fixed', NULL, 200.00, 50.00, 60, false, true, false, NOW()),
    (uuid_generate_v4(), 'cccc1111-cccc-1111-cccc-111111111111', 'Débouchage Canalisation', 'Débouchage de canalisations bouchées', 'Plomberie', 'Fixed', NULL, 300.00, 50.00, 90, true, true, false, NOW()),
    (uuid_generate_v4(), 'cccc1111-cccc-1111-cccc-111111111111', 'Installation Sanitaire', 'Installation de WC, lavabo, baignoire, douche', 'Plomberie', 'Hourly', 150.00, NULL, 50.00, 180, false, true, false, NOW()),
    (uuid_generate_v4(), 'cccc2222-cccc-2222-cccc-222222222222', 'Ménage Régulier', 'Nettoyage complet de l''appartement (3h)', 'Ménage', 'Fixed', NULL, 250.00, 0.00, 180, true, true, false, NOW()),
    (uuid_generate_v4(), 'cccc2222-cccc-2222-cccc-222222222222', 'Grand Nettoyage', 'Nettoyage en profondeur avec produits professionnels', 'Ménage', 'Fixed', NULL, 500.00, 0.00, 360, true, true, false, NOW()),
    (uuid_generate_v4(), 'cccc2222-cccc-2222-cccc-222222222222', 'Nettoyage Vitres', 'Nettoyage de toutes les vitres et miroirs', 'Ménage', 'Hourly', 100.00, NULL, 0.00, 120, true, true, false, NOW())
ON CONFLICT DO NOTHING;

-- Demo Grocery Store
INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "Email", "NormalizedEmail", "UserName", "NormalizedUserName", "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "IsActive", "PreferredLanguage", "CreatedAt")
VALUES (
    '55555555-5555-5555-5555-555555555555',
    'Fatima', 'Alaoui',
    'fatima@demo.ma', 'FATIMA@DEMO.MA',
    'fatima@demo.ma', 'FATIMA@DEMO.MA',
    true, '+212600000004', true, false, true, 0,
    'AQAAAAIAAYagAAAAEKn7DHjPfKG5tnhYLxyE3H3OkE+zNJQ5UlBl6xjRCF5q9wN8CxX2ZT2F5h1N0k3Ug==',
    uuid_generate_v4()::text, uuid_generate_v4()::text,
    true, 'fr', NOW()
) ON CONFLICT DO NOTHING;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES ('55555555-5555-5555-5555-555555555555', 'f6a7b8c9-d0e1-2345-fabc-456789012345')
ON CONFLICT DO NOTHING;

-- Grocery Stores
INSERT INTO "GroceryStores" ("Id", "OwnerId", "Name", "Slug", "Brand", "Description", "Phone", "Street", "City", "Latitude", "Longitude", "AverageRating", "TotalReviews", "MinOrderAmount", "DeliveryFee", "FreeDeliveryThreshold", "CommissionRate", "IsActive", "IsVerified", "AcceptsOnlinePayment", "AcceptsCashPayment", "IsDeleted", "CreatedAt")
VALUES
    ('dddd1111-dddd-1111-dddd-111111111111', '55555555-5555-5555-5555-555555555555', 'Marjane Ain Diab', 'marjane-ain-diab', 'Marjane', 'Hypermarché Marjane - Ain Diab Casablanca', '+212522987654', 'Route d''Azemmour, Ain Diab', 'Casablanca', 33.5728, -7.6769, 4.2, 156, 100.00, 25.00, 300.00, 10.00, true, true, true, true, false, NOW()),
    ('dddd2222-dddd-2222-dddd-222222222222', '55555555-5555-5555-5555-555555555555', 'Carrefour Market Maârif', 'carrefour-maarif', 'Carrefour', 'Carrefour Market au coeur du Maârif', '+212522876543', 'Rue Ibnou Hazm, Maârif', 'Casablanca', 33.5870, -7.6320, 4.4, 203, 80.00, 20.00, 250.00, 10.00, true, true, true, true, false, NOW())
ON CONFLICT DO NOTHING;

-- Grocery Departments
INSERT INTO "GroceryDepartments" ("Id", "StoreId", "Name", "Description", "IconName", "SortOrder", "IsActive", "IsDeleted", "CreatedAt")
VALUES
    ('ee010000-0000-0000-0000-000000000001', 'dddd1111-dddd-1111-dddd-111111111111', 'Fruits & Légumes', 'Fruits et légumes frais', 'apple', 1, true, false, NOW()),
    ('ee010000-0000-0000-0000-000000000002', 'dddd1111-dddd-1111-dddd-111111111111', 'Viandes & Poissons', 'Boucherie et poissonnerie', 'drumstick', 2, true, false, NOW()),
    ('ee010000-0000-0000-0000-000000000003', 'dddd1111-dddd-1111-dddd-111111111111', 'Épicerie', 'Épicerie salée et sucrée', 'shopping-basket', 3, true, false, NOW()),
    ('ee010000-0000-0000-0000-000000000004', 'dddd1111-dddd-1111-dddd-111111111111', 'Boissons', 'Eaux, jus, sodas', 'glass-water', 4, true, false, NOW()),
    ('ee010000-0000-0000-0000-000000000005', 'dddd1111-dddd-1111-dddd-111111111111', 'Produits Laitiers', 'Lait, yaourts, fromages', 'milk', 5, true, false, NOW()),
    ('ee010000-0000-0000-0000-000000000006', 'dddd1111-dddd-1111-dddd-111111111111', 'Hygiène & Beauté', 'Soins, hygiène, beauté', 'spray-can', 6, true, false, NOW())
ON CONFLICT DO NOTHING;

-- Grocery Products (Marjane)
INSERT INTO "GroceryProducts" ("Id", "StoreId", "DepartmentId", "Name", "Description", "Brand", "Price", "PricePerUnit", "UnitMeasure", "Weight", "WeightUnit", "StockQuantity", "IsBio", "IsHalal", "IsAvailable", "IsPopular", "AllowReplacement", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000001', 'Tomates Rondes', 'Tomates rondes de saison, origine Maroc', NULL, 8.90, 8.90, 'kg', 1, 'kg', 500, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000001', 'Bananes', 'Bananes importées, mûres à point', NULL, 14.90, 14.90, 'kg', 1, 'kg', 200, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000001', 'Pommes Golden', 'Pommes Golden croquantes', NULL, 19.90, 19.90, 'kg', 1, 'kg', 150, false, true, true, false, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000002', 'Poulet Entier', 'Poulet fermier halal', 'Koutoubia', 45.00, 45.00, 'kg', 1.5, 'kg', 80, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000002', 'Viande Hachée', 'Viande hachée de boeuf halal', 'Koutoubia', 75.00, 75.00, 'kg', 0.5, 'kg', 60, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000003', 'Huile d''Olive Extra Vierge', 'Huile d''olive marocaine 1L', 'Oued Souss', 55.00, 55.00, 'l', 1, 'l', 120, true, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000003', 'Couscous Moyen', 'Couscous grains moyens 1kg', 'Dari', 12.50, 12.50, 'kg', 1, 'kg', 200, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000004', 'Eau Minérale Sidi Ali', 'Pack 6x1.5L', 'Sidi Ali', 24.00, 2.67, 'l', 9, 'l', 300, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000004', 'Jus d''Orange Marrakech', 'Jus d''orange 100% pur jus 1L', 'Valencia', 15.00, 15.00, 'l', 1, 'l', 100, false, true, true, false, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000005', 'Lait Entier Centrale', 'Lait entier UHT 1L', 'Centrale', 7.50, 7.50, 'l', 1, 'l', 400, false, true, true, true, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000005', 'Yaourt Nature Danone', 'Pack 12 pots yaourt nature', 'Danone', 28.00, 2.33, 'pièce', NULL, NULL, 150, false, true, true, false, true, false, NOW()),
    (uuid_generate_v4(), 'dddd1111-dddd-1111-dddd-111111111111', 'ee010000-0000-0000-0000-000000000006', 'Shampoing Head & Shoulders', 'Anti-pelliculaire 400ml', 'Head & Shoulders', 42.00, 105.00, 'l', 0.4, 'l', 80, false, true, true, false, true, false, NOW())
ON CONFLICT DO NOTHING;

-- Demo Promo Codes
INSERT INTO "PromoCodes" ("Id", "Code", "Description", "DiscountType", "DiscountValue", "MaxDiscount", "MinOrderAmount", "ApplicableModule", "MaxUsages", "CurrentUsages", "MaxUsagesPerUser", "StartDate", "EndDate", "IsActive", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), 'BIENVENUE', 'Première commande - 20% de réduction', 'Percentage', 20.00, 50.00, 50.00, NULL, 10000, 0, 1, NOW(), NOW() + INTERVAL '1 year', true, false, NOW()),
    (uuid_generate_v4(), 'LIVGRATUITE', 'Livraison gratuite', 'FreeDelivery', 0, NULL, 100.00, 'Restaurant', 5000, 0, 3, NOW(), NOW() + INTERVAL '6 months', true, false, NOW()),
    (uuid_generate_v4(), 'MENAGE50', '50 DH de réduction sur le ménage', 'FixedAmount', 50.00, NULL, 200.00, 'Service', 2000, 0, 1, NOW(), NOW() + INTERVAL '3 months', true, false, NOW()),
    (uuid_generate_v4(), 'COURSES10', '10% sur les courses', 'Percentage', 10.00, 30.00, 150.00, 'Grocery', 3000, 0, 2, NOW(), NOW() + INTERVAL '3 months', true, false, NOW())
ON CONFLICT DO NOTHING;

-- System Configuration
INSERT INTO "SystemConfigurations" ("Id", "Key", "Value", "Description", "Module", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), 'restaurant_commission_rate', '15', 'Taux de commission restaurants (%)', 'Restaurant', false, NOW()),
    (uuid_generate_v4(), 'service_commission_rate', '20', 'Taux de commission services (%)', 'Service', false, NOW()),
    (uuid_generate_v4(), 'grocery_commission_rate', '10', 'Taux de commission courses (%)', 'Grocery', false, NOW()),
    (uuid_generate_v4(), 'delivery_base_fee', '15', 'Frais de livraison de base (DH)', NULL, false, NOW()),
    (uuid_generate_v4(), 'delivery_per_km_fee', '3', 'Frais de livraison par km (DH)', NULL, false, NOW()),
    (uuid_generate_v4(), 'loyalty_points_per_dh', '1', 'Points de fidélité par DH dépensé', NULL, false, NOW()),
    (uuid_generate_v4(), 'loyalty_silver_threshold', '500', 'Seuil points pour tier Silver', NULL, false, NOW()),
    (uuid_generate_v4(), 'loyalty_gold_threshold', '2000', 'Seuil points pour tier Gold', NULL, false, NOW()),
    (uuid_generate_v4(), 'loyalty_platinum_threshold', '5000', 'Seuil points pour tier Platinum', NULL, false, NOW()),
    (uuid_generate_v4(), 'max_delivery_radius_km', '15', 'Rayon max de livraison (km)', NULL, false, NOW()),
    (uuid_generate_v4(), 'support_email', 'support@multiservices.ma', 'Email support', NULL, false, NOW()),
    (uuid_generate_v4(), 'support_phone', '+212522000000', 'Téléphone support', NULL, false, NOW())
ON CONFLICT DO NOTHING;

-- Geographic Zones
INSERT INTO "GeographicZones" ("Id", "Name", "City", "IsActive", "DeliveryFeeMultiplier", "IsDeleted", "CreatedAt")
VALUES
    (uuid_generate_v4(), 'Centre Ville', 'Casablanca', true, 1.0, false, NOW()),
    (uuid_generate_v4(), 'Maârif', 'Casablanca', true, 1.0, false, NOW()),
    (uuid_generate_v4(), 'Ain Diab', 'Casablanca', true, 1.2, false, NOW()),
    (uuid_generate_v4(), 'Hay Hassani', 'Casablanca', true, 1.3, false, NOW()),
    (uuid_generate_v4(), 'Sidi Moumen', 'Casablanca', true, 1.5, false, NOW()),
    (uuid_generate_v4(), 'Bernoussi', 'Casablanca', true, 1.4, false, NOW()),
    (uuid_generate_v4(), 'Anfa', 'Casablanca', true, 1.0, false, NOW()),
    (uuid_generate_v4(), 'Hay Mohammadi', 'Casablanca', true, 1.2, false, NOW()),
    (uuid_generate_v4(), 'Guéliz', 'Marrakech', true, 1.0, false, NOW()),
    (uuid_generate_v4(), 'Médina', 'Marrakech', true, 1.1, false, NOW()),
    (uuid_generate_v4(), 'Agdal', 'Rabat', true, 1.0, false, NOW()),
    (uuid_generate_v4(), 'Hassan', 'Rabat', true, 1.0, false, NOW())
ON CONFLICT DO NOTHING;

-- =====================================================
-- END OF SEED DATA
-- =====================================================
