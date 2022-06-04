using DynamicData;
using QRSharingApp.ViewModel.Helpers;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive;
using System.Windows;
using System.Windows.Input;

namespace QRSharingApp.ViewModel.ViewModels.Base
{
    public class PageRequestViewModel : ViewModelBase, IPageRequest, IEquatable<IPageRequest>
    {
        private readonly ICollection _itemsCollection;

        public int Page { get; set; }
        public int Size { get; set; }

        public ICommand ChangePageCommand { get; set; }
        public ICommand ChangeSizeCommand { get; set; }
        public ICommand SwitchFilePageCommand { get; set; }

        public PageRequestViewModel(int page, int size, ICollection itemsCollection)
        {
            _itemsCollection = itemsCollection;
            Page = page;
            Size = size;
            ChangePageCommand = ReactiveCommand.Create<int?>(ChangePage);
            ChangeSizeCommand = ReactiveCommand.Create<int?>(ChangeSize);
            SwitchFilePageCommand = ReactiveCommand.Create<string>(SwitchFilePage);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IPageRequest);
        }

        public override int GetHashCode()
        {
            return Page.GetHashCode() + Size.GetHashCode();
        }

        public bool Equals(IPageRequest other)
        {
            return false;
        }

        public IObservable<Unit> WhenPageOrSizeChanged()
        {
            return this.WhenAnyValue(_ => _.Page, _ => _.Size).IgnoreValue();
        }

        public void NextPage()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Page + 1 > PageCount())
                {
                    Page = 1;
                    return;
                }
                Page++;
            });
        }

        public int PageCount()
        {
            var itemsCount = _itemsCollection.Count;
            return itemsCount == itemsCount / Size * Size
                ? itemsCount / Size
                : itemsCount / Size + 1;
        }

        public void PreviousPage()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Page - 1 == 0)
                {
                    Page = PageCount();
                    return;
                }
                Page--;
            });
        }

        private void ChangePage(int? page)
        {
            Page = page.GetValueOrDefault();
        }

        private void ChangeSize(int? size)
        {
            Size = size.GetValueOrDefault();
        }

        private void SwitchFilePage(string action)
        {
            switch (action)
            {
                case "left":
                    PreviousPage();
                    break;
                case "right":
                    NextPage();
                    break;
                default:
                    break;
            }
        }
    }
}
