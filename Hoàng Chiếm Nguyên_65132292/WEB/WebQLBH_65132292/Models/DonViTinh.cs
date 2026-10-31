using System.ComponentModel.DataAnnotations;
namespace DbQLBH_65132292.Models
{
    public class DonViTinh
    {
        [Key]
        public byte MaDVT { get; set; }
        public string TenDVT { get; set; } = string.Empty;
    }
}
