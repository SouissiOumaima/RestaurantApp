using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class Chef : Employe
{
    [Required(ErrorMessage = "La spécialité est obligatoire")]
    [MaxLength(100)]
    [Display(Name = "Spécialité")]
    public string Specialite { get; set; } = string.Empty;

    [Range(0, 50, ErrorMessage = "Les années d'expérience doivent être entre 0 et 50")]
    [Display(Name = "Années d'expérience")]
    public int AnneeExp { get; set; } = 0;
}