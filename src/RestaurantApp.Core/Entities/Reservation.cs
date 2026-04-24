using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class Reservation
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "La date est obligatoire")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Date et heure")]
    public DateTime Date { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "Le nombre de personnes doit être entre 1 et 20")]
    [Display(Name = "Nombre de personnes")]
    public int NbPersonnes { get; set; }

    [Required]
    [Display(Name = "Confirmée")]
    public bool StatutReservation { get; set; } = false;

    [MaxLength(500)]
    [Display(Name = "Notes / demandes spéciales")]
    public string? Notes { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime DateCreation { get; set; } = DateTime.Now;

    // FKs
    [Required]
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    [Required]
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    [Required]
    public int TableRestaurantId { get; set; }
    public TableRestaurant? TableRestaurant { get; set; }
}