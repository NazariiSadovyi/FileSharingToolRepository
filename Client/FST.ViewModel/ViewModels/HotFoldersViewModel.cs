using FST.CultureLocalization;
using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using FST.ViewModel.Interfaces;
using FST.ViewModel.ViewModels.Base;
using FST.ViewModel.ViewModels.Interfaces;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace FST.ViewModel.ViewModels
{
    public class HotFoldersViewModel : BaseNavigationViewModel
    {
        private ICommand _selectFolderCmd;
        private ICommand _removeFolderCmd;
        private ICommand _onloadCmd;
        private ObservableCollection<HotFolder> _hotFolders;
        private ISharedAppDataViewModel _sharedAppDataViewModel;

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

        public ObservableCollection<HotFolder> HotFolders
        {
            get { return _hotFolders; }
            set { SetProperty(ref _hotFolders, value); }
        }

        public ISharedAppDataViewModel SharedAppDataViewModel
        {
            get { return _sharedAppDataViewModel; }
            set { SetProperty(ref _sharedAppDataViewModel, value); }
        }

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
