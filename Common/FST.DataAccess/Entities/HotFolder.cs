using System.ComponentModel.DataAnnotations;

namespace FST.DataAccess.Entities
{
    public class HotFolder
    {
        [Key]
        public int Id { get; set; }
        public string FolderPath { get; set; }
    }
}
