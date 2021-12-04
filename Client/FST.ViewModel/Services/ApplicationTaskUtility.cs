using FST.ViewModel.Interfaces;
using FST.ViewModel.Models;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FST.ViewModel.Services
{
    public class ApplicationTaskUtility : IApplicationTaskUtility
    {
        private readonly CompositeCommand _fetchDataCommand = new CompositeCommand(true);
        public CompositeCommand FetchDataCommand
        {
            get { return _fetchDataCommand; }
        }

        private readonly CompositeCommand _informationMessageCommand = new CompositeCommand(true);
        public CompositeCommand InformationMessageCommand
        {
            get { return _informationMessageCommand; }
        }

        public async Task ExecuteFetchDataAsync(Func<Task> action, string message = default, bool showAtLeastSecond = true)
        {
            var fetchDataInfo = new FetchDataInfo()
            {
                ShowControl = true,
                Message = string.IsNullOrEmpty(message) ? CultureLocalization.Localization.GetResource("DialogTextFetching") : message
            };

            FetchDataCommand.Execute(fetchDataInfo);

            var mainActionTask = action();
            var oneSecondTask = Task.Delay(1000);

            var tasksToWait = new Collection<Task>()
            {
                mainActionTask
            };

            if (showAtLeastSecond)
            {
                tasksToWait.Add(oneSecondTask);
            }

            await Task.WhenAll(tasksToWait);

            fetchDataInfo.ShowControl = false;
            FetchDataCommand.Execute(fetchDataInfo);
        }

        public async Task<T> ExecuteFetchDataAsync<T>(Func<Task<T>> action, string message = default, bool showAtLeastSecond = true)
        {
            var fetchDataInfo = new FetchDataInfo()
            {
                ShowControl = true,
                Message = string.IsNullOrEmpty(message) ? CultureLocalization.Localization.GetResource("DialogTextFetching") : message
            };

            FetchDataCommand.Execute(fetchDataInfo);

            var mainActionTask = action();
            var oneSecondTask = Task.Delay(1000);

            var tasksToWait = new Collection<Task>()
            {
                mainActionTask
            };

            if (showAtLeastSecond)
            {
                tasksToWait.Add(oneSecondTask);
            }


            await Task.WhenAll(tasksToWait);

            fetchDataInfo.ShowControl = false;
            FetchDataCommand.Execute(fetchDataInfo);

            return mainActionTask.Result;
        }

        public void ShowInformationMessage(string message, InformationKind kind = InformationKind.Success)
        {
            var informationMessageInfo = new InformationMessageInfo()
            {
                Message = message,
                Kind = kind
            };

            InformationMessageCommand.Execute(informationMessageInfo);
        }
    }

}