namespace FST.Infrastructure.Services.Interfaces
{
    public interface IFileExplorerService
    {
        string SaveFile(string defaultFileName, string filter, string dialogTitle = "Save file");
        string SelectFile(string filter);
        string SelectFolder();
    }
}