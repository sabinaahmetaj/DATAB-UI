using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROJEKTDB.Models
{
    [Table("RRESHT")]
    public class Rresht
    {
        [Column("FAT_ID")]
        public int FatId { get; set; }

        [Column("RRE_ID")]
        public byte RreId { get; set; }

        [Column("PIK_ID")]
        [Required]
        [StringLength(4)]
        public string PikId { get; set; } = string.Empty;

        [Column("RRE_SASI")]
        [Range(0, 255, ErrorMessage = "Sasia duhet të jetë 0–255.")]
        public byte RreSasi { get; set; } = 0;

        [Column("RRE_CMIM")]
        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Cmimi duhet të jetë >= 0.")]
        public decimal RreCmim { get; set; } = 0m;

        // ===== Navigation properties =====
        [ForeignKey(nameof(PikId))]
        public Pikture? Pikture { get; set; }

        [ForeignKey(nameof(FatId))]
        public Fature? Fature { get; set; }
    }
}
