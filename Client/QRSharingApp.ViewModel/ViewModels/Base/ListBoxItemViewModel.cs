namespace QRSharingApp.ViewModel.ViewModels.Base
{
    public class ListBoxItemViewModel : ViewModelBase
    {
        public ListBoxItemViewModel(int id, string displayText, bool isSelected = false)
        {
            Id = id;
            DisplayText = displayText;
            IsSelected = isSelected;
        }

        public int Id { get; set; }
        public string DisplayText { get; set; }
        public bool IsSelected { get; set; }
    }
}
