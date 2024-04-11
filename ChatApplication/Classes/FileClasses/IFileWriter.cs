namespace FileClasses
{
    interface IFileWriter
    {
        public void WriteMessage(Message message);
        public void RenameFile(string newFileName);
        public void MakeBackupFile();
        void CreateFile();
    }
}