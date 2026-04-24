using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class Serveur : Employe
{
    [Range(0, 5, ErrorMessage = "La note doit être entre 0 et 5")]
    [Display(Name = "Note de performance")]
    public int Note { get; set; } = 0;

    [MaxLength(50)]
    [Display(Name = "Zone de service")]
    public string? Zone { get; set; }
}