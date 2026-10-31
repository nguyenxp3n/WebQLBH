using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class PhuongXa
    {
        [Key]
        [StringLength(5)]
        public string MaPX { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenPX { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        public string MaTinh { get; set; } = string.Empty;
    }
}
