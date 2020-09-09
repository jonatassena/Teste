using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Teste.Data;
using Teste.Models;
using Microsoft.AspNetCore.Authorization;

namespace Teste.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        [Authorize(Roles = "CanGetProducts")]
        public async Task<IActionResult> Index()
        {
            _context.AuditLog.Add(
                new AuditLog
                {
                    UserEmail = User.Identity.Name,
                    AuditDetails = String.Concat("Accessed products list screen")
                }
            );

            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        [Authorize(Roles = "CanGetProducts")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.AuditLog.Add(
                new AuditLog
                {
                    UserEmail = User.Identity.Name,
                    AuditDetails = String.Concat("Accessed details screen of product: ", product.Name)
                }
            );
            _context.SaveChanges();

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "CanAddProduct")]
        public IActionResult Create()
        {
            _context.AuditLog.Add(
                new AuditLog
                {
                    UserEmail = User.Identity.Name,
                    AuditDetails = "Accessed create product screen"
                }
            );
            _context.SaveChanges();

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CanAddProduct")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                _context.AuditLog.Add(
                    new AuditLog
                    {
                        UserEmail = User.Identity.Name,
                        AuditDetails = String.Concat("Registered product: ", product.Name,
                        "| Created at: ", DateTime.Now.ToLongDateString())
                    }
                );
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            new AuditLog
            {
                UserEmail = User.Identity.Name,
                AuditDetails = String.Concat("Accessed screen to edit product: ", product.Name)
            };
            _context.SaveChanges();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Product product)
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

                    _context.AuditLog.Add(
                        new AuditLog
                        {
                            UserEmail = User.Identity.Name,
                            AuditDetails = String.Concat("Edited product: ", product.Name,
                            "| Updated at: ", DateTime.Now.ToLongDateString())
                        }
                    );
                    _context.SaveChanges();
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

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            _context.AuditLog.Add(
                new AuditLog
                {
                    UserEmail = User.Identity.Name,
                    AuditDetails = String.Concat("Deleted product: ", product.Name,
                    "| Deleted at: ", DateTime.Now.ToLongDateString())
                }
            );
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
