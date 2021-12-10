﻿using FST.ActivationWebApp.Data.Entities;
using System;
using System.Collections.Generic;

namespace FST.ActivationWebApp.Models
{
    public class IndexActivationKeyViewModel
    {
        public string ProgramToolName { get; set; }
        public Guid ProgramToolId { get; set; }
        public List<ActivationKeyViewModel> ActivationKeys { get; set; }

        public IndexActivationKeyViewModel()
        {
            ActivationKeys = new List<ActivationKeyViewModel>();
        }
    }
}
