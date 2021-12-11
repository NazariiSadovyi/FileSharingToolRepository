using System;
using System.ComponentModel;

namespace FST.ActivationWebApp.Models
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
                
                var isInUse = !string.IsNullOrEmpty(MachineId);
                var isExpired = ActivationDate.Value.AddDays(ExpirationDays).Date <= DateTime.Now.Date;

                if (isExpired)
                {
                    return ActivationKeyState.Expired;
                }

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

                var isExpired = ActivationDate.Value.AddDays(ExpirationDays).Date <= DateTime.Now.Date;
                if (isExpired)
                {
                    return 0;
                }

                return (ActivationDate.Value.AddDays(ExpirationDays).Date - DateTime.Now.Date).Days;
            }
        }
    }
}
