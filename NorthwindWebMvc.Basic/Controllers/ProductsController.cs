using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NorthwindWebMvc.Basic.Models;
using NorthwindWebMvc.Basic.Models.Dto;
using NorthwindWebMvc.Basic.RepositoryContext;
using NorthwindWebMvc.Basic.Service;
using Microsoft.AspNetCore.Localization;

namespace NorthwindWebMvc.Basic.Controllers
{
    public class ProductsController : Controller
    {
        private readonly RepositoryDbContext _context;
        private readonly IProductService<ProductDto> _productService;
        private readonly ICategoryService<CategoryDto> _categoryService;

        public ProductsController(RepositoryDbContext context, IProductService<ProductDto> productService, ICategoryService<CategoryDto> categoryService)
        {
            //_context = context;
            _productService = productService;
            _categoryService = categoryService;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            var allProduct=await _productService.FindAll(false);

            foreach(var item in allProduct)
            {
                item.Category = await _categoryService.FindById(item.CategoryId,false);
            }
            return View(allProduct);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.FindAll(true).Result.FirstOrDefault(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.Category = await _categoryService.FindById(product.CategoryId,false);
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var allCategory=await _categoryService.FindAll(false);
            ViewData["CategoryId"] = new SelectList(allCategory, "Id", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,Price,Stock,Photo,CategoryId")] ProductDtoCreate productDtoCreate)
        {
            if (!ModelState.IsValid)
            {

                try
                {
                    var file = productDtoCreate.Photo;
                    var folderName = Path.Combine("Resources", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        //collect data from dto dan filename
                        var productDto = new ProductDto()
                        {
                            ProductName=productDtoCreate.ProductName,
                            Price = productDtoCreate.Price,
                            Stock = productDtoCreate.Stock,
                            CategoryId=productDtoCreate.CategoryId,
                            Photo = fileName
                        };
                        _productService.Create(productDto);

                        return RedirectToAction(nameof(Index));

                    }
                }
                catch (Exception)
                {

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productDtoCreate);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.FindById((int)id, true);
            var productDtoCreate = new ProductDtoCreate

            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price= product.Price,
                Stock = product.Stock,
                CategoryId=product.CategoryId,
            };
            var allCategory = await _categoryService.FindAll(false);
            ViewData["CategoryId"] = new SelectList(allCategory, "Id", "CategoryName", product.CategoryId);
            return View(productDtoCreate);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Price,Stock,Photo,CategoryId")] ProductDtoCreate productDtoCreate)
        {
            if (id != productDtoCreate.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var file = productDtoCreate.Photo;
                    var folderName = Path.Combine("Resources", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        //collect data from dto dan filename
                        var productDto = new ProductDto
                        {
                            Id = productDtoCreate.Id,
                            ProductName = productDtoCreate.ProductName,
                            Price = productDtoCreate.Price,
                            Stock = productDtoCreate.Stock,
                            CategoryId = productDtoCreate.CategoryId,
                            Photo = fileName
                        };
                        _productService.Update(productDto);

                        return RedirectToAction(nameof(Index));

                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(productDtoCreate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productDtoCreate);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _productService.FindAll(true).Result.FirstOrDefault(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var allCategory = await _categoryService.FindAll(false);
            ViewData["CategoryId"] = new SelectList(allCategory, "Id", "CategoryName");
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return Problem("Entity set 'RepositoryDbContext.Products'  is null.");
            }
            //var category = await _categoryService.FindById((int)id,true);
            var product = _productService.FindAll(true).Result.FirstOrDefault(m => m.Id == id);
            if (product != null)
            {
                _productService.Delete(product);
            }

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            IEnumerable<ProductDto> prod = await _productService.FindAll(false);
          return (prod?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
