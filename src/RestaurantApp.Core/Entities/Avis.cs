using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Core.Entities;

public class Avis
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "La note doit être entre 1 et 5")]
    [Display(Name = "Note")]
    public int Note { get; set; }

    [MaxLength(1000)]
    [Display(Name = "Commentaire")]
    public string? Commentaire { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Date de l'avis")]
    public DateTime DateAvis { get; set; } = DateTime.Now;

    // FKs
    [Required]
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    [Required]
    public int ClientId { get; set; }
    public Client? Client { get; set; }
}
