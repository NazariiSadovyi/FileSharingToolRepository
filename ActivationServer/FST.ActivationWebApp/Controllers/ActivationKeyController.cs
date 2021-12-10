using FST.ActivationWebApp.Converters;
using FST.ActivationWebApp.Data;
using FST.ActivationWebApp.Data.Entities;
using FST.ActivationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FST.ActivationWebApp.Controllers
{
    [Authorize]
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
            var activationKeyViewModels = programTool.ActivationKeys?.Select(_ => _.ToViewModel()).ToList();
            var viewModel = new IndexActivationKeyViewModel()
            {
                ProgramToolId = programToolId,
                ProgramToolName = (await _context.ProgramTool.FindAsync(programToolId)).Name,
                ActivationKeys = activationKeyViewModels ?? new List<ActivationKeyViewModel>()
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

            return View(activationKey.ToDetailViewModel());
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
                _context.Add(viewModel.ToEntity());
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

            var activationKey = await _context.ActivationKey
                .Include(_ => _.ProgramUser)
                .FirstOrDefaultAsync(_ => _.Id == id);
            if (activationKey == null)
            {
                return NotFound();
            }
            
            return View(activationKey.ToEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditActivationKeyViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var activationKey = await _context.ActivationKey
                        .Include(a => a.ProgramUser)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    activationKey.ProgramUser.Name = viewModel.UserName;
                    activationKey.ProgramUser.Email = viewModel.UserEmail;
                    if (activationKey.ActivationDate.HasValue)
                    {
                        var needDays = (activationKey.ActivationDate.Value.AddDays(activationKey.ExpirationDays).Date - DateTime.Now.Date).Days;
                        var pastDays = activationKey.ExpirationDays - needDays;
                        activationKey.ExpirationDays = pastDays + viewModel.ExpireAfter;
                    }
                    else
                    {
                        activationKey.ExpirationDays = viewModel.ExpireAfter;
                    }
                    _context.Update(activationKey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivationKeyExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { programToolId = viewModel.ProgramToolId });
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

            var activationKey = await _context.ActivationKey
                .Include(a => a.ProgramUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activationKey == null)
            {
                return NotFound();
            }

            return View(activationKey.ToDetailViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid programToolId)
        {
            var activationKey = await _context.ActivationKey.FindAsync(id);
            _context.ActivationKey.Remove(activationKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { programToolId = programToolId });
        }

        private bool ActivationKeyExists(Guid id)
        {
            return _context.ActivationKey.Any(e => e.Id == id);
        }
    }
}
