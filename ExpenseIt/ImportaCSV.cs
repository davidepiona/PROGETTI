using DATA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DATA
{
    /// <summary>
    /// Classe che permette di modificare i file .csv utilizzati dal programma MATRIX
    /// per ottenere dei file .csv leggibili da DATA
    /// </summary>
    class ImportaCSV
    {
        private string origine;
        private string destinazione;
        private string temp;

        /// <summary>
        /// Costruttore classico che inizializa alcuni attributi ai valori passati 
        /// </summary>
        public ImportaCSV(string origine, string destinazione)
        {
            this.origine = origine;
            this.destinazione = destinazione;
            string[] percorso = destinazione.Split('\\');
            //CREAVO LA CARTELLA TEMP PER PROVARE A RISOLVERE IL BUG - non ha funzionato
            //for (int i =0; i< percorso.Length-2; i++)
            //{
            //    temp += percorso[i] +'\\';
            //}
            //temp += "TEMP\\";
        }

        /// <summary>
        /// Funzione che gestisce la vera e propria importazione.
        /// - lista tutti i file nella cartella 'origine' passata e li passa uno ad uno
        /// - se trova i file LAST.csv li salta
        /// - se trova i file CLIENTI.csv li converte con l'apposito metodo importazioneCSV
        /// - tutti gli altri file li legge fino a che non incontra 4 punti consecutivi (fine file)
        /// - allora li copia nella cartella 'destinazione' passata
        /// </summary>
        /// <returns></returns>
        public string importazione()
        {
            Console.WriteLine("Read MATRIX Projects");
            Globals.log.Info("Read MATRIX Projects");
            string s = "";
            string[] fileEntries = Directory.GetFiles(origine);
            //Directory.CreateDirectory(temp);
            foreach (string fileName in fileEntries)
            {
                string nomeCSV = fileName.Split('\\').Last();
                if (nomeCSV.Equals("CLIENTI.csv") || nomeCSV.Equals("CLIENTI.CSV"))
                {
                    try
                    {
                        importazioneCLIENTI(fileName);
                        s += "Importato il file " + nomeCSV + "\n";
                    }
                    catch (IOException)
                    {
                        string msg = "E42 - errore nell'importazione del file " + fileName + "(IOException)";
                        s += "NON importato il file " + nomeCSV + "(IOException)\n";
                        Globals.log.Error(msg);
                    }
                    catch (FormatException)
                    {
                        string msg = "E42 - errore nell'importazione del file " + fileName + "(FormatException)";
                        s += "NON importato il file " + nomeCSV + "(FormatException)\n";
                        Globals.log.Error(msg);
                    }
                }

                if (!nomeCSV.Equals("CLIENTI.csv") && !nomeCSV.Equals("LAST.csv") && !nomeCSV.Equals("CLIENTI.CSV") && !nomeCSV.Equals("LAST.CSV"))
                {
                    List<Progetto> proj = readProjects(fileName);
                    if (proj != null)
                    {
                        string file = destinazione + nomeCSV;
                        File.Delete(file);

                        try
                        {
                            foreach (Progetto p in proj)
                            {
                                string projectDetails = p.numero + Environment.NewLine + p.nome + Environment.NewLine + p.tipoPLC + Environment.NewLine + p.tipoOP + Environment.NewLine + p.data + Environment.NewLine;
                                File.AppendAllText(file, projectDetails);
                            }
                            s += "Importato il file " + nomeCSV + "\n";
                        }
                        catch (IOException)
                        {
                            s += "NON importato il file " + nomeCSV + "(IOException)\n";
                        }
                        catch (FormatException)
                        {
                            s += "NON importato il file " + nomeCSV + "(FormatException)\n";
                        }
                    }
                }
            }
            return s;
        }
        /// <summary>
        /// Metodo ausiliario che si occupa della lettura dei file .csv fino a che non incontra 4 punti consecutivi 
        /// Quindi restituisce una lista dei progetti.
        /// </summary>
        private List<Progetto> readProjects(string fileName)
        {
            List<string> lines = new List<string>();
            int j = 0;
            List<Progetto> progetti = new List<Progetto>();
            try
            {
                using (var reader = new CsvFileReader(fileName))
                {
                    while (reader.ReadRow(lines) && lines.Count != 0 && lines != null)
                    {
                        string Snum = lines[0];
                        int num = Int32.Parse(Snum);
                        reader.ReadRow(lines);
                        string nome = lines[0];
                        reader.ReadRow(lines);
                        string tipoPLC = lines[0];
                        reader.ReadRow(lines);
                        string tipoOP = lines[0];
                        reader.ReadRow(lines);
                        string data = lines[0];
                        if (nome.Equals(".") && tipoPLC.Equals(".") && tipoOP.Equals("."))
                        {
                            return progetti;
                        }
                        progetti.Add(new Progetto(num, nome, tipoPLC, tipoOP, data, "", ""));
                        j++;
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E40 - Il file " + fileName + " non esiste o è aperto da un altro programma";
                //MessageBox.Show(msg, "E40"
                //                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return null;
            }
            catch (FormatException)
            {
                string msg = "E41 - Il file " + fileName + " è in un formato non corretto.\nProblema riscontrato al progetto numero: " + j;
                //MessageBox.Show(msg, "E41", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return null;
            }
            return progetti;
        }

        /// <summary>
        /// Metodo che si occupa di leggere e riscrivere il file CLIENTI.csv
        /// - legge tutto memorizzando le informazioni in nella lista CLIENTI (attribuisce 1 al lastId)
        /// - imposta LAST_CLIENT di default al primo cliente incontrato
        /// - scrive su un file .csv nuovo le stesse informazioni secondo il nuovo formato
        /// </summary>
        private void importazioneCLIENTI(string fileName)
        {
            var file = File.OpenRead(fileName);
            var reader = new StreamReader(file);
            reader.ReadLine();
            List<Cliente> CLIENTI = new List<Cliente>();
            bool first = true;
            string LAST_CLIENT = "";
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                if (line.Length == 4)
                {
                    CLIENTI.Add(new Cliente(line[0], line[1], 1, Int32.Parse(line[3])));
                }
                if (first)
                {
                    LAST_CLIENT = line[0];
                    first = false;
                }
            }
            file.Close();

            string[] lines = new string[2 + CLIENTI.Count];
            lines[0] = "CLIENTE,SUFFISSO,LAST_ID,MAX_ID";
            lines[1] = LAST_CLIENT;
            int i = 2;
            foreach (Cliente c in CLIENTI)
            {
                lines[i] = c.getNomeCliente() + "," + c.getSuffisso() + "," + c.getlastId() + "," + c.getMaxId();
                i++;
            }
            File.WriteAllLines(destinazione + "CLIENTI.csv", lines);
        }

        /// <summary>
        /// CREATO PROVANDO A RISOLVERE IL BUG CHE FA SI CHE IMPORTANDO I CSV DOPO AVER
        /// FATTO UNA RICERCA NELLA PAGINA CLIENTI SI BLOCCHI L'APPLICAZIONE.
        /// </summary>
        private void changeFolder()
        {
            Globals.log.Info("ChangeFolder");

            //METTO TUTTO IN TEMP
            string[] fileEntries = Directory.GetFiles(destinazione);
            foreach (string fileName in fileEntries)
            {
                string ultimo = fileName.Split('\\').Last();
                if (!File.Exists(temp + ultimo))
                {
                    File.Copy(fileName, temp + ultimo);
                }
            }
            Globals.DATI = temp;
            //ELIMINO DATI
            DirectoryInfo di = new DirectoryInfo(destinazione);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            Directory.Delete(destinazione, false);

            //RINOMINO TEMP IN DATI
            Globals.log.Info("DEST: " + destinazione);

            Directory.CreateDirectory(destinazione);
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(temp, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(temp, destinazione));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(temp, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(temp, destinazione), true);

            Globals.DATI = destinazione;

            //ELIMINO DATI
            di = new DirectoryInfo(temp);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            Directory.Delete(temp, false);
        }
    }
}
