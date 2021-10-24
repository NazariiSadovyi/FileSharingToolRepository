using System;

namespace FST.Infrastructure.Models
{
    public class LocalFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsVideo { get; set; }
        public bool IsPhoto { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
