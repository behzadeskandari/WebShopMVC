using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebShopMVC.Data;
using WebShopMVC.Models;

namespace WebShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ProductTypes.ToList());
        }

        //Get Create Action Method 
        public IActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// Create Post Action Method
        /// </summary>
        /// <param name="productTypes"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Add(productTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTypes = await _db.ProductTypes.FindAsync(id);
            if (productTypes == null)
            {
                return NotFound();
            }

            return View(productTypes);
        }


        /// <summary>
        /// Create Post Action Method
        /// </summary>
        /// <param name="productTypes"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProductTypes productTypes)
        {
            if (id != productTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(productTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTypes = await _db.ProductTypes.FindAsync(id);
            if (productTypes == null)
            {
                return NotFound();
            }

            return View(productTypes);
        }


        public async Task<IActionResult> Delete(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            var ProductTypes = await _db.ProductTypes.FindAsync(id);
            if (ProductTypes == null)
            {
                return NotFound();
            }

            return View(ProductTypes);
        }


        /// <summary>
        /// Create Post Action Method
        /// </summary>
        /// <param name="productTypes"></param>
        /// <returns></returns>
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {


                var productTypes = await _db.ProductTypes.FindAsync(id);
            
                 _db.ProductTypes.Remove(productTypes);

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

    }
}
