using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbQLBH_65132292.Models
{
    public class CTBH
    {
        [StringLength(9)]
        public string MaDBH { get; set; } = string.Empty;

        [StringLength(10)]
        public string MaSP { get; set; } = string.Empty;

        public byte SLB { get; set; }

        [Column(TypeName = "money")]
        public decimal DGB { get; set; }
    }
}
