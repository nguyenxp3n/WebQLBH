using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DbQLBH_65132292.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("bao-caos")]
    public class BaoCaosController : Controller
    {
        // Compatibility endpoint: keep old route but delegate to canonical BaoCao flow.
        [HttpGet("")]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Index", "BaoCao");
        }
    }
}
