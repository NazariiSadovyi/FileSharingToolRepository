﻿using FST.ActivationWebApp.ApiResponses;
using FST.ActivationWebApp.Data;
using FST.ActivationWebApp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FST.ActivationWebApp.Controllers
{
    [ApiController]
    public class ActivationApiController : ControllerBase
    {
        private ApplicationDbContext _applicationDbContext { get; set; }

        public ActivationApiController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet("api/activation/programTool/{programToolId}/key/{key}/machine/{machineId}/activate")]
        public async Task<IActionResult> Activate(Guid programToolId, Guid key, string machineId)
        {
            var response = new ActivationStatusResponse();
            var currentKey = await _applicationDbContext.ActivationKey
                .Include(_ => _.ProgramUser)
                .FirstOrDefaultAsync(_ => _.ProgramToolId == programToolId && _.Key == key
                    && string.IsNullOrEmpty(_.ProgramUser.MachineId));

            if (currentKey == null)
            {
                response.State = ActivationKeyStateEnum.Incorrect;
            }
            else
            {
                if (IsActivationKeyExpired(currentKey))
                {
                    response.State = ActivationKeyStateEnum.Expired;
                }
                else
                {
                    currentKey.ProgramUser.MachineId = machineId;
                    currentKey.ActivationDate = DateTime.Now;
                    await _applicationDbContext.SaveChangesAsync();
                    response.State = ActivationKeyStateEnum.Correct;
                }
            }

            return new JsonResult(response);
        }

        [HttpGet("api/activation/programTool/{programToolId}/key/{key}/machine/{machineId}/check")]
        public async Task<IActionResult> Check(Guid programToolId, Guid key, string machineId)
        {
            var response = new ActivationStatusResponse();
            var currentKey = await _applicationDbContext.ActivationKey
                .Include(_ => _.ProgramUser)
                .FirstOrDefaultAsync(_ => _.ProgramToolId == programToolId && _.Key == key
                    && _.ProgramUser.MachineId == machineId && _.ActivationDate != null);
            if (currentKey == null)
            {
                response.State = ActivationKeyStateEnum.Incorrect;
            }
            else
            {
                response.State = IsActivationKeyExpired(currentKey) ? ActivationKeyStateEnum.Expired : ActivationKeyStateEnum.Correct;
            }
            
            return new JsonResult(response);
        }

        [HttpGet("api/activation/programTool/{programToolId}/key/{key}/machine/{machineId}/reset")]
        public async Task<IActionResult> Reset(Guid programToolId, Guid key, string machineId)
        {
            var currentKey = await _applicationDbContext.ActivationKey
                .Include(_ => _.ProgramUser)
                .FirstOrDefaultAsync(_ => _.ProgramToolId == programToolId && _.Key == key
                    && _.ProgramUser.MachineId == machineId && _.ActivationDate != null);

            if (currentKey != null)
            {
                currentKey.ProgramUser.MachineId = string.Empty;
                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        private bool IsActivationKeyExpired(ActivationKey key)
        {
            return key.ActivationDate.Value.AddDays(key.ExpirationDays).Date >= DateTime.Now.Date;
        }
    }
}
