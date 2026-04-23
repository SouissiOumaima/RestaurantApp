using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;

namespace RestaurantApp.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly RestaurantService _restaurantService;
    private readonly ReservationService _reservationService;
    private readonly StatistiquesService _statistiquesService;

    public AdminController(
        AppDbContext context,
        RestaurantService restaurantService,
        ReservationService reservationService,
        StatistiquesService statistiquesService)
    {
        _context = context;
        _restaurantService = restaurantService;
        _reservationService = reservationService;
        _statistiquesService = statistiquesService;
    }

    // ── DASHBOARD ────────────────────────────────────────────
    public async Task<IActionResult> Dashboard()
    {
        var stats = await _statistiquesService.GetStatistiquesAsync();
        return View(stats);
    }

    // ══════════════════════════════════════════════════════════
    // CRUD RESTAURANTS
    // ══════════════════════════════════════════════════════════

    public async Task<IActionResult> Restaurants()
    {
        var list = await _restaurantService.GetAllAsync();
        return View(list);
    }

    public IActionResult CreateRestaurant() => View(new Restaurant());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRestaurant(Restaurant restaurant)
    {
        if (!ModelState.IsValid) return View(restaurant);
        await _restaurantService.CreateAsync(restaurant);
        TempData["Success"] = "Restaurant créé avec succès.";
        return RedirectToAction("Restaurants");
    }

    public async Task<IActionResult> EditRestaurant(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null) return NotFound();
        return View(restaurant);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRestaurant(Restaurant restaurant)
    {
        if (!ModelState.IsValid) return View(restaurant);
        await _restaurantService.UpdateAsync(restaurant);
        TempData["Success"] = "Restaurant modifié.";
        return RedirectToAction("Restaurants");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        var (success, message) = await _restaurantService.DeleteAsync(id);

        if (success)
            TempData["Success"] = message;
        else
            TempData["Error"] = message;

        return RedirectToAction("Restaurants");
    }

    // ══════════════════════════════════════════════════════════
    // CRUD EMPLOYES
    // ══════════════════════════════════════════════════════════

    public async Task<IActionResult> Employes()
    {
        var list = await _context.Employes
            .Include(e => e.Restaurant)
            .OrderBy(e => e.Nom)
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> CreateEmploye()
    {
        ViewBag.Restaurants = new SelectList(
            await _context.Restaurants.ToListAsync(), "Id", "Nom");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEmploye(
        string typeEmploye, string cin, string nom, string prenom,
        string email, string? numTel, double salaire,
        int restaurantId, int? note, string? zone,
        string? specialite, int? anneeExp)
    {
        Employe employe = typeEmploye == "Serveur"
            ? new Serveur { Note = note ?? 0, Zone = zone }
            : new Chef { Specialite = specialite ?? "", AnneeExp = anneeExp ?? 0 };

        employe.Cin = cin;
        employe.Nom = nom;
        employe.Prenom = prenom;
        employe.Email = email;
        employe.NumTel = numTel;
        employe.Salaire = salaire;
        employe.RestaurantId = restaurantId;

        _context.Employes.Add(employe);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Employé ajouté.";
        return RedirectToAction("Employes");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEmploye(int id)
    {
        var e = await _context.Employes.FindAsync(id);
        if (e != null) { _context.Employes.Remove(e); await _context.SaveChangesAsync(); }
        TempData["Success"] = "Employé supprimé.";
        return RedirectToAction("Employes");
    }

    // ══════════════════════════════════════════════════════════
    // CRUD TABLES
    // ══════════════════════════════════════════════════════════

    public async Task<IActionResult> Tables()
    {
        var list = await _context.Tables
            .Include(t => t.Restaurant)
            .OrderBy(t => t.RestaurantId).ThenBy(t => t.Numero)
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> CreateTable()
    {
        ViewBag.Restaurants = new SelectList(
            await _context.Restaurants.ToListAsync(), "Id", "Nom");
        return View(new TableRestaurant());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTable(TableRestaurant table)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Restaurants = new SelectList(
                await _context.Restaurants.ToListAsync(), "Id", "Nom");
            return View(table);
        }
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Table ajoutée.";
        return RedirectToAction("Tables");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTable(int id)
    {
        var t = await _context.Tables.FindAsync(id);
        if (t != null) { _context.Tables.Remove(t); await _context.SaveChangesAsync(); }
        TempData["Success"] = "Table supprimée.";
        return RedirectToAction("Tables");
    }

    // ══════════════════════════════════════════════════════════
    // CRUD MENUS + PLATS
    // ══════════════════════════════════════════════════════════

    public async Task<IActionResult> Menus()
    {
        var list = await _context.Menus
            .Include(m => m.Restaurant)
            .Include(m => m.Plats)
            .OrderBy(m => m.RestaurantId)
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> CreateMenu()
    {
        ViewBag.Restaurants = new SelectList(
            await _context.Restaurants.ToListAsync(), "Id", "Nom");
        return View(new Menu());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMenu(Menu menu)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Restaurants = new SelectList(
                await _context.Restaurants.ToListAsync(), "Id", "Nom");
            return View(menu);
        }
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Menu créé.";
        return RedirectToAction("Menus");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMenu(int id)
    {
        var m = await _context.Menus.FindAsync(id);
        if (m != null) { _context.Menus.Remove(m); await _context.SaveChangesAsync(); }
        TempData["Success"] = "Menu supprimé.";
        return RedirectToAction("Menus");
    }

    // ══════════════════════════════════════════════════════════
    // CRUD RESERVATIONS (Admin)
    // ══════════════════════════════════════════════════════════

    public async Task<IActionResult> Reservations()
    {
        var list = await _reservationService.GetAllAsync();
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var r = await _context.Reservations.FindAsync(id);
        if (r != null) { _context.Reservations.Remove(r); await _context.SaveChangesAsync(); }
        TempData["Success"] = "Réservation supprimée.";
        return RedirectToAction("Reservations");
    }

    // ══════════════════════════════════════════════════════════
    // CRUD CLIENTS
    // ══════════════════════════════════════════════════════════

    public async Task<IActionResult> Clients()
    {
        var list = await _context.Clients
            .OrderBy(c => c.Nom)
            .ToListAsync();
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var c = await _context.Clients.FindAsync(id);
        if (c != null) { _context.Clients.Remove(c); await _context.SaveChangesAsync(); }
        TempData["Success"] = "Client supprimé.";
        return RedirectToAction("Clients");
    }
}
