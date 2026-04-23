using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(60)]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [MaxLength(60)]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Adresse")]
    public string? Adresse { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Les points doivent être positifs")]
    [Display(Name = "Points de fidélité")]
    public int PointFidelite { get; set; } = 0;

    [EmailAddress(ErrorMessage = "Format email invalide")]
    [MaxLength(150)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    // Lien vers le compte Identity
    [MaxLength(450)]
    public string? UserId { get; set; }

    // Navigation properties
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Avis> Avis { get; set; } = new List<Avis>();
}