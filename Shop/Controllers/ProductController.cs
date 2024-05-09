using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shop.Data;
using Shop.Models;
using Shop.Models.Entities;

namespace Shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ProductController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index() // Read
        {
            try {
                var products = await dbContext.Products.ToListAsync();
                return View(products);
            } catch (Exception ex)
            {
                // Take first 5 lines of error and log (for clarity)
                var exceptionMessageLines = ex.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var exceptionSummary = string.Join(Environment.NewLine, exceptionMessageLines.Take(5));

                Log.Error("Error in Product/Index:\n" + exceptionSummary + "\n\n", "An error occurred in Product/Index.");

                TempData["Message"] = ex.Message;
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Add() // Write
        {
            return View();
        }

        private bool ProductExists(long id)
        {
            return dbContext.Products.Any(e => e.ProductId == id);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel viewModel)
        {
            try
            {
                byte[]? imageData = null;

                using (var memoryStream = new MemoryStream())
                {
                    if (viewModel.Image is not null) {
                        await viewModel.Image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                if(ProductExists(viewModel.Id))
                {
                    TempData["message"] = "A product with same \"Product Id: " + viewModel.Id + " \" already exists, please add a new id or edit the existing one";
                    return View(viewModel);
                }

                var product = new Product
                {
                    ProductId = viewModel.Id,
                    Name = viewModel.Name,
                    SKU = viewModel.SKU,
                    Quantity = viewModel.Quantity,
                    Category = viewModel.Category,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Image = imageData,
                };

                await dbContext.Products.AddAsync(product);

                await dbContext.SaveChangesAsync();

                return RedirectToAction("Added");
            } catch (Exception ex)
            {                // Take first 5 lines of error and log (for clarity)
                var exceptionMessageLines = ex.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var exceptionSummary = string.Join(Environment.NewLine, exceptionMessageLines.Take(5));

                Log.Error("Error in Product/Add:\n" + exceptionSummary + "\n\n", "An error occurred in Product/Add.");

                TempData["Message"] = ex.Message.ToString();
            }
            return View();
        }

        [HttpGet]
        public IActionResult Added()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var product = await dbContext.Products.FindAsync(id);

            if (product is not null) {
                return View(product);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile imageFile)
        {
            try {

                var existingProduct = await dbContext.Products.FindAsync(product.Id);

                byte[]? imageData = null;

                using (var memoryStream = new MemoryStream())
                {
                    if (imageFile is not null)
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                if (existingProduct is not null)
                {
                    if (existingProduct.ProductId == product.ProductId || !ProductExists(product.ProductId) || product.ProductId == 0)
                    {
                        existingProduct.ProductId = product.ProductId != 0 ? product.ProductId : existingProduct.ProductId;
                        existingProduct.Name = product.Name is not null ? product.Name : existingProduct.Name;
                        existingProduct.SKU = product.SKU is not null ? product.SKU : existingProduct.SKU;
                        existingProduct.Quantity = product.Quantity != 0 ? product.Quantity : existingProduct.Quantity;
                        existingProduct.Category = product.Category is not null ? product.Category : existingProduct.Category;
                        existingProduct.Description = product.Description is not null ? product.Description : existingProduct.Description;
                        existingProduct.Price = product.Price != 0 ? product.Price : existingProduct.Price;
                        if (imageData is not null) { existingProduct.Image = imageData; }

                        await dbContext.SaveChangesAsync();

                        return RedirectToAction("Edited");
                    }
                    else
                    {
                        TempData["Message"] = "A product already exists with the same ProductId, Please choose a different product id.";
                        return View(product);
                    }
                }
                else
                {
                    TempData["Message"] = "Product does not exists, please add as a new one";
                    return View("Error");
                }
            } catch (Exception ex)
            {                
                // Take first 5 lines of error and log (for clarity)
                var exceptionMessageLines = ex.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var exceptionSummary = string.Join(Environment.NewLine, exceptionMessageLines.Take(5));

                Log.Error("Error in Product/Edit:\n" + exceptionSummary + "\n\n", "An error occurred in Product/Edit.");

                TempData["Message"] = ex.Message.ToString();
                return View(product);
            }
        }


        [HttpGet]
        public IActionResult Edited()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(Product product) 
        {
            var existingProduct = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (existingProduct is not null)
            {
                dbContext.Products.Remove(existingProduct);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            try {
                var existingProduct = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == Id);

                if (existingProduct is not null)
                {
                    dbContext.Products.Remove(existingProduct);
                    await dbContext.SaveChangesAsync();
                }
            } catch (Exception ex)
            {
                // Take first 5 lines of error and log (for clarity)
                var exceptionMessageLines = ex.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var exceptionSummary = string.Join(Environment.NewLine, exceptionMessageLines.Take(5));

                Log.Error("Error in Product/Delete:\n" + exceptionSummary + "\n\n", "An error occurred in Product/Delete.");

                TempData["Message"] = ex.Message.ToString();
            }
            return RedirectToAction("Index");
        }
    }
}
