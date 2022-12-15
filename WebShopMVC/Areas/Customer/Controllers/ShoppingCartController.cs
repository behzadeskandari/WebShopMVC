using Microsoft.AspNetCore.Mvc;
using WebShopMVC.Data;
using WebShopMVC.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebShopMVC.Extensions;
using WebShopMVC.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebShopMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        [BindProperty()]
        public ShoppingCartViewModel shoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            shoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };

        }

        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                foreach (var cartItem in lstShoppingCart)
                {
                    Products prod = await _db.Products.Include(p => p.SpecialTags).Include(x => x.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefaultAsync();

                    shoppingCartVM.Products.Add(prod);
                }
            }

            return View(shoppingCartVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            shoppingCartVM.Appointments.AppointmentDate = shoppingCartVM.Appointments.AppointmentDate.AddHours(shoppingCartVM.Appointments.AppoinementTime.Hour).AddMinutes(shoppingCartVM.Appointments.AppoinementTime.Minute);

            Appointments appointments = shoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);

            _db.SaveChanges();

            int appoinmentId = appointments.Id;

            foreach (var productId in lstCartItems)
            {
                ProductSelectedForAppoinment productSelectedForAppoinment = new ProductSelectedForAppoinment
                {
                    AppoinmentId = appoinmentId,
                    ProductId = productId,

                };
                _db.ProductSelectedForAppoinment.Add(productSelectedForAppoinment);

            }

                _db.SaveChanges();
            lstCartItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction("AppoinmentConfirmation","ShoppingCart", new { Id = appoinmentId });
        }


        public IActionResult Remove(int id)
        {

            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    lstCartItems.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult AppoinmentConfirmation(int id)
        {
            shoppingCartVM.Appointments = _db.Appointments.Where(x => x.Id == id).FirstOrDefault();
            List<ProductSelectedForAppoinment> objProductList = _db.ProductSelectedForAppoinment.Where(p => p.AppoinmentId == id).ToList();

            foreach (ProductSelectedForAppoinment prodAptObj in objProductList)
            {
                shoppingCartVM.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());

            }

            return View(shoppingCartVM);
        }
    }
}
