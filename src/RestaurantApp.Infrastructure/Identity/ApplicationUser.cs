using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [MaxLength(60)]
    [Display(Name = "Nom")]
    public string? Nom { get; set; }

    [MaxLength(60)]
    [Display(Name = "Prénom")]
    public string? Prenom { get; set; }

    [Required]
    [Display(Name = "Date d'inscription")]
    public DateTime DateInscription { get; set; } = DateTime.Now;
}