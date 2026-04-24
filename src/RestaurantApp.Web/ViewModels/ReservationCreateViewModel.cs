using System.ComponentModel.DataAnnotations;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Web.ViewModels;

public class ReservationCreateViewModel
{
    public int RestaurantId { get; set; }
    public string RestaurantNom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date est obligatoire")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Date et heure")]
    public DateTime Date { get; set; } = DateTime.Now.AddDays(1);

    [Required]
    [Range(1, 20)]
    [Display(Name = "Nombre de personnes")]
    public int NbPersonnes { get; set; } = 2;

    [Display(Name = "Notes / demandes spéciales")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Veuillez choisir une table")]
    [Display(Name = "Table")]
    public int TableRestaurantId { get; set; }

    public List<TableRestaurant> TablesDisponibles { get; set; } = new();
}