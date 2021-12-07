namespace FST.WebApplication.Models
{
    public class CountryViewModel
    {
        public string DisplayName => $"{Name} (+{CallingCode})";
        public string Name { get; set; }
        public string Code { get; set; }
        public string CallingCode { get; set; }
    }
}
