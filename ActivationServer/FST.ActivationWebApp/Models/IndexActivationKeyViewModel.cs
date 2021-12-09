using FST.ActivationWebApp.Data.Entities;
using System;
using System.Collections.Generic;

namespace FST.ActivationWebApp.Models
{
    public class IndexActivationKeyViewModel
    {
        public string ProgramToolName { get; set; }
        public Guid ProgramToolId { get; set; }
        public List<ActivationKey> ActivationKeys { get; set; }

        public IndexActivationKeyViewModel()
        {
            ActivationKeys = new List<ActivationKey>();
        }
    }
}
