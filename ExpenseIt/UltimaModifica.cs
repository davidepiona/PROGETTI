using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA
{
    class UltimaModifica
    {
        /// <summary>
        /// Classe per la ricerca e memorizzazione dell'ultima modifica avvenuta in una cartella.
        /// - scrittura e lettura ultima modifica delle cartelle da file csv
        /// - confronto ultime modifiche sync e non per riempire la checkbox
        /// </summary>
        private Cliente cliente;
        private Dictionary<string, DateTime> allDate = new Dictionary<string, DateTime>();
        private Dictionary<string, DateTime> allDateSync = new Dictionary<string, DateTime>();
        private Dictionary<string, int> status = new Dictionary<string, int>();
        DateTime dtNew = new DateTime();
        private string fileName; 

        /// <summary>
        /// Costruttore semplice
        /// </summary>
        public UltimaModifica(Cliente cliente)
        {
            this.cliente = cliente;
        }

        /// <summary>
        /// Metodo che scrive nei progetti ricevuti le rispettive ultime modifiche che si hanno in memoria 
        /// La scrittura avviene in un formato che permette di ordinarli in modo da evidenziare gli ultimi progetti su cui si è lavorato
        /// </summary>
        public void aggiornoModifiche(List<Progetto> progetti)
        {
            Console.WriteLine("Aggiorno Modifiche");
            Globals.log.Info("Aggiorno Modifiche");
            foreach (Progetto p in progetti)
            {
                if (allDate.TryGetValue(p.sigla, out DateTime ultima))
                {
                    p.modifica = ultima.ToString("yyyy/MM/dd HH:mm:ss");
                }
            }
        }

        /// <summary>
        /// Metodo che fa partire una ricerca all'interno di tutti i file in tutte le cartelle (ricrosivamente) 
        /// a partire dalla cartella 'path'. Viene memorizzata l'ultima modifica più recente.
        /// Vengono controllate solo le modifiche dei file e non delle cartelle per cercare di non essere ingannati dalle cartelle copiate.
        /// Restituisce false se viene generata qualche eccezione.
        /// Utilizza i metodi ausiliari: modificheByFile, ProcessDirectory e ProcessFile.
        /// </summary>
        public bool ricercaLenta(string path)
        {
            try
            {
                allDate = new Dictionary<string, DateTime>();
                if (Directory.Exists(path))
                {
                    foreach (string proj in Directory.GetDirectories(path))
                    {
                        DateTime res = modificheByFile(proj);

                        allDate.Add(proj.Split('\\').Last(), res);
                        Console.WriteLine("CARTELLA: <" +proj+ "> - DATA PIU' RECENTE: <" + res + ">");
                        Globals.log.Info("CARTELLA: <" +proj+ "> - DATA PIU' RECENTE: <" + res + ">");
                    }
                }
            }catch(Exception e)
            {
                string msg = "Eccezione nella ricerca ultime modifiche: " + e;
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            return true;

        }

        /// <summary>
        /// Chiama i metodi ProcessDirectory o ProcessFile a seconda di che tipo di elemento riceve
        /// </summary>
        public DateTime modificheByFile(string proj)
        {
            dtNew = new DateTime();
            if (Directory.Exists(proj))
            {
                ProcessDirectory(proj);
            }
            else if (File.Exists(proj))
            {
                ProcessFile(proj);
            }
            return dtNew;
        }

        /// <summary>
        /// Processa tutti i file nella cartella che è stata passata e
        /// agisce ricorsivamente nello stesso modo in ogni sottodirectory
        /// </summary>
        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        /// <summary>
        /// Azione di confronto su ogni singolo file per valutare se la sua data di modifica 
        /// sia la più recente e nel caso memorizzarla.
        /// </summary>
        private void ProcessFile(string path)
        {
            DateTime dt = File.GetLastWriteTime(path);
            
            if (DateTime.Compare(dtNew, dt) < 0)
            {
                dtNew = dt;
                fileName = path;
            }
        }

        /// <summary>
        /// Metodo che confronta le date di ultima modifica sync e non
        /// Assegna all'attributo sync degli oggetti Progetto un valore che permetta di riempire la checkbox della DataGrid
        /// </summary>
        public bool confrontoSync(List<Progetto> progetti)
        {
            Console.WriteLine("Syncronizzo");
            Globals.log.Info("Syncronizzo");
            if (allDate.Count == 0 || allDateSync.Count == 0)
            {
                return false;
            }
            foreach (Progetto p in progetti)
            {
                bool pc = allDate.TryGetValue(p.sigla, out DateTime datePc);
                bool sync = allDateSync.TryGetValue(p.sigla, out DateTime dateSync);
                //Console.WriteLine("SY: " + p.numero + "  pc: " + datePc + "  sync: " + dateSync);
                if (pc & sync)
                {
                    if (DateTime.Compare(DateTime.Parse(datePc.ToString()), DateTime.Parse(dateSync.ToString())) > 0)
                    {
                        p.sync = null;
                    }
                    else if (DateTime.Compare(DateTime.Parse(datePc.ToString()), DateTime.Parse(dateSync.ToString())) < 0)
                    {
                        p.sync = false;
                    }
                    else
                    {
                        p.sync = true;
                    }
                }
                else if (pc)
                {
                    p.sync = null;
                }
                else if (sync)
                {
                    p.sync = false;
                }

            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             SCRIVI E LEGGI CSV                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Metodo che legge le date da file *CLIENTE*date.csv in DATI e le scrive in allDate
        /// Restituisce false se per qualche ragione è stata sollevata un IOException
        /// </summary>
        public bool readByCSV(string file)
        {
            List<string> lines = new List<string>();
            int j = 0;
            try
            {
                using (var reader = new CsvFileReader(file))
                {
                    while (reader.ReadRow(lines))
                    {
                        j++;
                        if (lines.Count != 0)
                        {
                            allDate.Add(lines[0], DateTime.Parse(lines[1]));
                        }
                        else
                        {
                            Console.WriteLine("vuoto");
                        }
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E28 - Il file " + file + " non esiste o è aperto da un altro programma";
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            catch (FormatException)
            {
                string msg = "E33 - Il file " + file + " è in un formato non corretto.\nProblema riscontrato all riga numero: " + j;
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            Globals.log.Info("Date lette da DATI");
            return true;
        }

        /// <summary>
        /// Metodo che legge le date da file *CLIENTE*date.csv da DATIsync e le scrive in allDateSync
        /// Restituisce false se per qualche ragione è stata sollevata un IOException
        /// </summary>
        public bool readSync(string file)
        {
            allDateSync = new Dictionary<string, DateTime>();
            List<string> lines = new List<string>();
            int j = 0;
            try
            {
                using (var reader = new CsvFileReader(file))
                {
                    while (reader.ReadRow(lines))
                    {
                        j++;
                        if (lines.Count != 0)
                        {
                            allDateSync.Add(lines[0], DateTime.Parse(lines[1]));
                        }
                        else
                        {
                            Console.WriteLine("vuoto");
                        }
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E29 - Il file " + file + " non esiste o è aperto da un altro programma";
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            catch (FormatException)
            {
                string msg = "E33 - Il file " + file + " è in un formato non corretto.\nProblema riscontrato all riga numero: " + j;
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            catch (Exception e)
            {
                string msg = "Eccezione non riconosciuta alla lettura del file sync .csv" + e.Message;
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            Globals.log.Info("Date lette da DATIsync");
            return true;
        }

        /// <summary>
        /// Metodo che scrive le attuali date memorizzate in allDate nel file *CLIENTE*date.csv contenuto in DATI
        /// Restituisce false se per qualche ragione è stata sollevata un IOException
        /// </summary>
        public bool writeInCSV(string file)
        {
            
            string projectDate = "";
            foreach (KeyValuePair<string, DateTime> i in allDate)
            {
                projectDate += i.Key + "," + i.Value + Environment.NewLine;
            }
            try
            {
                File.WriteAllText(file, projectDate);
            }
            catch (IOException)
            {
                return false;
            }
            Globals.log.Info("Date scritte in DATI");
            return true;
        }

        public void addModifyDate(string file, string progetto, DateTime data)
        {
            allDate.Add(progetto, data);
            writeInCSV(file);
        }

        public void checkLastModifyProject(string sigla, string file)
        {
            if (!allDate.TryGetValue(sigla, out DateTime ultima))
            {
                allDate.Add(sigla, modificheByFile(file));
            }
        }

        public string getFileName()
        {
            return fileName;
        }
    }
}

// RICERCA RAPIDA --> non implementata perchè la ricerca veloce + il fatto che vengono letti da csv i risultati 
// delle scorse ricerche si sono (fin ora) rivelati abbastanza rapidi

//public void ricercaRapida()
//{
//    string path = PROGETTI + cliente.getNomeCliente() + @"\";
//    if (Directory.Exists(path))
//    {
//        foreach (string proj in Directory.GetDirectories(path))
//        {
//            allDate.Add(proj.Split('\\').Last(), modificheLiv2(proj));
//        }
//    }

//    //foreach (KeyValuePair<string, DateTime> i in allDate)
//    //{
//    //    Console.WriteLine(i.ToString() + " ");
//    //}
//    confronto();
//}

//private DateTime modificheLiv2(string proj)
//{
//    DateTime dtNew = new DateTime();
//    if (Directory.Exists(proj))
//    {
//        DateTime dt = Directory.GetLastWriteTime(proj);
//        if (DateTime.Compare(dtNew, dt) < 0)
//        {
//            //Console.WriteLine("più nuovo liv1");
//            dtNew = dt;
//        }
//        foreach (string c in Directory.GetDirectories(proj))
//        {
//            dt = Directory.GetLastWriteTime(c);
//            if (DateTime.Compare(dtNew, dt) < 0)
//            {
//                //Console.WriteLine("più nuovo liv2");
//                dtNew = dt;
//            }
//        }
//    }
//    return dtNew;
//}

//private List<string> confronto()
//{
//    string file = @"R:\PROGETTI\DATA\SRC\DATI\CLIENTI\" + cliente.getNomeCliente() + "date.csv";
//    List<string> daControllare = new List<string>();
//    List<string> lines = new List<string>();
//    using (var reader = new CsvFileReader(file))
//    {
//        while (reader.ReadRow(lines))
//        {
//            if (lines.Count != 0)
//            {
//                //Console.WriteLine("letto: " + lines[0]);
//                DateTime tempDate;
//                if (allDate.TryGetValue(lines[0], out tempDate))
//                {
//                    //Console.WriteLine("trovato " + tempDate + "  " + DateTime.Parse(lines[1]));
//                    if (DateTime.Compare(DateTime.Parse(tempDate.ToString()), DateTime.Parse(lines[1])) > 0)
//                    {
//                        daControllare.Add(lines[0]);
//                        Console.WriteLine("aggiunto " + lines[0] + " < " + tempDate + ">  <" + DateTime.Parse(lines[1]) + ">");
//                    }
//                }
//            }
//            else
//            {
//                Console.WriteLine("vuoto");
//            }
//        }
//    }
//    Console.WriteLine("DA CONTROLLARE: " + daControllare.Count);
//    return daControllare;
//}

