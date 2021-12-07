using System;

namespace FST.ActivationWebApp.Models
{
    public class CreateActivationViewModel
    {
        public Guid Key { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
    }
}
