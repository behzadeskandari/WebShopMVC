using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShopMVC.Data;
using WebShopMVC.Extensions;

namespace WebShopMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();


            return View(productList);
        }

        public async  Task<IActionResult> Details(int id)
        {
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(x => x.Id == id).FirstOrDefaultAsync();



            return View(productList);
        }


        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<int>();
            }
            lstShoppingCart.Add(id);

            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        public IActionResult Remove(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                if (lstShoppingCart.Contains(id))
                {
                    lstShoppingCart.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Privacy()
        {
            return View();
        }
    }
}
