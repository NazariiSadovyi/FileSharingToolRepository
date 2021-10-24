using System.ComponentModel.DataAnnotations;

namespace FST.DataAccess.Entities
{
    public class Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
