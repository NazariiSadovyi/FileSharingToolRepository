using System.ComponentModel.DataAnnotations;

namespace QRSharingApp.DataAccess.Entities
{
    public class HotFolder
    {
        [Key]
        public int Id { get; set; }
        public string FolderPath { get; set; }
    }
}
