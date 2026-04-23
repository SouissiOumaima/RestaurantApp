using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantApp.Core.Entities;

public abstract class Employe
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le CIN est obligatoire")]
    [MaxLength(20)]
    [Display(Name = "CIN")]
    public string Cin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(60)]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [MaxLength(60)]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format email invalide")]
    [MaxLength(150)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    [Display(Name = "Numéro de téléphone")]
    public string? NumTel { get; set; }

    [Required]
    [Range(0, 99999, ErrorMessage = "Le salaire doit être positif")]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Salaire")]
    public double Salaire { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}