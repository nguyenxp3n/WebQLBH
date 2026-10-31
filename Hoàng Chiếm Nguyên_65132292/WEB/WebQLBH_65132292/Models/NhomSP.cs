using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class NhomSP
    {
        [Key]
        public short MaNSP { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNSP { get; set; } = string.Empty;
    }
}

