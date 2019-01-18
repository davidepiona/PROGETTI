using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                MessageBox.Show("E12 - Il file " + DestinationPath + " è aperto da un altro programma (o non esiste).\n\nNon è possibile eliminare la cartella.", "E12"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
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
                MessageBox.Show("E13 - Il file " + SourcePath + " non esiste o è aperto da un altro programma.\n\nNon è possibile copiare la cartella.", "E13"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                return false;
            }
            
            Console.WriteLine("Copiato DATI in DATIsync");
            return true;

        }

        public bool push()
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
            _processStartInfo.FileName = Globals.GITPATH;
            var proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
            }

            _processStartInfo.Arguments = gitAddOrigin;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
            }

            _processStartInfo.Arguments = gitAddArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
            }

            _processStartInfo.Arguments = gitCommitArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERR: " + err);
                string[] split = err.Split(':');
            }

            _processStartInfo.Arguments = gitPushArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                Console.WriteLine("ERRUltim: " + err);
                string[] split = err.Split(':');
                if (split[0].Equals("To https"))
                {
                    return true;
                }
            }

            return false;



            //Process.Start(_processStartInfo);
            //Process.Start(gitCommand, gitAddOrigin);
            //Process.Start(gitCommand, gitAddArgument);
            //Process.Start(gitCommand, gitCommitArgument);
            //Process.Start(gitCommand, gitPushArgument);
        }

        public bool clone()
        {
            string SourcePath = Globals.DATI;
            string DestinationPath = Globals.DATIsync;
            try {

                var dir = new DirectoryInfo(DestinationPath);
                if (dir.Exists)
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
                MessageBox.Show("E15 - Il file " + DestinationPath + " è aperto da un altro programma (o non esiste).\n\nNon è possibile eliminare la cartella.", "E15"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                return false;
            }
    Console.WriteLine("Eliminata vecchia cartella DATIsync");
            Directory.CreateDirectory(DestinationPath);
            string gitCloneArgument = @"clone " + Globals.GITURL + " " + Globals.DATIsync;
            Console.WriteLine(gitCloneArgument);
            //Process.Start(workingDirectory);
            //Process.Start(gitCommand, );
            ProcessStartInfo _processStartInfo = new ProcessStartInfo();
            _processStartInfo.WorkingDirectory = Globals.DATIsync;
            _processStartInfo.RedirectStandardOutput = true;
            _processStartInfo.RedirectStandardInput = true;
            _processStartInfo.RedirectStandardError = true;
            _processStartInfo.Arguments = @gitCloneArgument;
            _processStartInfo.UseShellExecute = false;
            _processStartInfo.FileName = Globals.GITPATH;
            try
            {
                var proc = Process.Start(_processStartInfo);
                while (!proc.StandardError.EndOfStream)
                {
                    string err = proc.StandardError.ReadLine();
                    Console.WriteLine("ERR: " + err);
                    string[] split = err.Split(':');
                    if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                    {
                        MessageBox.Show("Problemi con il repository git, azione di clone non eseguita", "Problemi con git"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

                        return false;
                    }
                    //string[] split = err.Split(':');
                }
            }catch(Win32Exception w32e)
            {
                MessageBox.Show("Azione di clone non eseguita a causa di errore nel comando:\n"+ w32e.Message, "Clone non eseguito"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                return false;
            }
            catch(FileNotFoundException fnfe)
            {
                MessageBox.Show("Azione di clone non eseguita a causa di:\n" + fnfe.Message, "Clone non eseguito"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                return false;
            }
            //Win32Exception
            //FileNotFoundException
            return true;
        }
    }
}
