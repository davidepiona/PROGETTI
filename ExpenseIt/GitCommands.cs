using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExpenseIt
{
    class GitCommands
    {
        private string gitCommand = "git";

        private string workingDirectory;
        public GitCommands()
        {
            workingDirectory = Directory.GetParent(Directory.GetParent(Globals.DATI).ToString()).ToString();
            //Console.WriteLine( + "\n" + System.IO.Directory.GetParent(Globals.DATI).FullName);
        }
        public bool copyFolder()
        {
            string SourcePath = Globals.DATI;
            string DestinationPath = Globals.DATIsync;
            try
            {
                var dir = new DirectoryInfo(DestinationPath);
                if (Directory.Exists(DestinationPath))
                {
                    foreach (var info in dir.GetFileSystemInfos("*", SearchOption.AllDirectories))
                    {
                        info.Attributes = FileAttributes.Normal;
                    }

                    dir.Delete(true);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("E12 - Il file " + DestinationPath + " è aperto da un altro programma (o non esiste).\n\nNon è possibile eliminare la cartella.");
                return false;
            }
            Console.WriteLine("Eliminata vecchia cartella DATIsync");
            //Now Create all of the directories
            try
            {
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
                Directory.CreateDirectory(DestinationPath);
                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            }
            catch (IOException)
            {
                MessageBox.Show("E13 - Il file " + SourcePath + " non esiste o è aperto da un altro programma.\n\nNon è possibile copiare la cartella.");
                return false;
            }
            
            Console.WriteLine("Copiato DATI in DATIsync");
            return true;

        }

        public void push()
        {
            string gitAddOrigin = @"remote add origin " + @Globals.GITURL;
            string gitAddArgument = @"add " + Globals.DATIsync;
            string gitCommitArgument = @"commit -m ""cambiamento"" ";
            string gitPushArgument = @"push -f origin master";

            ProcessStartInfo _processStartInfo = new ProcessStartInfo();
            _processStartInfo.WorkingDirectory = Globals.DATIsync;
            _processStartInfo.RedirectStandardError = true;
            _processStartInfo.Arguments = "init";
            _processStartInfo.UseShellExecute = false;
            _processStartInfo.FileName = @"C:\Program Files\Git\cmd\git.exe";
            var proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
                if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                {

                }
            }

            _processStartInfo.Arguments = gitAddOrigin;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
                if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                {

                }
            }

            _processStartInfo.Arguments = gitAddArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
                if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                {

                }
            }

            _processStartInfo.Arguments = gitCommitArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
                if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                {

                }
            }

            _processStartInfo.Arguments = gitPushArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERRUltim: " + err);
                string[] split = err.Split(':');
                if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                {

                }
            }




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
            string gitCloneArgument = @"clone " + Globals.GITURL + " " + Globals.DATIsync;
            Console.WriteLine(gitCloneArgument);
            //Process.Start(workingDirectory);
            Process.Start(gitCommand, @gitCloneArgument);
        }
    }
}
