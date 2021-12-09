using System;

namespace FST.ActivationWebApp.Data.Entities
{
    public class ProgramUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MachineId { get; set; }

        public Guid ActivationKeyId { get; set; }
        public ActivationKey ActivationKey { get; set; }
    }
}
