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
    /// <summary>
    /// Classe per la gestione delle operazioni che comprendono:
    /// - clone degli elementi presenti nel repository Globals.GITURL
    /// - copia degli elementi da DATI a DATIsync
    /// - push degli elementi sul repository Globals.GITURL
    /// </summary>
    class GitCommands
    {
        /// <summary>
        /// Elimina i file *cliente*.csv e *cliente*date.csv dalla cartella DATIsync.
        /// Se l'operazione riesce copia gli stessi file dalla cartella DATI a DATIsync.
        /// </summary>
        public bool copyFile(string cliente)
        {
            try
            {
                var dir = new DirectoryInfo(Globals.DATIsync);
                if (Directory.Exists(Globals.DATIsync))
                {
                    string[] fileEntries = Directory.GetFiles(Globals.DATIsync);
                    foreach (string fileName in fileEntries)
                    {
                        try
                        {
                            if (fileName.Split('\\').Last().Equals(cliente + ".csv"))
                            {
                                Console.WriteLine("Trovato " + fileName + " da eliminare");
                                File.Delete(fileName);
                            }
                            if (fileName.Split('\\').Last().Equals(cliente + "date.csv"))
                            {
                                Console.WriteLine("Trovato " + fileName + " da eliminare");
                                File.Delete(fileName);
                            }
                            if (fileName.Split('\\').Last().Equals(cliente + ".CSV"))
                            {
                                Console.WriteLine("Trovato " + fileName + " da eliminare");
                                File.Delete(fileName);
                            }
                            if (fileName.Split('\\').Last().Equals(cliente + "date.CSV"))
                            {
                                Console.WriteLine("Trovato " + fileName + " da eliminare");
                                File.Delete(fileName);
                            }
                        }
                        catch (IOException)
                        {
                            string msg = "E25 - Non è possibile eliminare un file tra " + cliente + ".csv e " + cliente + "date.csv.\n" +
                                "La ricerca è avvenuta in " + Globals.DATIsync + ".";
                            MessageBox.Show(msg, "E25"
                                , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                            Globals.log.Error(msg);
                            return false;
                        }
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E12 - Il file " + Globals.DATIsync + " è aperto da un altro programma (o non esiste).\n\nNon è possibile eliminare la cartella.";
                MessageBox.Show(msg, "E12", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }

            Console.WriteLine("Eliminati file " + cliente + ".csv e " + cliente + "date.csv in DATIsync");
            Globals.log.Info("Eliminati file " + cliente + ".csv e " + cliente + "date.csv in DATIsync");
            try
            {
                File.Copy(Globals.DATI + cliente + ".csv", Globals.DATIsync + cliente + ".csv");
                File.Copy(Globals.DATI + cliente + "date.csv", Globals.DATIsync + cliente + "date.csv");
            }
            catch (IOException ioe)
            {
                string msg = "E13 - Il file non esiste o è aperto da un altro programma.\n\nNon è possibile copiare la cartella." + ioe.Message;
                MessageBox.Show(msg, "E13", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            Console.WriteLine("Copiato DATI in DATIsync");
            Globals.log.Info("Copiato DATI in DATIsync");
            return true;
        }

        /// <summary>
        /// Copia (sostituendo se presenti) tutti i file dalla cartella DATI a DATIsync.
        /// Restituisce false se viene sollevata una IOException.
        /// </summary>
        public bool copyFolder()
        {
            string SourcePath = Globals.DATI;
            string DestinationPath = Globals.DATIsync;
            try
            {
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            }
            catch (IOException)
            {
                string msg = "E40 - Il file " + SourcePath + " non esiste o è aperto da un altro programma.\n\nNon è possibile copiare la cartella.";
                MessageBox.Show(msg, "E42", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            Console.WriteLine("Sostituito file in DATIsync con quelli in DATI");
            Globals.log.Info("Sostituito file in DATIsync con quelli in DATI");
            return true;
        }

        /// <summary>
        /// Esegue i comandi shell necessari per effettuare un commit e un push, restituendo una lista dei file caricati
        /// - aggiunge tutti i fine di tipo cliente*
        /// - colleziona la lista dei file aggiunti (la restituirà in caso di successo del push)
        /// - crea un commit
        /// - effettua il push 
        /// - SE l'ultimo output inizia con "To https" o "Everything up-to-date" ritrona la lista di file altrimenti 'null'
        /// - EFFETTUO DUE VOLTE IL PUSH PERCHE' (PER MOTIVO SCONOSCIUTO) NON SEMPRE AL PRIMO RIESCE A CARICARLO (--> DA TROVARE SOLUZIONE PIU' VELOCE)
        /// </summary>
        public List<string> push(string cliente)
        {
            ProcessStartInfo _processStartInfo = new ProcessStartInfo();
            _processStartInfo.WorkingDirectory = Globals.DATIsync;
            _processStartInfo.RedirectStandardError = true;
            _processStartInfo.RedirectStandardOutput = true;
            _processStartInfo.UseShellExecute = false;
            _processStartInfo.CreateNoWindow = true;
            _processStartInfo.FileName = Globals.GITPATH;

            string gitAddArgument = @"add " + @Globals.DATIsync + cliente + "*";
            if (cliente.Equals(""))
            {
                gitAddArgument = @"add *";
            }
            _processStartInfo.Arguments = gitAddArgument;
            var proc = Process.Start(_processStartInfo);
            while (!proc.StandardOutput.EndOfStream)
            {
                string info = proc.StandardOutput.ReadLine();
                Console.WriteLine("OUT: " + info);
            }
            List<string> commitInfo = new List<string>();
            string gitDryRunArgument = @"commit --dry-run --short";
            _processStartInfo.Arguments = gitDryRunArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardOutput.EndOfStream)
            {
                string info = proc.StandardOutput.ReadLine();
                //string[] split = info.Split(':');
                Console.WriteLine("OUT: " + info + "info[0]: <" + info[0] + "> quindi: " + !info[0].Equals(' '));
                Globals.log.Info("OUT: " + info + "info[0]: <" + info[0] + "> quindi: " + !info[0].Equals(' '));
                if (!info[0].Equals(' ') && !info[0].Equals('?'))
                {
                    commitInfo.Add(info);
                }
            }
            string gitCommitArgument = @"commit -m ""cambiamento"" ";
            _processStartInfo.Arguments = gitCommitArgument;
            proc = Process.Start(_processStartInfo);
            string gitPushArgument = @"push -f origin master";
            _processStartInfo.Arguments = gitPushArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                string[] split = err.Split(':');
                Console.WriteLine("ERRPenultimo: " + err + ">");
                Globals.log.Info("ERRPenultimo: " + err + ">");

                if (split[0].Equals("To https"))
                {
                    Console.WriteLine("Fine push, numero elementi:" + commitInfo.Count);
                    Globals.log.Info("Fine push, numero elementi:" + commitInfo.Count);
                    return commitInfo;
                }
            }
            _processStartInfo.Arguments = gitPushArgument;
            proc = Process.Start(_processStartInfo);
            while (!proc.StandardError.EndOfStream)
            {
                string err = proc.StandardError.ReadLine();
                string[] split = err.Split(':');
                Console.WriteLine("ERRUltimo: " + err + ">");
                Globals.log.Info("ERRUltimo: " + err + ">");

                if (split[0].Equals("To https") || split[0].Equals("Everything up-to-date"))
                {
                    Console.WriteLine("Fine push, numero elementi:" + commitInfo.Count);
                    Globals.log.Info("Fine push, numero elementi:" + commitInfo.Count);
                    return commitInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Esegue i comandi shell necessari per effettuare un clone, restituendo un bool che rappresenta l'esito
        /// - elimina la cartella DATIsync
        /// - esegue un clone del repository Globals.GITURL
        /// - se non vengono sollevate eccezioni ritorna true
        /// </summary>
        public bool clone()
        {
            try
            {
                var dir = new DirectoryInfo(Globals.DATIsync);
                if (dir.Exists)
                {
                    foreach (var info in dir.GetFileSystemInfos("*", SearchOption.AllDirectories))
                    {
                        info.Attributes = FileAttributes.Normal;
                    }

                    dir.Delete(true);
                }
                Console.WriteLine("Eliminata vecchia cartella DATIsync");
                Globals.log.Info("Eliminata vecchia cartella DATIsync");
            }
            catch (IOException)
            {
                string msg = "E15 - Il file " + Globals.DATIsync + " è aperto da un altro programma (o non esiste).\n\nNon è possibile eliminare la cartella.";
                MessageBox.Show(msg, "E15", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            Directory.CreateDirectory(Globals.DATIsync);
            string gitCloneArgument = @"clone " + Globals.GITURL + " " + Globals.DATIsync;
            Console.WriteLine("Clone argument: "+gitCloneArgument);
            Globals.log.Info("Clone argument: "+gitCloneArgument);
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
                    Globals.log.Info("ERR: " + err);
                    string[] split = err.Split(':');
                    if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                    {
                        string msg = "Problemi con il repository git, azione di clone non eseguita";
                        MessageBox.Show(msg, "Problemi con git", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                        return false;
                    }
                }
            }
            catch (Win32Exception w32e)
            {
                string msg = "Azione di clone non eseguita a causa di errore nel comando:\n" + w32e.Message;
                MessageBox.Show(msg, "Clone non eseguito", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            catch (FileNotFoundException fnfe)
            {
                string msg = "Azione di clone non eseguita a causa di:\n" + fnfe.Message;
                MessageBox.Show(msg, "Clone non eseguito"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            return true;
        }

        public bool Checkout(string cliente)
        {
            ProcessStartInfo _processStartInfo = new ProcessStartInfo();
            _processStartInfo.WorkingDirectory = Globals.DATIsync;
            _processStartInfo.RedirectStandardOutput = true;
            _processStartInfo.RedirectStandardInput = true;
            _processStartInfo.RedirectStandardError = true;
            _processStartInfo.UseShellExecute = false;
            _processStartInfo.FileName = Globals.GITPATH;
            try
            {
                string gitFetch = @"fetch";
                _processStartInfo.Arguments = gitFetch;

                string gitCheckout = @"checkout origin/master -- " + cliente + "*";
                _processStartInfo.Arguments = gitCheckout;
            }
            catch (Win32Exception w32e)
            {
                string msg = "Azione di clone non eseguita a causa di errore nel comando:\n" + w32e.Message;
                MessageBox.Show(msg, "Clone non eseguito", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            return true;
        }
    }
}
