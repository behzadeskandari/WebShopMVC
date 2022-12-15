using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebShopMVC.Data;
using WebShopMVC.Models;
using WebShopMVC.Models.ViewModel;
using WebShopMVC.Utility;

namespace WebShopMVC.Controllers
{

    [Authorize(Roles = SD.SuperAdminEndUser)]
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

        public IActionResult Create()
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


        //Get :: EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id ==null)
            {
                return NotFound();
            }
            productsVM.Products = await _db.Products.Include(x => x.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(x => x.Id == id);

            if (productsVM.Products == null)
            {
                return NotFound();
            }
            return View(productsVM);

        }


        //post :EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProductsViewModel productsVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.Id == productsVM.Products.Id).FirstOrDefault();
                //files[0].Length
                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads,productsVM.Products.Id+extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, productsVM.Products.Id + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, productsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    productsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + extension_new;

                }

                if (productsVM.Products.Image != null)
                {
                    productFromDb.Image = productsVM.Products.Image;
                }

                productFromDb.Name = productsVM.Products.Name;
                productFromDb.Price = productsVM.Products.Price;
                productFromDb.Available = productsVM.Products.Available;
                productFromDb.ProductTypeId = productsVM.Products.ProductTypeId;
                productFromDb.SpecialTagId = productsVM.Products.SpecialTagId;
                productFromDb.ShadeColor = productsVM.Products.ShadeColor;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(productsVM);
        }


        //Get :: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            productsVM.Products = await _db.Products.Include(x => x.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(x => x.Id == id);

            if (productsVM.Products == null)
            {
                return NotFound();
            }
            return View(productsVM);

        }

        //Get :: Details
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            productsVM.Products = await _db.Products.Include(x => x.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(x => x.Id == id);

            if (productsVM.Products == null)
            {
                return NotFound();
            }
            return View(productsVM);

        }


        ///POST : Delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);
                if (System.IO.File.Exists(Path.Combine(uploads,products.Id+extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
                }
                
                _db.Products.Remove(products);

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
