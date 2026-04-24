using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class RestaurantService
{
    private readonly AppDbContext _context;

    public RestaurantService(AppDbContext context)
    {
        _context = context;
    }

    // Tous les restaurants avec leurs tables
    public async Task<List<Restaurant>> GetAllAsync()
        => await _context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.Menus).ThenInclude(m => m.Plats)
            .Include(r => r.Avis)
            .OrderBy(r => r.Nom)
            .ToListAsync();

    // Détail d'un restaurant
    public async Task<Restaurant?> GetByIdAsync(int id)
        => await _context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.Menus).ThenInclude(m => m.Plats)
                .ThenInclude(p => p.Categorie)
            .Include(r => r.Avis).ThenInclude(a => a.Client)
            .Include(r => r.Employes)
            .FirstOrDefaultAsync(r => r.Id == id);

    // Recherche multicritères (Pattern Strategy simplifié)
    public async Task<List<Restaurant>> SearchAsync(
        string? nom, TypeRestaurant? type, decimal? prixMax)
    {
        var query = _context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.Menus).ThenInclude(m => m.Plats)
            .Include(r => r.Avis)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nom))
            query = query.Where(r => r.Nom.Contains(nom));

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        if (prixMax.HasValue)
            query = query.Where(r =>
                r.Menus.Any(m => m.Plats.Any(p => p.Prix <= prixMax.Value)));

        return await query.OrderBy(r => r.Nom).ToListAsync();
    }

    // Tables disponibles d'un restaurant pour une date
    public async Task<List<TableRestaurant>> GetTablesDisponiblesAsync(
        int restaurantId, DateTime date, int nbPersonnes)
    {
        var tablesReservees = await _context.Reservations
            .Where(r => r.RestaurantId == restaurantId
                     && r.Date.Date == date.Date
                     && r.StatutReservation)
            .Select(r => r.TableRestaurantId)
            .ToListAsync();

        return await _context.Tables
            .Where(t => t.RestaurantId == restaurantId
                     && t.Disponible
                     && t.Capacite >= nbPersonnes
                     && !tablesReservees.Contains(t.Id))
            .ToListAsync();
    }

    // CRUD Admin
    public async Task CreateAsync(Restaurant restaurant)
    {
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var restaurant = await _context.Restaurants
            .Include(r => r.Employes)
            .Include(r => r.Tables).ThenInclude(t => t.Reservations)
            .Include(r => r.Menus)
            .Include(r => r.Avis)
            .Include(r => r.Reservations)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant == null)
            return (false, "Restaurant introuvable.");

        // Vérifier les dépendances
        var obstacles = new List<string>();

        if (restaurant.Employes.Any())
            obstacles.Add($"{restaurant.Employes.Count} employé(s)");

        if (restaurant.Reservations.Any())
            obstacles.Add($"{restaurant.Reservations.Count} réservation(s)");

        if (restaurant.Tables.Any(t => t.Reservations.Any()))
            obstacles.Add("des réservations liées aux tables");

        if (restaurant.Menus.Any())
            obstacles.Add($"{restaurant.Menus.Count} menu(s)");

        if (restaurant.Avis.Any())
            obstacles.Add($"{restaurant.Avis.Count} avis");

        if (obstacles.Any())
        {
            var liste = string.Join(", ", obstacles);
            return (false,
                $"Impossible de supprimer « {restaurant.Nom} ». " +
                $"Veuillez d'abord supprimer : {liste}.");
        }

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();
        return (true, $"Restaurant « {restaurant.Nom} » supprimé avec succès.");
    }

    // Recalcule la note globale depuis les avis
    public async Task RecalculerNoteAsync(int restaurantId)
    {
        var avis = await _context.Avis
            .Where(a => a.RestaurantId == restaurantId)
            .ToListAsync();

        var restaurant = await _context.Restaurants.FindAsync(restaurantId);
        if (restaurant != null && avis.Any())
        {
            restaurant.NoteGlobale = avis.Average(a => a.Note);
            await _context.SaveChangesAsync();
        }
    }
}