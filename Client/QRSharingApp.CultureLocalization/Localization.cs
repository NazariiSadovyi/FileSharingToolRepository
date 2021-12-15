using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace QRSharingApp.CultureLocalization
{
    public static class Localization
    {
        public static string GetResource(string key)
        {
            var dict = new ResourceDictionary();
            var cultureName = Thread.CurrentThread.CurrentUICulture.Name;
            switch (cultureName)
            {
                case "ru-RU":
                    dict.Source = new Uri(string.Format("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.{0}.xaml", cultureName));
                    break;
                case "es-ES":
                    dict.Source = new Uri(string.Format("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.{0}.xaml", cultureName));
                    break;
                default:
                    dict.Source = new Uri("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.xaml");
                    break;
            }

            return dict[key] as string;
        }

        public static CultureInfo Language
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                Thread.CurrentThread.CurrentUICulture = value;

                //2. Создаём ResourceDictionary для новой культуры
                var dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        dict.Source = new Uri(string.Format("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.{0}.xaml", value.Name));
                        break;
                    case "es-ES":
                        dict.Source = new Uri(string.Format("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.{0}.xaml", value.Name));
                        break;
                    default:
                        dict.Source = new Uri("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.xaml");
                        break;
                }

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                var oldDict = (from d in Application.Current.Resources.MergedDictionaries
                               where d.Source != null && d.Source.OriginalString
                                 .StartsWith("pack://application:,,,/QRSharingApp.CultureLocalization;component/Resources/lang.")
                               select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
            }
        }
    }
}