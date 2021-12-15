using System;
using System.Collections.Generic;

namespace QRSharingApp.ActivationWebApp.Data.Entities
{
    public class ProgramTool
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<ActivationKey> ActivationKeys { get; set; }
    }
}
