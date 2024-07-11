using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class FolderSynchronizer
{
    private string sourcePath;
    private string replicaPath;
    private int syncInterval;
    private string logFilePath;

    public FolderSynchronizer(string sourcePath, string replicaPath, int syncInterval, string logFilePath)
    {
        this.sourcePath = sourcePath;
        this.replicaPath = replicaPath;
        this.syncInterval = syncInterval;
        this.logFilePath = logFilePath;
    }

    public void Start()
    {
        while (true)
        {
            SynchronizeFolders();
            Thread.Sleep(syncInterval * 1000);
        }
    }

    private void SynchronizeFolders()
    {
        Log("Synchronization started.");
        try
        {
            var sourceFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            var replicaFiles = Directory.GetFiles(replicaPath, "*.*", SearchOption.AllDirectories);

            // Copy/update files from source to replica
            foreach (var sourceFile in sourceFiles)
            {
                var relativePath = sourceFile.Substring(sourcePath.Length + 1);
                var replicaFile = Path.Combine(replicaPath, relativePath);

                if (!File.Exists(replicaFile) || !FilesAreEqual(new FileInfo(sourceFile), new FileInfo(replicaFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(replicaFile));
                    File.Copy(sourceFile, replicaFile, true);
                    Log($"Copied/Updated: {relativePath}");
                }
            }

            // Delete files from replica that are not in source
            foreach (var replicaFile in replicaFiles)
            {
                var relativePath = replicaFile.Substring(replicaPath.Length + 1);
                var sourceFile = Path.Combine(sourcePath, relativePath);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(replicaFile);
                    Log($"Deleted: {relativePath}");
                }
            }

            // Handle directories
            SynchronizeDirectories(sourcePath, replicaPath);
        }
        catch (Exception ex)
        {
            Log($"Error: {ex.Message}");
        }
        Log("Synchronization completed.");
    }

    private void SynchronizeDirectories(string sourceDir, string replicaDir)
    {
        var sourceDirectories = Directory.GetDirectories(sourceDir, "*.*", SearchOption.AllDirectories);
        var replicaDirectories = Directory.GetDirectories(replicaDir, "*.*", SearchOption.AllDirectories);

        // Create directories in replica that exist in source
        foreach (var sourceDirectory in sourceDirectories)
        {
            var relativePath = sourceDirectory.Substring(sourceDir.Length + 1);
            var replicaDirectory = Path.Combine(replicaDir, relativePath);

            if (!Directory.Exists(replicaDirectory))
            {
                Directory.CreateDirectory(replicaDirectory);
                Log($"Created directory: {relativePath}");
            }
        }

        // Delete directories from replica that are not in source
        foreach (var replicaDirectory in replicaDirectories)
        {
            var relativePath = replicaDirectory.Substring(replicaDir.Length + 1);
            var sourceDirectory = Path.Combine(sourceDir, relativePath);

            if (!Directory.Exists(sourceDirectory))
            {
                Directory.Delete(replicaDirectory, true);
                Log($"Deleted directory: {relativePath}");
            }
        }
    }

    private bool FilesAreEqual(FileInfo first, FileInfo second)
    {
        if (first.Length != second.Length)
            return false;

        using (var fs1 = first.OpenRead())
        using (var fs2 = second.OpenRead())
        {
            var sha1 = new SHA1Managed();
            var hash1 = sha1.ComputeHash(fs1);
            var hash2 = sha1.ComputeHash(fs2);
            return hash1.SequenceEqual(hash2);
        }
    }

    private void Log(string message)
    {
        var logMessage = $"{DateTime.Now}: {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }

    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSynchronizer <sourcePath> <replicaPath> <syncInterval> <logFilePath>");
            return;
        }

        var sourcePath = args[0];
        var replicaPath = args[1];
        var syncInterval = int.Parse(args[2]);
        var logFilePath = args[3];

        var synchronizer = new FolderSynchronizer(sourcePath, replicaPath, syncInterval, logFilePath);
        synchronizer.Start();
    }
}
