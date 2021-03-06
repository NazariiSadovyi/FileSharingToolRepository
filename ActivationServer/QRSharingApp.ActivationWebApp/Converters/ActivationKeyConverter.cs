using QRSharingApp.ActivationWebApp.Data.Entities;
using QRSharingApp.ActivationWebApp.Helpers;
using QRSharingApp.ActivationWebApp.Models;
using System;

namespace QRSharingApp.ActivationWebApp.Converters
{
    public static class ActivationKeyConverter
    {
        public static ActivationKey ToEntity(this CreateActivationKeyViewModel viewModel)
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
            };

            return activationKey;
        }

        public static ActivationKeyDetailViewModel ToDetailViewModel(this ActivationKey entity)
        {
            return new ActivationKeyDetailViewModel()
            {
                Id = entity.Id,
                Key = entity.Key,
                ActivationDate = entity.ActivationDate,
                ExpirationDays = entity.ExpirationDays,
                UserEmail = entity.ProgramUser.Email,
                UserName = entity.ProgramUser.Name,
                MachineId = entity.ProgramUser.MachineId,
                CreateDate = entity.CreateDate,
                ProgramToolId = entity.ProgramToolId
            };
        }

        public static ActivationKeyViewModel ToViewModel(this ActivationKey entity)
        {
            return new ActivationKeyViewModel()
            {
                Id = entity.Id,
                Key = entity.Key,
                ActivationDate = entity.ActivationDate,
                ExpirationDays = entity.ExpirationDays,
                UserEmail = entity.ProgramUser.Email,
                UserName = entity.ProgramUser.Name,
                MachineId = entity.ProgramUser.MachineId,
                CreateDate = entity.CreateDate,
            };
        }

        public static EditActivationKeyViewModel ToEditViewModel(this ActivationKey entity)
        {
            return new EditActivationKeyViewModel()
            {
                Id = entity.Id,
                Key = entity.Key,
                ProgramToolId = entity.ProgramToolId,
                UserEmail = entity.ProgramUser.Email,
                UserName = entity.ProgramUser.Name,
                ExpireAfter = entity.GetExpiredAfter()
            };
        }
    }
}
