using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FST.ActivationWebApp.Data;
using FST.ActivationWebApp.Data.Entities;
using FST.ActivationWebApp.Models;

namespace FST.ActivationWebApp.Controllers
{
    public class ActivationKeyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivationKeyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActivationKey
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActivationKey.Include(a => a.ProgramUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ActivationKey/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activationKey = await _context.ActivationKey
                .Include(a => a.ProgramUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activationKey == null)
            {
                return NotFound();
            }

            return View(activationKey);
        }

        // GET: ActivationKey/Create
        public IActionResult Create()
        {
            return View(new CreateActivationViewModel());
        }

        // POST: ActivationKey/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateActivationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var activationKey = new ActivationKey() 
                {
                    Id = Guid.NewGuid(),
                    Key = viewModel.Key,
                    CreateDate = DateTime.Now,
                    ExpirationDate = viewModel.ExpirationDate,
                    ProgramUser = new ProgramUser()
                    {
                       Email = viewModel.UserEmail,
                       Name = viewModel.UserName
                    }
                };
                _context.Add(activationKey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(viewModel);
        }

        // GET: ActivationKey/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activationKey = await _context.ActivationKey.FindAsync(id);
            if (activationKey == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.ProgramUser, "Id", "Id", activationKey.Id);
            return View(activationKey);
        }

        // POST: ActivationKey/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Key,CreateDate,ActivationDate,ExpirationDate")] ActivationKey activationKey)
        {
            if (id != activationKey.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activationKey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivationKeyExists(activationKey.Id))
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
            ViewData["Id"] = new SelectList(_context.ProgramUser, "Id", "Id", activationKey.Id);
            return View(activationKey);
        }

        // GET: ActivationKey/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activationKey = await _context.ActivationKey
                .Include(a => a.ProgramUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activationKey == null)
            {
                return NotFound();
            }

            return View(activationKey);
        }

        // POST: ActivationKey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var activationKey = await _context.ActivationKey.FindAsync(id);
            _context.ActivationKey.Remove(activationKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivationKeyExists(Guid id)
        {
            return _context.ActivationKey.Any(e => e.Id == id);
        }
    }
}
