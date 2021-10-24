using FST.ViewModel.Models;
using Prism.Commands;
using System;
using System.Threading.Tasks;

namespace FST.ViewModel.Interfaces
{
    public interface IApplicationTaskUtility
    {
        CompositeCommand FetchDataCommand { get; }
        CompositeCommand InformationMessageCommand { get; }

        Task ExecuteFetchDataAsync(Func<Task> action, string message = null, bool showAtLeastSecond = true);
        void ShowInformationMessage(string message, InformationKind kind = InformationKind.Success);
    }
}