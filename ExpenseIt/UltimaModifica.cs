﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    class UltimaModifica
    {
        private Cliente cliente;
        private Dictionary<string, DateTime> allDate = new Dictionary<string, DateTime>();
        private Dictionary<string, DateTime> allDateSync = new Dictionary<string, DateTime>();
        private Dictionary<string, int> status = new Dictionary<string, int>();
        DateTime dtNew = new DateTime();

        public UltimaModifica(Cliente cliente)
        {
            this.cliente = cliente;

        }

        public Dictionary<string, DateTime> getAllDate()
        {
            return allDate;
        }


        //public void confrontoSync(List<Progetto> progetti)
        //{
        //    Dictionary<string, int> merge = allDate.Concat(allDateSync).GroupBy(d => d.Key)
        //    .ToDictionary(d => d.Key, d => 1);

        //    foreach (KeyValuePair<string, int> i in merge)
        //    {


        //    bool pc = allDate.TryGetValue(i.Key, out DateTime datePc);
        //    bool sync = allDateSync.TryGetValue(i.Key, out DateTime dateSync);

        //    if (pc & sync)
        //    {
        //        if (DateTime.Compare(datePc, dateSync) > 0)
        //        {
        //            status.Add(i.Key, 2);
        //        }
        //        else if (DateTime.Compare(datePc, dateSync) < 0)
        //        {
        //                status.Add(i.Key, 0);
        //            }
        //            else
        //            {
        //                status.Add(i.Key, 1);
        //            }
        //        }
        //    else if (pc)
        //    {
        //            status.Add(i.Key, 2);
        //        }
        //    else if (sync)
        //    {
        //            status.Add(i.Key, 0);
        //        }

        //}
        //    foreach (KeyValuePair<string, int> i in status)
        //    {
        //        Console.WriteLine(i);
        //    }
        //}
        public void aggiornoModifiche(List<Progetto> progetti)
        {
            Console.Write("Aggiorno Modifiche");
            foreach (Progetto p in progetti)
            {
                if (allDate.TryGetValue(p.numero, out DateTime ultima))
                {
                    p.modifica = ultima.ToString();
                }
            }
        }
        public void confrontoSync(List<Progetto> progetti)
        {
            Console.Write("Syncronizzo");
            foreach (Progetto p in progetti)
            {
                bool pc = allDate.TryGetValue(p.numero, out DateTime datePc);
                bool sync = allDateSync.TryGetValue(p.numero, out DateTime dateSync);
                Console.WriteLine("SY: " + p.numero + "  pc: " + datePc + "  sync: " + dateSync);
                if (pc & sync)
                {
                    if (DateTime.Compare(DateTime.Parse(datePc.ToString()),DateTime.Parse(dateSync.ToString())) > 0)
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
            //foreach (Progetto i in progetti)
            //{
            //    Console.WriteLine(i);
            //}
        }
        public void ricercaLenta(string path)
        {
            //string path = PROGETTI + cliente.getNomeCliente() + @"\";
            allDate = new Dictionary<string, DateTime>();
            if (Directory.Exists(path))
            {
                foreach (string proj in Directory.GetDirectories(path))
                {
                    DateTime res = modificheByFile(proj);
                    allDate.Add(proj.Split('\\').Last(), res);
                    Console.WriteLine("FINE, DATA PIU' RECENTE: <" + res + ">");
                }
            }

        }


        public DateTime modificheByFile(string proj)
        {
            Console.WriteLine("valuto la cartella " + proj);
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

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            //DateTime dt = Directory.GetLastWriteTime(targetDirectory);
            //Console.WriteLine(targetDirectory + "  data:<" + dt + ">");

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            //Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);


        }

        // Insert logic for processing found files here.
        private void ProcessFile(string path)
        {
            DateTime dt = File.GetLastWriteTime(path);

            if (DateTime.Compare(dtNew, dt) < 0)
            {
                dtNew = dt;
            }
        }

        public void writeInCSV(string file)
        {
            //string file = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente.getNomeCliente() + "date.csv";
            string projectDate = "";
            foreach (KeyValuePair<string, DateTime> i in allDate)
            {
                projectDate += i.Key + "," + i.Value + Environment.NewLine;
            }
            File.WriteAllText(file, projectDate);
        }

        public void readByCSV(string file)
        {
            //string file = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente.getNomeCliente() + "date.csv";
            List<string> lines = new List<string>();
            try
            {
                using (var reader = new CsvFileReader(file))
                {
                    while (reader.ReadRow(lines))
                    {
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
            }catch(FileNotFoundException) { }
        }

        public void readSync(string file)
        {
            //string file = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync\CLIENTI\" + cliente.getNomeCliente() + "date.csv";
            allDateSync = new Dictionary<string, DateTime>();
            List<string> lines = new List<string>();
            using (var reader = new CsvFileReader(file))
            {
                while (reader.ReadRow(lines))
                {
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
        //    string file = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente.getNomeCliente() + "date.csv";
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
    }
}