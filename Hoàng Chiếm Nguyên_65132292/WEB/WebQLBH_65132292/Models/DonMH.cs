using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models
{
    public class DonMH
    {
        [Key]
        [StringLength(7)]
        public string MaDMH { get; set; } = string.Empty;

        public DateTime NgayMua { get; set; }

        public short MaNCC { get; set; }
    }
}
