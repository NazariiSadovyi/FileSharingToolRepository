using QRSharingApp.CultureLocalization;
using QRSharingApp.Infrastructure.Models;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class HotFoldersViewModel : BaseNavigationViewModel
    {
        [Dependency]
        public IHotFolderService HotFolderService;
        [Dependency]
        public IFileExplorerService FileExplorerService;
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility;

        public ICommand SelectFolderCmd => ReactiveCommand.CreateFromTask(
            async () =>
            {
                var newFolderPath = FileExplorerService.SelectFolder();
                if (string.IsNullOrEmpty(newFolderPath))
                {
                    return;
                }

                var result = await HotFolderService.AddNew(newFolderPath);
                if (result)
                {
                    await RefreshFolders();
                }
                else
                {
                    ApplicationTaskUtility.ShowInformationMessage(Localization.GetResource("HotFolderInformationDialog"), Models.InformationKind.Error);
                }
            }
        );

        public ICommand RemoveFolderCmd => ReactiveCommand.Create<int?>(
            async folderId => {
                await HotFolderService.Remove(folderId.Value);
                await RefreshFolders();
            }
        );

        public ObservableCollection<HotFolder> HotFolders { get; set; }
        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }

        public HotFoldersViewModel(ISharedAppDataViewModel sharedAppDataViewModel)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
            HotFolders = new ObservableCollection<HotFolder>();
        }

        public override async Task OnLoadAsync()
        {
            await RefreshFolders();
        }

        private async Task RefreshFolders()
        {
            HotFolders.Clear();
            var hotFolders = await HotFolderService.GetAll();
            foreach (var hotFolder in hotFolders)
            {
                HotFolders.Add(hotFolder);
            }
        }
    }
}
