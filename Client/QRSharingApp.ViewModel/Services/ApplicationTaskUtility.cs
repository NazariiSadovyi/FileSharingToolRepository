using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.Models;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.Services
{
    public class ApplicationTaskUtility : IApplicationTaskUtility
    {
        public ISubject<FetchDataInfo> FetchDataSubject { get; set; } = new Subject<FetchDataInfo>();
        public ISubject<InformationMessageInfo> InformationMessageSubject { get; set; } = new Subject<InformationMessageInfo>();

        public async Task ExecuteFetchDataAsync(Func<Task> action, string message = default, bool showAtLeastSecond = true, bool hideLoader = true)
        {
            await ExecuteFetchDataInternalAsync(action(), message, showAtLeastSecond, hideLoader);
        }

        public async Task<T> ExecuteFetchDataAsync<T>(Func<Task<T>> action, string message = default, bool showAtLeastSecond = true, bool hideLoader = true)
        {
            var mainActionTask = action();
            await ExecuteFetchDataInternalAsync(mainActionTask, message, showAtLeastSecond, hideLoader);

            return mainActionTask.Result;
        }

        public void ShowInformationMessage(string message, InformationKind kind = InformationKind.Success)
        {
            var informationMessageInfo = new InformationMessageInfo()
            {
                Message = message,
                Kind = kind
            };

            InformationMessageSubject.OnNext(informationMessageInfo);
        }

        private async Task ExecuteFetchDataInternalAsync(Task action, string message = default, bool showAtLeastSecond = true, bool hideLoader = true)
        {
            var fetchDataInfo = new FetchDataInfo()
            {
                ShowControl = true,
                Message = string.IsNullOrEmpty(message) ? CultureLocalization.Localization.GetResource("DialogTextFetching") : message
            };

            FetchDataSubject.OnNext(fetchDataInfo);

            var oneSecondTask = Task.Delay(1000);

            var tasksToWait = new Collection<Task>()
            {
                action
            };

            if (showAtLeastSecond)
            {
                tasksToWait.Add(oneSecondTask);
            }

            await Task.WhenAll(tasksToWait);

            if (!hideLoader)
            {
                return;
            }

            fetchDataInfo.ShowControl = false;
            FetchDataSubject.OnNext(fetchDataInfo);
        }
    }

}