using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class LoaiSP
    {
        [Key]
        public short MaLSP { get; set; }

        [Required]
        [StringLength(100)]
        public string TenLSP { get; set; } = string.Empty;

        public short MaNSP { get; set; }
    }
}