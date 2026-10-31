using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class NhaCC
    {
        [Key]
        public short MaNCC { get; set; }

        [Required]
        [StringLength(50)]
        public string TenNCC { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DienThoai { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(150)]
        public string DiaChi { get; set; } = string.Empty;

        [Required]
        [StringLength(5)]
        public string MaPX { get; set; } = string.Empty;
    }
}
