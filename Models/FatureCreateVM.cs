using System;
using System.ComponentModel.DataAnnotations;

namespace PROJEKTDB.Models
{
    public class FatureCreateVM
    {
        [Required]
        public int FatId { get; set; }

        [Required]
        public string PerId { get; set; } = string.Empty;

        public DateTime FatDat { get; set; } = DateTime.Now;

        [Required]
        public string PikId { get; set; } = string.Empty;

        [Range(1, 255)]
        public byte RreSasi { get; set; } = 1;

        // Vetëm për UI
        public string? PikTit { get; set; }
        public decimal PikCmim { get; set; }
        public string? ArtistEmri { get; set; }
    }
}
