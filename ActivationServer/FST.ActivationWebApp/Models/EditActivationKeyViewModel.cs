using System;
using System.ComponentModel.DataAnnotations;

namespace FST.ActivationWebApp.Models
{
    public class EditActivationKeyViewModel
    {
        public Guid ProgramToolId { get; set; }
        public Guid Key { get; set; }
        [Required]
        public DateTime? ExpirationDate { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
