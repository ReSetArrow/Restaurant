using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Filters;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [ServiceFilter(typeof(LoginStatusFilter))]
    public class TableListController : Controller
    {
        private readonly RestaurantContext _context;
        public TableListController(RestaurantContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //select * from Table
            var table= await _context.Table.ToListAsync();
            return  View(table);
        }
        public IActionResult Cart()
        {

            return View();
        }
    }
}
