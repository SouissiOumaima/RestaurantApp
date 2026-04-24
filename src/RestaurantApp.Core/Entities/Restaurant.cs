using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RestaurantApp.Core.Enums;

namespace RestaurantApp.Core.Entities;

public class Restaurant
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères")]
    [Display(Name = "Nom du restaurant")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est obligatoire")]
    [MaxLength(200)]
    [Display(Name = "Adresse")]
    public string Adresse { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type est obligatoire")]
    [Display(Name = "Type de cuisine")]
    public TypeRestaurant Type { get; set; }

    [Phone]
    [MaxLength(20)]
    [Display(Name = "Téléphone")]
    public string? Telephone { get; set; }

    [Range(0, 5)]
    [Display(Name = "Note globale")]
    public double NoteGlobale { get; set; } = 0;


    public ICollection<Employe> Employes { get; set; } = new List<Employe>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<TableRestaurant> Tables { get; set; } = new List<TableRestaurant>();
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public ICollection<Avis> Avis { get; set; } = new List<Avis>();
}