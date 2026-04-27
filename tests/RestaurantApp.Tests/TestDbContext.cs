using Microsoft.EntityFrameworkCore;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Tests;

public class TestDbContext : AppDbContext
{
    public TestDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}