using System;
using System.ComponentModel.DataAnnotations;

namespace FST.ActivationWebApp.Models
{
    public class CreateActivationKeyViewModel
    {
        public Guid ProgramToolId { get; set; }
        public Guid Key { get; set; }
        [Required]
        public int ExpirationDays { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
