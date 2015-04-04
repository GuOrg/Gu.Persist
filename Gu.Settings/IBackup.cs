namespace Gu.Settings
{
    public interface IBackup
    {
        void CreateBackup(string fileName);
     
        void RestoreBackup(string fileName);
    }
}