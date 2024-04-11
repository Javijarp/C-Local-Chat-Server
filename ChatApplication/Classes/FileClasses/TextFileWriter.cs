namespace FileClasses
{
    public class TextFileWriter : IFileWriter
    {
        protected string folderPath;
        protected string fileName;
        protected string fullPath;

        public TextFileWriter(string folderPath, string fileName)
        {
            this.folderPath = folderPath;
            this.fileName = fileName;
            this.fullPath = Path.Combine(folderPath, fileName);
            Console.WriteLine($"fullPath: {this.fullPath}");
            this.CreateFile();
        }
        public void WriteMessage(Message message)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(fullPath))
                {
                    writer.WriteLine(message.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        public void RenameFile(string newFileName)
        {
            try
            {
                string newFullPath = Path.Combine(this.folderPath, newFileName);
                if (File.Exists(this.fullPath))
                {
                    if (!File.Exists(newFullPath))
                    {
                        File.Move(this.fullPath, newFullPath);
                        this.fullPath = newFullPath;
                        this.fileName = newFileName;
                        Console.WriteLine($"File renamed to: {newFileName}");
                    }
                    else
                    {
                        Console.WriteLine($"File '{newFullPath}' already exists.");
                    }
                }
                else
                {
                    Console.WriteLine($"Original file '{this.fileName}' already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error renaming file: {ex.Message}");
            }
        }

        public void MakeBackupFile()
        {
            try
            {
                string backupFolderPath = Path.Combine(this.folderPath, "Backup");
                Directory.CreateDirectory(backupFolderPath); // Ensure the Backup folder exists.
                string backupFilePath = Path.Combine(backupFolderPath, this.fileName);

                if (File.Exists(this.fullPath))
                {
                    File.Copy(this.fullPath, backupFilePath, true); // Overwrite the backup if it exists.
                    Console.WriteLine($"Backup saved to: {backupFilePath}");
                }
                else
                {
                    Console.WriteLine($"File '{this.fileName}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        public void CreateFile()
        {
            string absoluteFolderPath = Path.GetFullPath(this.folderPath);
            Directory.CreateDirectory(absoluteFolderPath);
            this.folderPath = absoluteFolderPath;
            this.fullPath = Path.Combine(this.folderPath, this.fileName);
            if (!File.Exists(fullPath))
            {
                using (var stream = File.Create(this.fullPath))
                {
                    Console.WriteLine($"File created: {this.fullPath}");
                }
            }
        }
    }
}