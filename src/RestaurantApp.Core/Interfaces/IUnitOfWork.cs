using System;
using System.Collections.Generic;
using System.Text;

using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Restaurant> Restaurants { get; }
    IRepository<Employe> Employes { get; }
    IRepository<Client> Clients { get; }
    IRepository<Reservation> Reservations { get; }
    IRepository<TableRestaurant> Tables { get; }
    IRepository<Menu> Menus { get; }
    IRepository<Plat> Plats { get; }
    IRepository<Categorie> Categories { get; }
    IRepository<Avis> Avis { get; }

    Task<int> SaveChangesAsync();
}