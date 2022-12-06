using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebShopMVC.Data;
using WebShopMVC.Models.ViewModel;
using WebShopMVC.Utility;

namespace WebShopMVC.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        private ProductsViewModel productsVM { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            productsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Models.Products(),
            };

        }
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return View(productsVM);
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        //ProductsViewModel productsVM
        public async Task<IActionResult> CreatePOST(ProductsViewModel productsVM) 
        {
            if (!ModelState.IsValid)
            {
                return View(productsVM);
            }

            
            _db.Products.Add(productsVM.Products);
            
            await _db.SaveChangesAsync();
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var productsFromDb = _db.Products.Find(productsVM.Products.Id);
            if (files.Count != 0)
            {
                //Image has been uploaded

                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extensions = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads,productsVM.Products.Id+extensions),FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + extensions;

            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + ".png");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + ".png";
              
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
