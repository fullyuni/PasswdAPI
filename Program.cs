using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Configuration;

namespace PasswdAPI
{
    public class Program
    {
        //global dataset used to store tables 
        public static DataSet dataSet = new DataSet();

        //path to demo input files
        public static string parentFilePath = @"C:\Projects\PasswdAPI\inputData\";


        //watcher to handle changes to files while program is running
        public static FileSystemWatcher watcher = new FileSystemWatcher();

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            //ignore other files in parentDirectory
            if (e.Name.Contains("passwd") || e.Name.Contains("group"))
            {
                //rebuild tables if changes occured
                dataSet = new DataSet();
                DataTableBuilders.UpdateDataTables(dataSet, parentFilePath);
            }
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }

        //creates new watcher
        public static FileSystemWatcher watchFile(string filePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = filePath;

            watcher.NotifyFilter = NotifyFilters.LastAccess
                            | NotifyFilters.LastWrite
                            | NotifyFilters.FileName
                            | NotifyFilters.DirectoryName;

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            return watcher;
        }
    }
}
