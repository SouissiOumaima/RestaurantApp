using System;
using System.Collections.Generic;
using System.Text;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<Restaurant> Restaurants { get; }
    public IRepository<Employe> Employes { get; }
    public IRepository<Client> Clients { get; }
    public IRepository<Reservation> Reservations { get; }
    public IRepository<TableRestaurant> Tables { get; }
    public IRepository<Menu> Menus { get; }
    public IRepository<Plat> Plats { get; }
    public IRepository<Categorie> Categories { get; }
    public IRepository<Avis> Avis { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Restaurants = new Repository<Restaurant>(context);
        Employes = new Repository<Employe>(context);
        Clients = new Repository<Client>(context);
        Reservations = new Repository<Reservation>(context);
        Tables = new Repository<TableRestaurant>(context);
        Menus = new Repository<Menu>(context);
        Plats = new Repository<Plat>(context);
        Categories = new Repository<Categorie>(context);
        Avis = new Repository<Avis>(context);
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}