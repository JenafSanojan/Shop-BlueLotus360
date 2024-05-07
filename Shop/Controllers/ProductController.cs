using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var products = await dbContext.Products.ToListAsync();

            return View(products);
        }

        [HttpGet]
        public IActionResult Add() // Write
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Added(AddProductViewModel viewModel)
        {
            var product = new Product
            {
                ProductId = viewModel.Id,
                Name = viewModel.Name,
                SKU = viewModel.SKU,
                Quantity = viewModel.Quantity,
                Category = viewModel.Category,
                Description = viewModel.Description,
                Price = viewModel.Price,
            };

            await dbContext.Products.AddAsync(product);

            await dbContext.SaveChangesAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var product = await dbContext.Products.FindAsync(id);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edited(Product product) 
        {
            var existingProduct = await dbContext.Products.FindAsync(product.Id);

            if (existingProduct is not null)
            {
                existingProduct.ProductId = product.ProductId;
                existingProduct.Name = product.Name;
                existingProduct.SKU = product.SKU;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Category = product.Category;
                existingProduct.Description = product.Description;  
                existingProduct.Price = product.Price;

                await dbContext.SaveChangesAsync();

                return View();
            }
            else
            {
                return View("Error");
            }
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
            var existingProduct = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (existingProduct is not null)
            {
                dbContext.Products.Remove(existingProduct);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
