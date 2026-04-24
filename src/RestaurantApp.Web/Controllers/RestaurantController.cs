using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Identity;
using RestaurantApp.Infrastructure.Services;
using RestaurantApp.Web.ViewModels;

namespace RestaurantApp.Web.Controllers;

public class RestaurantController : Controller
{
    private readonly RestaurantService _restaurantService;

    public RestaurantController(RestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    // GET /Restaurant — liste + recherche
    public async Task<IActionResult> Index(
        string? nom, TypeRestaurant? type, decimal? prixMax)
    {
        var vm = new RestaurantSearchViewModel
        {
            Nom = nom,
            Type = type,
            PrixMax = prixMax,
            Results = await _restaurantService.SearchAsync(nom, type, prixMax)
        };
        return View(vm);
    }

    // GET /Restaurant/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var restaurant = await _restaurantService.GetByIdAsync(id);
        if (restaurant == null) return NotFound();
        return View(restaurant);
    }
}