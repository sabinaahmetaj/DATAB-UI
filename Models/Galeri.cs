using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROJEKTDB.Models
{
    [Table("GALERI")]
    public class Galeri
    {
        [Key]
        [Column("GAL_ID")]
        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "GAL_ID duhet të ketë 3 shifra (p.sh. 001).")]
        public string GalId { get; set; } = string.Empty;

        [Column("GAL_MNG")]
        [Required]
        [StringLength(25)]
        public string GalMng { get; set; } = string.Empty;

        [Column("GAL_TEL")]
        [Required]
        [StringLength(12, MinimumLength = 10)]
        [RegularExpression(@"^(06\d{8}|\+\d{10,11})$", ErrorMessage = "Telefoni duhet të jetë 06XXXXXXXX (10 shifra) ose +XXXXXXXXXXX (11–12 karaktere).")]
        public string GalTel { get; set; } = string.Empty;

        [Column("GAL_ZON")]
        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Zona duhet të ketë 4 shifra (p.sh. 1001).")]
        public string GalZon { get; set; } = string.Empty;
    }
}
