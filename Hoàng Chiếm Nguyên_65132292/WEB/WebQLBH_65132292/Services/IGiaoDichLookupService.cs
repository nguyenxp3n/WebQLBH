using DbQLBH_65132292.Models.ViewModels;

namespace DbQLBH_65132292.Services
{
    public interface IGiaoDichLookupService
    {
        Task<GiaoDichTongQuanSnapshot> GetTongQuanAsync();

        Task<List<DonMuaTongHopItem>> GetDonMuaTongHopAsync(
            string? tuKhoa = null,
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            int top = 500);

        Task<List<DonBanTongHopItem>> GetDonBanTongHopAsync(
            string? tuKhoa = null,
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            int top = 500);

        Task<GiaoDichPagedResult<DonMuaTongHopItem>> GetDonMuaTongHopPagedAsync(
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            int page,
            int pageSize);

        Task<GiaoDichPagedResult<DonBanTongHopItem>> GetDonBanTongHopPagedAsync(
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            int page,
            int pageSize);
    }

    public sealed class GiaoDichTongQuanSnapshot
    {
        public int TongDonMua { get; init; }
        public int TongDonBan { get; init; }
        public decimal TongGiaTriMua { get; init; }
        public decimal TongGiaTriBan { get; init; }
    }

    public sealed class GiaoDichPagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    }
}
