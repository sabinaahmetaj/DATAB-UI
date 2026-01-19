using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROJEKTDB.Models
{
    [Table("PERSON")]
    public class Person
    {
        [Key]
        [Column("PER_ID")]
        [Required]
        [StringLength(10)]
        public string PerId { get; set; } = string.Empty;

        [Column("PER_EM")]
        [Required]
        [StringLength(25)]
        public string PerEm { get; set; } = string.Empty;

        [Column("PER_MB")]
        [Required]
        [StringLength(25)]
        public string PerMb { get; set; } = string.Empty;

        [Column("PER_DAT")]
        public DateTime PerDat { get; set; }

        [Column("PER_ZON")]
        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Zone duhet të ketë 4 shifra (p.sh. 1001).")]
        public string PerZon { get; set; } = string.Empty;

        [Column("PER_TEL")]
        [Required]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Telefoni duhet të ketë 9–12 karaktere.")]
        public string PerTel { get; set; } = string.Empty;
    }
}
