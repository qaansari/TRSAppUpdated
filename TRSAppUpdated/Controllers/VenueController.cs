using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;

        public VenueController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 50)
        {
            var _query = _context.Venues.Where(x => !x.IsDeleted)
                .Select(x => new VenueVM()
                {
                    Title = x.Title,
                    Address = x.Address,
                    ImagePath = x.ImagePath,
                })
                .OrderByDescending(x => x.ID)
                .AsNoTracking()
                .AsQueryable();
            int _skip = (_pageNo - 1) * _pageSize;
            ViewBag.TotalRecords = await _query.CountAsync();
            ViewBag.PageNo = _pageNo;
            ViewBag.PageSize = _pageSize;
            _query = _query.Take(_pageSize).Skip(_skip);
            var _result = await _query.ToListAsync();

            return View();
        }
    }
}
