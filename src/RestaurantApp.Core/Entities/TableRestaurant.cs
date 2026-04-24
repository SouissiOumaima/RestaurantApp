using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class TableRestaurant
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le numéro de table est obligatoire")]
    [Range(1, 999, ErrorMessage = "Le numéro doit être entre 1 et 999")]
    [Display(Name = "Numéro de table")]
    public int Numero { get; set; }

    [Required(ErrorMessage = "La capacité est obligatoire")]
    [Range(1, 20, ErrorMessage = "La capacité doit être entre 1 et 20")]
    [Display(Name = "Capacité (personnes)")]
    public int Capacite { get; set; }

    [Required]
    [Display(Name = "Disponible")]
    public bool Disponible { get; set; } = true;

    // FK
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}