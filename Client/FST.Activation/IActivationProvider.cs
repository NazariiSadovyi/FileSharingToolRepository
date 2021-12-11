﻿using System.Threading.Tasks;

namespace FST.Activation
{
    public interface IActivationProvider
    {
        Task<bool?> CheckAndSaveLicense(string key);
        Task<bool?> IsActivatedCheck();
        string GetSavedLicenseKey();
        Task DeactivateLicense();
    }
}