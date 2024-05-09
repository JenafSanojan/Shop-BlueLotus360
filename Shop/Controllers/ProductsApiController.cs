using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shop.Data;
using Shop.Models;
using Shop.Models.Entities;

namespace Shop.Controllers
{
    public class ProductsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductsApi
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        // GET: ProductsApi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: ProductsApi/Ui   does not send an api but a html file
        public IActionResult Ui()
        {
            return View();
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // POST: ProductsApi/Create <- Product
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel viewModel)
        {
            try
            {
                byte[]? imageData = null;

                using (var memoryStream = new MemoryStream())
                {
                    if (viewModel.Image is not null)
                    {
                        await viewModel.Image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                if (ProductExists(viewModel.Id))
                {
                    return BadRequest("Product with same ProductID already exists");
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

                await _context.Products.AddAsync(product);

                await _context.SaveChangesAsync();

                return Ok("Product has been added");
            }
            catch (Exception ex)
            {                // Take first 5 lines of error and log (for clarity)
                var exceptionMessageLines = ex.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var exceptionSummary = string.Join(Environment.NewLine, exceptionMessageLines.Take(5));

                Log.Error("Error in Product/Add:\n" + exceptionSummary + "\n\n", "An error occurred in Product/Add.");

                return StatusCode(500, "Internal Error: \n" + exceptionSummary.ToString());
            }


        }


















        // PUT: ProductsApi/Edit/5
        [HttpPut]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Name,Category,Price,Quantity,SKU,Description,Image")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: ProductsApi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: ProductsApi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
