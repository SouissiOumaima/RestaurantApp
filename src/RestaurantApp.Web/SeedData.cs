using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Identity;

namespace RestaurantApp.Web;

public static class SeedData
{
    public static async Task InitializeAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        // ── 1. Rôles ────────────────────────────────────────
        foreach (var role in new[] { "Admin", "User" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        // ── 2. Compte Admin ──────────────────────────────────
        const string adminEmail = "admin@restaurant.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nom = "Admin",
                Prenom = "Super",
                DateInscription = DateTime.Now,
                EmailConfirmed = true
            };
            var r = await userManager.CreateAsync(admin, "Admin123!");
            if (r.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── 3. Données de test (si base vide) ───────────────
        if (await context.Restaurants.AnyAsync()) return;

        // Catégories
        var categories = new List<Categorie>
        {
            new() { Libelle = "Entrée",       Description = "Starters" },
            new() { Libelle = "Plat principal",Description = "Main course" },
            new() { Libelle = "Dessert",      Description = "Sweets" },
            new() { Libelle = "Boisson",      Description = "Drinks" }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // Restaurants
        var restaurants = new List<Restaurant>
        {
            new() {
                Nom = "La Dolce Vita", Adresse = "12 Rue de la Paix, Tunis",
                Type = TypeRestaurant.Italien, Telephone = "71 000 001",
                NoteGlobale = 4.5
            },
            new() {
                Nom = "Sakura Garden", Adresse = "8 Avenue Habib Bourguiba, Tunis",
                Type = TypeRestaurant.Japonais, Telephone = "71 000 002",
                NoteGlobale = 4.2
            },
            new() {
                Nom = "El Sombrero", Adresse = "5 Rue Ibn Khaldoun, Sfax",
                Type = TypeRestaurant.Mexicain, Telephone = "74 000 003",
                NoteGlobale = 3.8
            },
            new() {
                Nom = "Le Bouchon", Adresse = "3 Rue de Marseille, Tunis",
                Type = TypeRestaurant.Francais, Telephone = "71 000 004",
                NoteGlobale = 4.7
            }
        };
        context.Restaurants.AddRange(restaurants);
        await context.SaveChangesAsync();

        // Tables
        foreach (var resto in restaurants)
        {
            for (int i = 1; i <= 5; i++)
            {
                context.Tables.Add(new TableRestaurant
                {
                    RestaurantId = resto.Id,
                    Numero = i,
                    Capacite = i % 2 == 0 ? 4 : 2,
                    Disponible = true
                });
            }
        }
        await context.SaveChangesAsync();

        // Employés
        context.Employes.AddRange(
            new Serveur
            {
                Cin = "12345678",
                Nom = "Ben Ali",
                Prenom = "Mohamed",
                Email = "serveur1@restaurant.com",
                Salaire = 1200,
                RestaurantId = restaurants[0].Id,
                Note = 4,
                Zone = "Salle A"
            },
            new Chef
            {
                Cin = "87654321",
                Nom = "Dupont",
                Prenom = "Pierre",
                Email = "chef1@restaurant.com",
                Salaire = 2500,
                RestaurantId = restaurants[0].Id,
                Specialite = "Pâtes fraîches",
                AnneeExp = 10
            },
            new Serveur
            {
                Cin = "11223344",
                Nom = "Tanaka",
                Prenom = "Yuki",
                Email = "serveur2@restaurant.com",
                Salaire = 1100,
                RestaurantId = restaurants[1].Id,
                Note = 5,
                Zone = "Salle principale"
            },
            new Chef
            {
                Cin = "44332211",
                Nom = "Yamamoto",
                Prenom = "Kenji",
                Email = "chef2@restaurant.com",
                Salaire = 3000,
                RestaurantId = restaurants[1].Id,
                Specialite = "Sushi & Sashimi",
                AnneeExp = 15
            }
        );
        await context.SaveChangesAsync();

        // Menus + Plats
        var menu1 = new Menu
        {
            Libelle = "Menu Classique",
            PrixTotal = 35.00m,
            RestaurantId = restaurants[0].Id,
            Description = "Le menu traditionnel italien"
        };
        var menu2 = new Menu
        {
            Libelle = "Menu Dégustation",
            PrixTotal = 55.00m,
            RestaurantId = restaurants[1].Id,
            Description = "Découverte de la cuisine japonaise"
        };
        context.Menus.AddRange(menu1, menu2);
        await context.SaveChangesAsync();

        context.Plats.AddRange(
            new Plat
            {
                Nom = "Bruschetta",
                Prix = 8.00m,
                Disponible = true,
                MenuId = menu1.Id,
                CategorieId = categories[0].Id,
                Description = "Pain grillé, tomates, basilic"
            },
            new Plat
            {
                Nom = "Spaghetti Carbonara",
                Prix = 18.00m,
                Disponible = true,
                MenuId = menu1.Id,
                CategorieId = categories[1].Id,
                Description = "Pâtes fraîches, pancetta, œuf"
            },
            new Plat
            {
                Nom = "Tiramisu",
                Prix = 9.00m,
                Disponible = true,
                MenuId = menu1.Id,
                CategorieId = categories[2].Id,
                Description = "Dessert traditionnel au mascarpone"
            },
            new Plat
            {
                Nom = "Soupe Miso",
                Prix = 6.00m,
                Disponible = true,
                MenuId = menu2.Id,
                CategorieId = categories[0].Id,
                Description = "Soupe traditionnelle japonaise"
            },
            new Plat
            {
                Nom = "Sushi Mix (12 pièces)",
                Prix = 28.00m,
                Disponible = true,
                MenuId = menu2.Id,
                CategorieId = categories[1].Id,
                Description = "Assortiment de sushis du chef"
            },
            new Plat
            {
                Nom = "Mochi Ice Cream",
                Prix = 8.00m,
                Disponible = true,
                MenuId = menu2.Id,
                CategorieId = categories[2].Id,
                Description = "Glace enrobée de mochi"
            }
        );
        await context.SaveChangesAsync();

        // Compte User de test + Client lié
        const string userEmail = "user@test.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                Nom = "Dupont",
                Prenom = "Jean",
                DateInscription = DateTime.Now,
                EmailConfirmed = true
            };
            var r = await userManager.CreateAsync(user, "User123!");
            if (r.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                context.Clients.Add(new Client
                {
                    Nom = "Dupont",
                    Prenom = "Jean",
                    Email = userEmail,
                    UserId = user.Id,
                    PointFidelite = 30
                });
                await context.SaveChangesAsync();
            }
        }
    }
}