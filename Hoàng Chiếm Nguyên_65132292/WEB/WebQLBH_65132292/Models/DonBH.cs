using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class DonBH
    {
        [Key]
        [StringLength(9)]
        public string MaDBH { get; set; } = string.Empty;

        public DateTime NgayBan { get; set; }

        [Required]
        [StringLength(150)]
        public string DCGH { get; set; } = string.Empty;

        [Required]
        [StringLength(5)]
        public string MaPX { get; set; } = string.Empty;

        public int MaKH { get; set; }
    }
}
