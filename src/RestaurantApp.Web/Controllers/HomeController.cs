using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Infrastructure.Services;

namespace RestaurantApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly RestaurantService _restaurantService;

    public HomeController(RestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    public async Task<IActionResult> Index()
    {
        // Affiche les 6 meilleurs restaurants sur la page d'accueil
        var restaurants = await _restaurantService.GetAllAsync();
        var top6 = restaurants
            .OrderByDescending(r => r.NoteGlobale)
            .Take(6)
            .ToList();
        return View(top6);
    }

    public IActionResult Error()
    {
        return View();
    }
}