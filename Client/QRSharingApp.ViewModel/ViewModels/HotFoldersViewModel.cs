using QRSharingApp.CultureLocalization;
using QRSharingApp.Infrastructure.Models;
using QRSharingApp.Infrastructure.Services.Interfaces;
using QRSharingApp.ViewModel.Interfaces;
using QRSharingApp.ViewModel.ViewModels.Base;
using QRSharingApp.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace QRSharingApp.ViewModel.ViewModels
{
    public class HotFoldersViewModel : BaseNavigationViewModel
    {
        private ICommand _selectFolderCmd;
        private ICommand _removeFolderCmd;
        private ICommand _onloadCmd;

        [Dependency]
        public IHotFolderService _hotFolderService;
        [Dependency]
        public IFileExplorerService _fileExplorerService;
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility;

        public ICommand SelectFolderCmd
        {
            get
            {
                return _selectFolderCmd ??
                  (_selectFolderCmd = new DelegateCommand(
                      async () => {
                          var newFolderPath = _fileExplorerService.SelectFolder();
                          if (string.IsNullOrEmpty(newFolderPath))
                          {
                              return;
                          }

                          var result = await _hotFolderService.AddNew(newFolderPath);
                          if (result)
                          {
                              await RefreshFolders();
                          }
                          else
                          {
                              ApplicationTaskUtility.ShowInformationMessage(Localization.GetResource("HotFolderInformationDialog"), Models.InformationKind.Error);
                          }
                      }
                  ));
            }
        }

        public ICommand RemoveFolderCmd
        {
            get
            {
                return _removeFolderCmd ??
                  (_removeFolderCmd = new DelegateCommand<int?>(
                      async folderId => {
                          await _hotFolderService.Remove(folderId.Value);
                          await RefreshFolders();
                      }
                  ));
            }
        }

        public ICommand OnLoadCmd
        {
            get
            {
                return _onloadCmd??
                  (_onloadCmd = new DelegateCommand(
                      () => {
                          Task.Run(RefreshFolders);
                      }
                  ));
            }
        }

        public ObservableCollection<HotFolder> HotFolders { get; set; }
        public ISharedAppDataViewModel SharedAppDataViewModel { get; set; }

        public HotFoldersViewModel(ISharedAppDataViewModel sharedAppDataViewModel)
        {
            SharedAppDataViewModel = sharedAppDataViewModel;
        }

        private async Task RefreshFolders()
        {
            var hotFolders = await _hotFolderService.GetAll();
            HotFolders = new ObservableCollection<HotFolder>(hotFolders);
        }
    }
}
