using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;

namespace RestaurantApp.Web.ViewModels;

public class RestaurantSearchViewModel
{
    public string? Nom { get; set; }
    public TypeRestaurant? Type { get; set; }
    public decimal? PrixMax { get; set; }
    public List<Restaurant> Results { get; set; } = new();
}