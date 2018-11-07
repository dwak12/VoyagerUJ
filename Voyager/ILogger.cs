namespace Voyager
{
    interface ILogger
    {
        void OpenFile();
        void CloseFile();
        void AppendLog(string text);
    }
}
