using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TimeHandler
{
    class Watcher
    {
        private FileSystemWatcher _watcher;
        public Watcher()
        {
            this._watcher = new FileSystemWatcher
            {
                Path = @"C:\",
                NotifyFilter = NotifyFilters.CreationTime
                                        | NotifyFilters.Attributes
                                        | NotifyFilters.FileName
                                        | NotifyFilters.LastAccess
                                        | NotifyFilters.LastWrite
                                        | NotifyFilters.DirectoryName
                                        | NotifyFilters.Security
                                        | NotifyFilters.Size,
                Filter = "*.*",
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
            };
            this._watcher.Changed += OnFileChanged;
            this._watcher.Created += OnFileChanged;
            this._watcher.Deleted += OnFileChanged;
            this._watcher.Error += OnError;
            this._watcher.Renamed += OnFileChanged;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            if (args.FullPath.IndexOf(@"AppData\Roaming") > 0
                || args.FullPath.IndexOf(@"AppData\Local") > 0
                || args.FullPath.IndexOf(@"ProgramData") > 0
                || args.FullPath.EndsWith(".log")
                || args.FullPath.EndsWith(".LOG2")
                || args.FullPath.StartsWith(@"C:\Windows")
                || args.FullPath.StartsWith(@"C:\$Recycle.Bin")
                || args.FullPath.StartsWith($@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.")
                )
            {
                return;
            }
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    Console.WriteLine($"Changed {args.FullPath}");
                    break;
                case WatcherChangeTypes.Created:
                    Console.WriteLine($"Created {args.FullPath}");
                    break;
                case WatcherChangeTypes.Deleted:
                    Console.WriteLine($"Deleted {args.FullPath}");
                    break;
                case WatcherChangeTypes.Renamed:
                    var args2 = (RenamedEventArgs)args;
                    Console.WriteLine($"Renamed {args2.OldFullPath} {args2.FullPath}");
                    break;
            }
            this.AddWork(args.FullPath);
        }

        private void AddWork(string path)
        {
            var start_idx = path.ToLower().IndexOf("apx");
            if (start_idx < 0)
            {
                return;
            }
            var story_start = path.ToLower().Substring(start_idx);
            int number_start;
            if (story_start[3] == '-')
            {
                number_start = 4;
            } else
            {
                number_start = 3;
            }
            int story_end_idx = number_start;
            for (var i = number_start; i < story_start.Length; i++)
            {
                if (!char.IsNumber(story_start[i]))
                {
                    break;
                }
                story_end_idx++;
            }

        }

        private void OnError(object sender, EventArgs e)
        {
            Console.Error.WriteLine("Error");
        }
    }
}
