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

        [HttpGet]
        public async Task<IActionResult> Index(Guid programToolId)
        {
            var programTool = await _context.ProgramTool
                .Include(_ => _.ActivationKeys)
                .ThenInclude(_ => _.ProgramUser)
                .FirstOrDefaultAsync(_ => _.Id == programToolId);
            var viewModel = new IndexActivationKeyViewModel()
            {
                ProgramToolId = programToolId,
                ProgramToolName = (await _context.ProgramTool.FindAsync(programToolId)).Name,
                ActivationKeys = programTool.ActivationKeys ?? new List<ActivationKey>()
            };

            return View(viewModel);
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult Create(Guid programToolId)
        {
            return View(new CreateActivationKeyViewModel()
            { 
                ProgramToolId = programToolId,
                Key = Guid.NewGuid()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateActivationKeyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newUserId = Guid.NewGuid();
                var activationKey = new ActivationKey() 
                {
                    Id = Guid.NewGuid(),
                    Key = viewModel.Key,
                    CreateDate = DateTime.Now,
                    ExpirationDays = viewModel.ExpirationDays,
                    ProgramUserId = newUserId,
                    ProgramUser = new ProgramUser()
                    {
                        Id = newUserId,
                        Email = viewModel.UserEmail,
                        Name = viewModel.UserName
                    },
                    ProgramToolId = viewModel.ProgramToolId,
                    ProgramTool = _context.ProgramTool.Find(viewModel.ProgramToolId)
                };
                _context.Add(activationKey);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { programToolId = viewModel.ProgramToolId });
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

            var activationKey = await _context.ActivationKey.FindAsync(id);
            if (activationKey == null)
            {
                return NotFound();
            }
            
            return View(activationKey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ActivationKey activationKey)
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
            
            return View(activationKey);
        }

        [HttpGet]
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
