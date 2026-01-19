using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROJEKTDB.Models
{
    [Table("FATURE")]
    public class Fature
    {
        [Key]
        [Column("FAT_ID")]
        [Range(1, int.MaxValue, ErrorMessage = "FAT_ID duhet të jetë > 0.")]
        public int FatId { get; set; }

        [Column("PER_ID")]
        [Required]
        public string PerId { get; set; } = string.Empty;

        [Column("FAT_DAT")]
        public DateTime FatDat { get; set; }

        // ===== NAVIGATION =====

        public Person? Person { get; set; }

        // 🔑 Lidhja me RRESHT (1 Fature → shumë Rreshta)
        public virtual ICollection<Rresht> Rresht { get; set; } = new HashSet<Rresht>();
    }
}

