namespace Gu.Settings.Demo
{
    using System.Windows.Input;
    using RemoveItemBox;

    public class ManualSaveSettingsViewModel
    {
        public ManualSaveSettingsViewModel()
        {
            ManualSaveSetting = RepositoryVm.Instance.ManualSaveSetting;
            SaveCommand = new RelayCommand(_ => RepositoryVm.Instance.Save(ManualSaveSetting),
                _ => RepositoryVm.Instance.Repository.IsDirty(ManualSaveSetting));
        }

        public ManualSaveSetting ManualSaveSetting { get; }

        public ICommand SaveCommand { get; }
    }
}