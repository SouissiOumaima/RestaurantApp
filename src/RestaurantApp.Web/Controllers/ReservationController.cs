using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Identity;
using RestaurantApp.Infrastructure.Services;
using RestaurantApp.Web.ViewModels;

namespace RestaurantApp.Web.Controllers;

[Authorize]
public class ReservationController : Controller
{
    private readonly ReservationService _reservationService;
    private readonly RestaurantService _restaurantService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public ReservationController(
        ReservationService reservationService,
        RestaurantService restaurantService,
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _reservationService = reservationService;
        _restaurantService = restaurantService;
        _userManager = userManager;
        _context = context;
    }

    // GET /Reservation — mes réservations
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.UserId == user!.Id);

        if (client == null) return View(new List<Reservation>());

        var reservations = await _reservationService.GetByClientAsync(client.ClientId);
        return View(reservations);
    }

    // GET /Reservation/Create?restaurantId=1
    public async Task<IActionResult> Create(int restaurantId)
    {
        var restaurant = await _restaurantService.GetByIdAsync(restaurantId);
        if (restaurant == null) return NotFound();

        var vm = new ReservationCreateViewModel
        {
            RestaurantId = restaurantId,
            RestaurantNom = restaurant.Nom,
            Date = DateTime.Now.Date.AddDays(1).AddHours(20)
        };
        return View(vm);
    }

    // POST /Reservation/GetTables — AJAX tables disponibles
    [HttpPost]
    public async Task<IActionResult> GetTables(
    [FromBody] GetTablesRequest request)
    {
        var tables = await _restaurantService
            .GetTablesDisponiblesAsync(
                request.RestaurantId,
                DateTime.Parse(request.Date),
                request.NbPersonnes);

        return Json(tables.Select(t => new
        {
            t.Id,
            t.Numero,
            t.Capacite
        }));
    }

    public class GetTablesRequest
    {
        public int RestaurantId { get; set; }
        public string Date { get; set; } = string.Empty;
        public int NbPersonnes { get; set; }
    }

    // POST /Reservation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.TablesDisponibles = await _restaurantService
                .GetTablesDisponiblesAsync(vm.RestaurantId, vm.Date, vm.NbPersonnes);
            return View(vm);
        }

        var user = await _userManager.GetUserAsync(User);
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.UserId == user!.Id);

        if (client == null)
        {
            TempData["Error"] = "Profil client introuvable.";
            return RedirectToAction("Index", "Home");
        }

        var reservation = new Reservation
        {
            RestaurantId = vm.RestaurantId,
            ClientId = client.ClientId,
            TableRestaurantId = vm.TableRestaurantId,
            Date = vm.Date,
            NbPersonnes = vm.NbPersonnes,
            Notes = vm.Notes
        };

        var (success, message) = await _reservationService.CreateAsync(reservation);

        if (success)
        {
            TempData["Success"] = message;
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", message);
        vm.TablesDisponibles = await _restaurantService
            .GetTablesDisponiblesAsync(vm.RestaurantId, vm.Date, vm.NbPersonnes);
        return View(vm);
    }

    // POST /Reservation/Annuler/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Annuler(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.UserId == user!.Id);

        if (client == null) return Forbid();

        var success = await _reservationService.AnnulerAsync(id, client.ClientId);
        TempData[success ? "Success" : "Error"] =
            success ? "Réservation annulée." : "Impossible d'annuler cette réservation.";

        return RedirectToAction("Index");
    }
}