using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class StatistiquesService
{
    private readonly AppDbContext _context;

    public StatistiquesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StatistiquesViewModel> GetStatistiquesAsync()
    {
        var totalRestaurants = await _context.Restaurants.CountAsync();
        var totalClients = await _context.Clients.CountAsync();
        var totalReservations = await _context.Reservations.CountAsync();
        var totalEmployes = await _context.Employes.CountAsync();

        var reservationsMois = await _context.Reservations
            .Where(r => r.Date.Month == DateTime.Now.Month
                     && r.Date.Year == DateTime.Now.Year)
            .CountAsync();

        var restaurantTop = await _context.Restaurants
            .OrderByDescending(r => r.NoteGlobale)
            .Select(r => new { r.Nom, r.NoteGlobale })
            .Take(5)
            .ToListAsync();

        var reservationsParMois = await _context.Reservations
            .GroupBy(r => new { r.Date.Year, r.Date.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .Take(6)
            .ToListAsync();

        return new StatistiquesViewModel
        {
            TotalRestaurants = totalRestaurants,
            TotalClients = totalClients,
            TotalReservations = totalReservations,
            TotalEmployes = totalEmployes,
            ReservationsMois = reservationsMois,
            TopRestaurants = restaurantTop
                .Select(r => new TopRestaurantItem
                { Nom = r.Nom, Note = r.NoteGlobale }).ToList(),
            ReservationsParMois = reservationsParMois
                .Select(r => new ReservationsMoisItem
                { Mois = $"{r.Month}/{r.Year}", Count = r.Count }).ToList()
        };
    }
}

// ViewModels statistiques
public class StatistiquesViewModel
{
    public int TotalRestaurants { get; set; }
    public int TotalClients { get; set; }
    public int TotalReservations { get; set; }
    public int TotalEmployes { get; set; }
    public int ReservationsMois { get; set; }
    public List<TopRestaurantItem> TopRestaurants { get; set; } = new();
    public List<ReservationsMoisItem> ReservationsParMois { get; set; } = new();
}

public class TopRestaurantItem
{
    public string Nom { get; set; } = string.Empty;
    public double Note { get; set; }
}

public class ReservationsMoisItem
{
    public string Mois { get; set; } = string.Empty;
    public int Count { get; set; }
}