using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FST.Client.Controls.Properties
{
    public class ButtonProperties
    {
        public static PackIconKind GetImage(DependencyObject obj)
        {
            return (PackIconKind)obj.GetValue(ImageProperty);
        }

        public static void SetImage(DependencyObject obj, PackIconKind value)
        {
            obj.SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached("Image", typeof(PackIconKind), typeof(ButtonProperties), new UIPropertyMetadata(null));
    }
}