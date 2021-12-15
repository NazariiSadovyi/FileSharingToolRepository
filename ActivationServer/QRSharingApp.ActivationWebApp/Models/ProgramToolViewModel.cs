using System;
using System.ComponentModel.DataAnnotations;

namespace QRSharingApp.ActivationWebApp.Models
{
    public class ProgramToolViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
