using QRSharingApp.ViewModel.Models;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.Interfaces
{
    public interface IApplicationTaskUtility
    {
        ISubject<FetchDataInfo> FetchDataSubject { get; }
        ISubject<InformationMessageInfo> InformationMessageSubject { get; }

        Task ExecuteFetchDataAsync(Func<Task> action, string message = null, bool showAtLeastSecond = true, bool hideLoader = true);
        Task<T> ExecuteFetchDataAsync<T>(Func<Task<T>> action, string message = null, bool showAtLeastSecond = true, bool hideLoader = true);
        void ShowInformationMessage(string message, InformationKind kind = InformationKind.Success);
    }
}