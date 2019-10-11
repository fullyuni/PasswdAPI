using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace PasswdAPI
{
    public class Program
    {
        //global dataset used to store tables 
        public static DataSet dataSet;

        public static PasswdErrors.PasswdError errorStatus = 0;
        public static JArray changeLog;

        public static string groupFilePath;
        public static string passwdFilePath;

        //watcher to handle changes to files while program is running
        public static FileSystemWatcher groupWatcher = new FileSystemWatcher();
        public static FileSystemWatcher userWatcher = new FileSystemWatcher();

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
            //this will break if files are not named correctly
            if (e.Name.Contains("passwd"))
            {
                //rebuild tables if changes occured
                dataSet = new DataSet();
                DataTableBuilders.UpdateUserTable(dataSet, passwdFilePath);
            }
            else if (e.Name.Contains("group"))
            {
                //rebuild tables if changes occured
                dataSet = new DataSet();
                DataTableBuilders.UpdateGroupTable(dataSet, groupFilePath);
            }
            changeLog.Add($"File: {e.FullPath} {e.ChangeType}\n");
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            errorStatus = PasswdErrors.PasswdError.FileDeleted;
            changeLog.Add($"File: {e.FullPath} {e.ChangeType}\n");
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            errorStatus = PasswdErrors.PasswdError.FileRenamed;
            changeLog.Add($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }

        //creates new watcher
        public static FileSystemWatcher watchFile(string filePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            try
            {
                watcher.Path = Directory.GetParent(filePath).FullName;

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
            }
            catch
            {
                errorStatus = PasswdErrors.PasswdError.FileNotFound;
            }
            
            return watcher;
        }
    }
}
