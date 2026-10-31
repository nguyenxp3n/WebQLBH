using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class Tinh
    {
        [Key]
        [StringLength(2)]
        public string MaTinh { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenTinh { get; set; } = string.Empty;
    }
}
