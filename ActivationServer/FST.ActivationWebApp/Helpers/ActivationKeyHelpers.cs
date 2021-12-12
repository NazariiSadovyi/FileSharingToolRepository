using FST.ActivationWebApp.Data.Entities;
using FST.ActivationWebApp.Models;
using System;

namespace FST.ActivationWebApp.Helpers
{
    public static class ActivationKeyHelpers
    {
        public static bool IsExpired(this ActivationKey entityKey)
        {
            return IsExpiredPrivate(entityKey.ActivationDate, entityKey.ExpirationDays);
        }

        public static bool IsExpired(this ActivationKeyViewModel viewModel)
        {
            return IsExpiredPrivate(viewModel.ActivationDate, viewModel.ExpirationDays);
        }

        private static bool IsExpiredPrivate(DateTime? activationDate, int expirationDays)
        {
            if (!activationDate.HasValue)
            {
                return false;
            }

            return activationDate.Value.AddDays(expirationDays).Date <= DateTime.Now.Date;
        }
    }
}
