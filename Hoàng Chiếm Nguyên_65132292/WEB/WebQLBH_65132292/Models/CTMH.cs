using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbQLBH_65132292.Models
{
    public class CTMH
    {
        [StringLength(7)]
        public string MaDMH { get; set; } = string.Empty;

        [StringLength(10)]
        public string MaSP { get; set; } = string.Empty;

        public short SLM { get; set; }

        [Column(TypeName = "money")]
        public decimal DGM { get; set; }
    }
}
