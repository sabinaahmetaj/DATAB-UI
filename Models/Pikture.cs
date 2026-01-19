using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROJEKTDB.Models
{
    [Table("PIKTURE")]
    public class Pikture
    {
        [Key]
        [Column("PIK_ID")]
        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIK_ID duhet të ketë 4 shifra (p.sh. 0001).")]
        public string PikId { get; set; } = string.Empty;

        [Column("PIK_TIT")]
        [Required]
        [StringLength(35)]
        public string PikTit { get; set; } = string.Empty;

        [Column("PIK_CMIM")]
        [Range(0, 9999999999.99, ErrorMessage = "Çmimi duhet të jetë >= 0.")]
        public decimal PikCmim { get; set; }

        [Column("PER_ID")]
        [Required]
        public string PerId { get; set; } = string.Empty;

        [Column("GAL_ID")]
        [Required]
        public string GalId { get; set; } = string.Empty;

        // Navigations (për dropdown + shfaqje)
        public Person? Person { get; set; }
        public Galeri? Galeri { get; set; }
    }
}
