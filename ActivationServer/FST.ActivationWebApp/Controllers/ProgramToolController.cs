using FST.ActivationWebApp.Data;
using FST.ActivationWebApp.Data.Entities;
using FST.ActivationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FST.ActivationWebApp.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Create([Bind("Id,Name")] ProgramToolViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var nameIsUnique = (await _context.ProgramTool.FirstOrDefaultAsync(_ => _.Name == viewModel.Name)) == null;
                if (!nameIsUnique)
                {
                    ModelState.TryAddModelError(nameof(ProgramToolViewModel.Name), "Name is not unique");
                    return View(viewModel);
                }

                viewModel.Id = Guid.NewGuid();
                _context.Add(new ProgramTool()
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name
                });
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
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

            return View(new ProgramToolViewModel()
            {
                Id = programTool.Id,
                Name = programTool.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] ProgramToolViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nameIsUnique = (await _context.ProgramTool.FirstOrDefaultAsync(_ => _.Name == viewModel.Name)) == null;
                    if (!nameIsUnique)
                    {
                        ModelState.TryAddModelError(nameof(ProgramToolViewModel.Name), "Name is not unique");
                        return View(viewModel);
                    }

                    var programTool = await _context.ProgramTool.FindAsync(viewModel.Id);
                    programTool.Name = viewModel.Name;
                    _context.Update(programTool);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgramToolExists(viewModel.Id))
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
            return View(viewModel);
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
