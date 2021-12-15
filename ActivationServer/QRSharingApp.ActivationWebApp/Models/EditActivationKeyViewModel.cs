using System;
using System.ComponentModel.DataAnnotations;

namespace QRSharingApp.ActivationWebApp.Models
{
    public class EditActivationKeyViewModel
    {
        public Guid Id { get; set; }
        public Guid ProgramToolId { get; set; }
        public Guid Key { get; set; }
        [Required]
        public int ExpireAfter { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
