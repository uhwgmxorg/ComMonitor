namespace WpfHexaEditor.Core.Interfaces
{
    public interface IFileEditable
    {
        string FileName { get; set; }
        void CloseProvider();
        void SubmitChanges(string newfilename, bool overwrite = false);
        void SubmitChanges();

        void Undo(int repeat = 1);
        bool ReadOnlyMode { get; set; }
        void CopyToClipboard(CopyPasteMode copypastemode);
    }
}
