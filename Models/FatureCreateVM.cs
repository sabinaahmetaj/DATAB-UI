using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PROJEKTDB.Models
{
    public class FatureCreateVM
    {
        [Required]
        public string PerId { get; set; } = string.Empty;

        public DateTime FatDat { get; set; } = DateTime.Now;

        [Required]
        public string PikId { get; set; } = string.Empty;

        [Range(1, 255)]
        public byte RreSasi { get; set; } = 1;

        // ====================
        // Vetëm për UI (mos u bindo / mos u valido nga POST)
        // ====================
        [BindNever]
        public string? PikTit { get; set; }

        [BindNever]
        public decimal PikCmim { get; set; }

        [BindNever]
        public string? ArtistEmri { get; set; }
    }
}
