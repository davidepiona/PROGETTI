using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    class GitCommands
    {
        private string gitCommand = "git";
        private string gitUrl = "https://github.com/davidepiona/DATIsync.git";
        private string workingDirectory;
        public GitCommands() {
            workingDirectory = Directory.GetParent(Directory.GetParent(Globals.DATI).ToString()).ToString();
            //Console.WriteLine( + "\n" + System.IO.Directory.GetParent(Globals.DATI).FullName);
                }
        public void copyFolder()
        {
            string SourcePath = Globals.DATI; 
            string DestinationPath = Globals.DATIsync;
            var dir = new DirectoryInfo(DestinationPath);
            if (Directory.Exists(DestinationPath))
            {
                foreach (var info in dir.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                dir.Delete(true);
            }
            Console.WriteLine("Eliminata vecchia cartella DATIsync");
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            Directory.CreateDirectory(DestinationPath);
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            Console.WriteLine("Copiato DATI in DATIsync");
        }

        public void push()
        {
            string gitAddOrigin = @"remote add origin " + @gitUrl;
            string gitAddArgument = @"add "+ Globals.DATIsync;
            string gitCommitArgument = @"commit -m ""cambiamento"" ";
            string gitPushArgument = @"push origin master";

            ProcessStartInfo _processStartInfo = new ProcessStartInfo();
            _processStartInfo.WorkingDirectory = Globals.DATIsync;
            _processStartInfo.Arguments = "init";
            _processStartInfo.FileName = @"C:\Program Files\Git\cmd\git.exe";
            Process.Start(_processStartInfo);
            
            _processStartInfo.Arguments = gitAddOrigin;
            Process.Start(_processStartInfo);

            _processStartInfo.Arguments = gitAddArgument;
            Process.Start(_processStartInfo);

            _processStartInfo.Arguments = gitCommitArgument;
            Process.Start(_processStartInfo);

            _processStartInfo.Arguments = gitPushArgument;
            Process.Start(_processStartInfo);




            //Process.Start(_processStartInfo);
            //Process.Start(gitCommand, gitAddOrigin);
            //Process.Start(gitCommand, gitAddArgument);
            //Process.Start(gitCommand, gitCommitArgument);
            //Process.Start(gitCommand, gitPushArgument);
        }

        public void clone()
        {
            string SourcePath = Globals.DATI;
            string DestinationPath = Globals.DATIsync;
            var dir = new DirectoryInfo(DestinationPath);
            if (dir.Exists)
            {
                foreach (var info in dir.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                dir.Delete(true);

            }
            Console.WriteLine("Eliminata vecchia cartella DATIsync");
            Directory.CreateDirectory(DestinationPath);
            string gitCloneArgument = @"clone " + gitUrl + " " + Globals.DATIsync;
            Console.WriteLine(gitCloneArgument);
            //Process.Start(workingDirectory);
            Process.Start(gitCommand, @gitCloneArgument);
        }
    }
}
