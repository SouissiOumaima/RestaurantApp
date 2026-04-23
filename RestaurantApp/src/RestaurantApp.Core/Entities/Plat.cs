using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantApp.Core.Entities;

public class Plat
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(100)]
    [Display(Name = "Nom du plat")]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Range(0, 999, ErrorMessage = "Le prix doit être positif")]
    [Column(TypeName = "decimal(6,2)")]
    [Display(Name = "Prix (DT)")]
    public decimal Prix { get; set; }

    [Required]
    [Display(Name = "Disponible")]
    public bool Disponible { get; set; } = true;

    // FKs
    [Required]
    public int CategorieId { get; set; }
    public Categorie? Categorie { get; set; }

    [Required]
    public int MenuId { get; set; }
    public Menu? Menu { get; set; }
}