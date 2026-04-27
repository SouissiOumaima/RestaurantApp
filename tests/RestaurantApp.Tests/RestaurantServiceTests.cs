using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;

namespace RestaurantApp.Tests;

public class RestaurantServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllRestaurants()
    {
        var context = CreateContext();
        context.Restaurants.AddRange(
            new Restaurant { Nom = "R1", Adresse = "A1", Type = TypeRestaurant.Italien },
            new Restaurant { Nom = "R2", Adresse = "A2", Type = TypeRestaurant.Japonais }
        );
        await context.SaveChangesAsync();

        var result = await new RestaurantService(context).GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Search_ByType_ShouldReturnFiltered()
    {
        var context = CreateContext();
        context.Restaurants.AddRange(
            new Restaurant { Nom = "R1", Adresse = "A1", Type = TypeRestaurant.Italien },
            new Restaurant { Nom = "R2", Adresse = "A2", Type = TypeRestaurant.Japonais },
            new Restaurant { Nom = "R3", Adresse = "A3", Type = TypeRestaurant.Italien }
        );
        await context.SaveChangesAsync();

        var result = await new RestaurantService(context)
            .SearchAsync(null, TypeRestaurant.Italien, null);

        result.Should().HaveCount(2);
        result.All(r => r.Type == TypeRestaurant.Italien).Should().BeTrue();
    }

    [Fact]
    public async Task Search_ByNom_ShouldReturnMatching()
    {
        var context = CreateContext();
        context.Restaurants.AddRange(
            new Restaurant { Nom = "La Dolce Vita", Adresse = "A1", Type = TypeRestaurant.Italien },
            new Restaurant { Nom = "Sakura Garden", Adresse = "A2", Type = TypeRestaurant.Japonais }
        );
        await context.SaveChangesAsync();

        var result = await new RestaurantService(context)
            .SearchAsync("Dolce", null, null);

        result.Should().HaveCount(1);
        result[0].Nom.Should().Contain("Dolce");
    }

    [Fact]
    public async Task Delete_ShouldRemoveRestaurant()
    {
        var context = CreateContext();
        var resto = new Restaurant
        { Nom = "A supprimer", Adresse = "X", Type = TypeRestaurant.Mexicain };
        context.Restaurants.Add(resto);
        await context.SaveChangesAsync();

        await new RestaurantService(context).DeleteAsync(resto.Id);

        var result = await context.Restaurants.FindAsync(resto.Id);
        result.Should().BeNull();
    }
}