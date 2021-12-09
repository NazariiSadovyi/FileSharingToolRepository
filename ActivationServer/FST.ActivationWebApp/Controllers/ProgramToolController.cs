using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FST.ActivationWebApp.Data;
using FST.ActivationWebApp.Data.Entities;

namespace FST.ActivationWebApp.Controllers
{
    public class ProgramToolController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramToolController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProgramTool.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programTool = await _context.ProgramTool.FirstOrDefaultAsync(m => m.Id == id);
            if (programTool == null)
            {
                return NotFound();
            }

            return View(programTool);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ProgramTool programTool)
        {
            if (ModelState.IsValid)
            {
                programTool.Id = Guid.NewGuid();
                _context.Add(programTool);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(programTool);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programTool = await _context.ProgramTool.FindAsync(id);
            if (programTool == null)
            {
                return NotFound();
            }
            return View(programTool);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] ProgramTool programTool)
        {
            if (id != programTool.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(programTool);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgramToolExists(programTool.Id))
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
            return View(programTool);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programTool = await _context.ProgramTool.FirstOrDefaultAsync(m => m.Id == id);
            if (programTool == null)
            {
                return NotFound();
            }

            return View(programTool);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var programTool = await _context.ProgramTool.FindAsync(id);
            _context.ProgramTool.Remove(programTool);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProgramToolExists(Guid id)
        {
            return _context.ProgramTool.Any(e => e.Id == id);
        }
    }
}
