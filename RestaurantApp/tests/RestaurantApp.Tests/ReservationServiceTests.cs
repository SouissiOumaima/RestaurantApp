using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;

namespace RestaurantApp.Tests;

public class ReservationServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task CreateReservation_ShouldSucceed_WhenTableAvailable()
    {
        var context = CreateContext();
        var service = new ReservationService(context);

        var restaurant = new Restaurant
        { Nom = "Test", Adresse = "Test", Type = TypeRestaurant.Italien };
        context.Restaurants.Add(restaurant);

        var client = new Client
        { Nom = "Test", Prenom = "User", PointFidelite = 0 };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var table = new TableRestaurant
        {
            RestaurantId = restaurant.Id,
            Numero = 1,
            Capacite = 4,
            Disponible = true
        };
        context.Tables.Add(table);
        await context.SaveChangesAsync();

        var reservation = new Reservation
        {
            RestaurantId = restaurant.Id,
            ClientId = client.ClientId,
            TableRestaurantId = table.Id,
            Date = DateTime.Now.AddDays(1),
            NbPersonnes = 2
        };

        var (success, message) = await service.CreateAsync(reservation);

        success.Should().BeTrue();
        message.Should().Contain("succès");
    }

    [Fact]
    public async Task CreateReservation_ShouldFail_WhenTableAlreadyBooked()
    {
        var context = CreateContext();
        var service = new ReservationService(context);

        var restaurant = new Restaurant
        { Nom = "Test", Adresse = "Test", Type = TypeRestaurant.Japonais };
        context.Restaurants.Add(restaurant);

        var client = new Client
        { Nom = "Test", Prenom = "User", PointFidelite = 0 };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var table = new TableRestaurant
        {
            RestaurantId = restaurant.Id,
            Numero = 1,
            Capacite = 4,
            Disponible = true
        };
        context.Tables.Add(table);
        await context.SaveChangesAsync();

        // Première réservation existante
        context.Reservations.Add(new Reservation
        {
            RestaurantId = restaurant.Id,
            ClientId = client.ClientId,
            TableRestaurantId = table.Id,
            Date = DateTime.Now.AddDays(1),
            NbPersonnes = 2,
            StatutReservation = true
        });
        await context.SaveChangesAsync();

        // Deuxième tentative sur la même table
        var res2 = new Reservation
        {
            RestaurantId = restaurant.Id,
            ClientId = client.ClientId,
            TableRestaurantId = table.Id,
            Date = DateTime.Now.AddDays(1),
            NbPersonnes = 2
        };

        var (success, message) = await service.CreateAsync(res2);

        success.Should().BeFalse();
        message.Should().Contain("déjà réservée");
    }

    [Fact]
    public async Task CreateReservation_ShouldAddFidelityPoints()
    {
        var context = CreateContext();
        var service = new ReservationService(context);

        var restaurant = new Restaurant
        { Nom = "Test", Adresse = "Test", Type = TypeRestaurant.Mexicain };
        context.Restaurants.Add(restaurant);

        var client = new Client
        { Nom = "Test", Prenom = "User", PointFidelite = 0 };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var table = new TableRestaurant
        {
            RestaurantId = restaurant.Id,
            Numero = 1,
            Capacite = 4,
            Disponible = true
        };
        context.Tables.Add(table);
        await context.SaveChangesAsync();

        await service.CreateAsync(new Reservation
        {
            RestaurantId = restaurant.Id,
            ClientId = client.ClientId,
            TableRestaurantId = table.Id,
            Date = DateTime.Now.AddDays(1),
            NbPersonnes = 2
        });

        var updated = await context.Clients.FindAsync(client.ClientId);
        updated!.PointFidelite.Should().Be(10);
    }
}