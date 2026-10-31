using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbQLBH_65132292.Models
{
    public class SanPham
    {
        [Key]
        [StringLength(10)]
        public string MaSP { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TenSP { get; set; } = string.Empty;

        [StringLength(255)]
        public string? AnhSP { get; set; }

        [Column(TypeName = "money")]
        public decimal DonGia { get; set; }

        public string? MoTaCT { get; set; }

        public short MaLSP { get; set; }

        public byte MaDVT { get; set; }
    }
}