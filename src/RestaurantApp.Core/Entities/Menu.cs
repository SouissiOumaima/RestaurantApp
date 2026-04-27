using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantApp.Core.Entities;

public class Menu
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le libellé est obligatoire")]
    [MaxLength(100)]
    [Display(Name = "Libellé du menu")]
    public string Libelle { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Range(0, 9999, ErrorMessage = "Le prix doit être positif")]
    [Column(TypeName = "decimal(8,2)")]
    [Display(Name = "Prix total (DT)")]
    public decimal PrixTotal { get; set; }

    [Display(Name = "Disponible à partir de")]
    public TimeSpan? DisponibleDu { get; set; }

    // FK
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    // Navigation
    public ICollection<Plat> Plats { get; set; } = new List<Plat>();
}