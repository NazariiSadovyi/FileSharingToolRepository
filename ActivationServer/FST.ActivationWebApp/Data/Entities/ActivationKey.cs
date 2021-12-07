using System;

namespace FST.ActivationWebApp.Data.Entities
{
    public class ActivationKey
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public ProgramUser ProgramUser { get; set; }
    }
}
