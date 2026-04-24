using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class Categorie
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le libellé est obligatoire")]
    [MaxLength(50)]
    [Display(Name = "Catégorie")]
    public string Libelle { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    // Navigation
    public ICollection<Plat> Plats { get; set; } = new List<Plat>();
}