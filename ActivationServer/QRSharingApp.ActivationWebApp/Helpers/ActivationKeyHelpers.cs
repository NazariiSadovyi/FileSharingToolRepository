using QRSharingApp.ActivationWebApp.Data.Entities;
using QRSharingApp.ActivationWebApp.Models;
using System;

namespace QRSharingApp.ActivationWebApp.Helpers
{
    public static class ActivationKeyHelpers
    {
        public static bool IsExpired(this ActivationKey entityKey)
        {
            return IsExpiredPrivate(entityKey.CreateDate, entityKey.ExpirationDays);
        }

        public static bool IsExpired(this ActivationKeyViewModel viewModel)
        {
            return IsExpiredPrivate(viewModel.CreateDate, viewModel.ExpirationDays);
        }

        private static bool IsExpiredPrivate(DateTime createDate, int expirationDays)
        {
            return createDate.AddDays(expirationDays).Date <= DateTime.Now.Date;
        }

        public static int GetExpiredAfter(this ActivationKey entityKey)
        {
            if (entityKey.IsExpired())
            {
                return 0;
            }

            return GetExpiredAfterPrivate(entityKey.CreateDate, entityKey.ExpirationDays);
        }

        public static int GetExpiredAfter(this ActivationKeyViewModel viewModel)
        {
            if (viewModel.IsExpired())
            {
                return 0;
            }

            return GetExpiredAfterPrivate(viewModel.CreateDate, viewModel.ExpirationDays);
        }

        private static int GetExpiredAfterPrivate(DateTime createDate, int expirationDays)
        {
            return (createDate.AddDays(expirationDays).Date - DateTime.Now.Date).Days;
        }
    }
}
