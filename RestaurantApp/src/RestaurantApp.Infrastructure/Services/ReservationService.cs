using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class ReservationService
{
    private readonly AppDbContext _context;

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }

    // Réservations d'un client
    public async Task<List<Reservation>> GetByClientAsync(int clientId)
        => await _context.Reservations
            .Include(r => r.Restaurant)
            .Include(r => r.TableRestaurant)
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.Date)
            .ToListAsync();

    // Toutes les réservations (Admin)
    public async Task<List<Reservation>> GetAllAsync()
        => await _context.Reservations
            .Include(r => r.Restaurant)
            .Include(r => r.Client)
            .Include(r => r.TableRestaurant)
            .OrderByDescending(r => r.Date)
            .ToListAsync();

    // Créer une réservation
    public async Task<(bool Success, string Message)> CreateAsync(Reservation reservation)
    {
        // Vérifier que la table est disponible
        var conflit = await _context.Reservations
            .AnyAsync(r => r.TableRestaurantId == reservation.TableRestaurantId
                        && r.Date.Date == reservation.Date.Date
                        && r.StatutReservation);

        if (conflit)
            return (false, "Cette table est déjà réservée pour cette date.");

        // Vérifier la capacité
        var table = await _context.Tables.FindAsync(reservation.TableRestaurantId);
        if (table == null || table.Capacite < reservation.NbPersonnes)
            return (false, "La table n'a pas la capacité suffisante.");

        reservation.StatutReservation = true;
        reservation.DateCreation = DateTime.Now;

        _context.Reservations.Add(reservation);

        // Ajouter des points de fidélité au client
        var client = await _context.Clients.FindAsync(reservation.ClientId);
        if (client != null)
        {
            client.PointFidelite += 10;
        }

        await _context.SaveChangesAsync();
        return (true, "Réservation confirmée avec succès !");
    }

    // Annuler une réservation
    public async Task<bool> AnnulerAsync(int id, int clientId)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId);

        if (reservation == null) return false;

        reservation.StatutReservation = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
        => await _context.Reservations
            .Include(r => r.Restaurant)
            .Include(r => r.TableRestaurant)
            .Include(r => r.Client)
            .FirstOrDefaultAsync(r => r.Id == id);
}