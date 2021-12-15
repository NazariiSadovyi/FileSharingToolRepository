using QRSharingApp.ActivationWebApp.Helpers;
using System;
using System.ComponentModel;

namespace QRSharingApp.ActivationWebApp.Models
{
    public class ActivationKeyViewModel
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        [DisplayName("Activation Date")]
        public DateTime? ActivationDate { get; set; }
        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDate { get; set; }
        public int ExpirationDays { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("User Email")]
        public string UserEmail { get; set; }
        [DisplayName("Machine Id")]
        public string MachineId { get; set; }
        
        public ActivationKeyState State
        {
            get
            {
                if (!ActivationDate.HasValue)
                {
                    return ActivationKeyState.NotUsed;
                }
                
                if (this.IsExpired())
                {
                    return ActivationKeyState.Expired;
                }

                var isInUse = !string.IsNullOrEmpty(MachineId);
                return isInUse ? ActivationKeyState.InUse : ActivationKeyState.NotUsed;
            }
        }

        [DisplayName("Expire After")]
        public int ExpireAfter
        {
            get
            {
                if (!ActivationDate.HasValue)
                {
                    return ExpirationDays;
                }

                if (this.IsExpired())
                {
                    return 0;
                }

                return (ActivationDate.Value.AddDays(ExpirationDays).Date - DateTime.Now.Date).Days;
            }
        }
    }
}
