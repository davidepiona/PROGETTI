﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATA
{
    /// <summary>
    /// Classe per la gestione della sicronizzazione dei progetti locali con quelli in DATIsync che consente di scoprire quali progetti sono assenti
    /// - legge i progetti memorizzati nei file selezionati (pensato per quelli interni alla cartella DATIsync) e li memorizza in progettiSync
    /// - confronta progetti e progettiSync (guardando la data di creazione)
    /// </summary>
    class Sync
    {
        private List<Progetto> progetti = new List<Progetto>();
        private List<Progetto> progettiSync = new List<Progetto>();
        private int num_cliente;

        /// <summary>
        /// Costruttore classico, inizializza gli attributi con i valori che gli vengono passati
        /// </summary>
        public Sync(List<Progetto> progetti, int num_cliente)
        {
            this.progetti = progetti;
            this.num_cliente = num_cliente;
        }

        /// <summary>
        /// Legge dai file contenenti i progetti di un cliente tutte le informazioni e le scrive in progettiSync.
        /// Pensato per leggere i file contenuti nella cartella DATIsync e confrontarli con quelli in DATI.
        /// </summary>
        public void readSyncProject(string path)
        {
            Console.WriteLine("Read Sync Projects");
            Globals.log.Info("Read Sync Projects");
            List<string> lines = new List<string>();
            int j = 0;
            try
            {
                using (var reader = new CsvFileReader(path))
                {
                    while (reader.ReadRow(lines) && lines.Count != 0 && lines != null)
                    {
                        int num = Int32.Parse(lines[0]);
                        reader.ReadRow(lines);
                        string nome = lines[0];
                        reader.ReadRow(lines);
                        string tipoPLC = lines[0];
                        reader.ReadRow(lines);
                        string tipoOP = lines[0];
                        reader.ReadRow(lines);
                        string data = lines[0];
                        progettiSync.Add(new Progetto(num, nome, tipoPLC, tipoOP, data, Globals.CLIENTI[num_cliente].getNomeCliente(), Globals.CLIENTI[num_cliente].getSuffisso()));
                        j++;
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E09 - Il file " + path + " non esiste o è aperto da un altro programma";
                MessageBox.Show(msg, "E09"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            catch (FormatException)
            {
                string msg = "E32 - Il file " + path + " è in un formato non corretto.\nProblema riscontrato al progetto numero: " + j;
                MessageBox.Show(msg, "E32"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Confronta le date di CREAZIONE dei progetti in progetti e in progettiSync.
        /// Restituisce un array contenente 3 liste di progetti
        /// - array[0] = contiene i progetti uguali
        /// - array[1] = contiene i progetti mancanti localmente
        /// - array[2] = contiene i progetti in più
        /// </summary>
        public List<Progetto>[] compareSyncProject()
        {
            List<Progetto>[] list = new List<Progetto>[3];
            list[0] = new List<Progetto>();
            list[1] = new List<Progetto>();
            list[2] = new List<Progetto>();
            foreach (Progetto p in progetti)
            {   
                int i = progettiSync.FindIndex(x => x.data.Equals(p.data));
                if (i !=-1 )
                {
                    list[0].Add(p);
                    progettiSync[i].sync = true;
                }
                else
                {
                    Console.WriteLine("Un progetto in più: " + p.sigla + " -->"+ p.data);
                    Globals.log.Info("Un progetto in più: " + p.sigla + " -->"+ p.data);
                    list[2].Add(p);
                }
            }
            foreach(Progetto ps in progettiSync)
            {
                if (ps.sync==false)
                {
                    Console.WriteLine("Un progetto mancante localmente: " + ps.sigla + " -->" + ps.data);
                    Globals.log.Info("Un progetto mancante localmente: " + ps.sigla + " -->" + ps.data);
                    list[1].Add(ps);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Confronto> compareProjectsLists()
        {
            List<Confronto> list = new List<Confronto>();
            foreach (Progetto p in progetti)
            {
                int i = progettiSync.FindIndex(x => x.numero.Equals(p.numero));
                if (i != -1) //trovato
                {
                    progettiSync[i].sync = true;
                    //if (p.nome.Equals(progettiSync[i].nome) && p.data.Equals(progettiSync[i].data))
                    string data1 = p.data;
                    string data2 = progettiSync[i].data;
                    if (data1.Length != data2.Length)
                    {
                        //Console.WriteLine(data1 + "\n" + data2);
                        data1 = data1.Substring(0, 16);
                        data2 = data2.Substring(0, 16);
                    }
                    if (p.nome.Equals(progettiSync[i].nome) && data1.Equals(data2))
                    {

                    }
                    else
                    {
                        Confronto c = new Confronto(p, progettiSync[i]);
                        list.Add(c);
                    }
                }
                else
                {
                    Console.WriteLine("Un progetto in più: " + p.sigla + " -->" + p.data);
                    Globals.log.Info("Un progetto in più: " + p.sigla + " -->" + p.data);
                    Confronto c = new Confronto(p, null);
                    list.Add(c);
                }
            }
            foreach (Progetto ps in progettiSync)
            {
                if (ps.sync == false)
                {
                    Console.WriteLine("Un progetto mancante localmente: " + ps.sigla + " -->" + ps.data);
                    Globals.log.Info("Un progetto mancante localmente: " + ps.sigla + " -->" + ps.data);
                    Confronto c = new Confronto(null, ps);
                    list.Add(c);
                }
            }
            return list;
        }
    }
}
