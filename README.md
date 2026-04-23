# RestaurantApp — Gestion des Restaurants .NET 8

## Description
Application Web ASP.NET Core MVC pour la gestion des restaurants,
réservations, employés et menus.

## Architecture
- **RestaurantApp.Core** — Entités, Enums, Interfaces (domaine pur)
- **RestaurantApp.Infrastructure** — EF Core, Repositories, Services, Identity
- **RestaurantApp.Web** — Controllers, Views, ViewModels

## Patterns utilisés
- Repository + Unit of Work
- Dependency Injection (natif ASP.NET Core)
- Factory (création d'Employés)
- Strategy (recherche multicritères)
- TPH — Table Per Hierarchy (héritage Employe/Serveur/Chef)

## Prérequis
- .NET 8 SDK
- SQL Server LocalDB ou SQL Server Express
- Visual Studio 2022 / VS Code

## Installation

### 1. Cloner le projet
git clone <url>
cd RestaurantApp

### 2. Appliquer les migrations
dotnet ef database update --project src/RestaurantApp.Infrastructure
  --startup-project src/RestaurantApp.Web

### 3. Lancer l'application
dotnet run --project src/RestaurantApp.Web

### 4. Accéder à l'application
http://localhost:5000

## Comptes de test
| Rôle  | Email                  | Mot de passe |
|-------|------------------------|--------------|
| Admin | admin@restaurant.com   | Admin123!    |
| User  | user@test.com          | User123!     |

## Fonctionnalités
### Utilisateur
- Inscription / Connexion
- Consulter la liste des restaurants
- Recherche par nom, type, prix
- Réserver une table avec vérification disponibilité
- Voir et annuler ses réservations
- Points de fidélité (+10 par réservation)

### Administrateur
- Dashboard avec statistiques et graphiques
- CRUD complet : Restaurants, Employés, Tables, Menus, Réservations, Clients

## Tests
dotnet test tests/RestaurantApp.Tests 
