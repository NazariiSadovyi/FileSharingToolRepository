using System.Windows;
using System.Windows.Controls;

namespace QRSharingApp.Client.Controls
{
    /// <summary>
    /// Interaction logic for TaskLoadingControl.xaml
    /// </summary>
    public partial class TaskLoadingControl : UserControl
    {
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(TaskLoadingControl), new PropertyMetadata(false));
        
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(TaskLoadingControl), new PropertyMetadata(string.Empty));

        public TaskLoadingControl()
        {
            InitializeComponent();
        }
    }
}
