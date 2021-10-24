using System.ComponentModel.DataAnnotations;

namespace FST.DataAccess.Entities
{
    public class LocalFile
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
