using System;

namespace QRSharingApp.ActivationWebApp.Data.Entities
{
    public class ActivationKey
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ExpirationDays { get; set; }

        public Guid ProgramUserId { get; set; }
        public ProgramUser ProgramUser { get; set; }

        public Guid ProgramToolId { get; set; }
        public ProgramTool ProgramTool { get; set; }
    }
}
