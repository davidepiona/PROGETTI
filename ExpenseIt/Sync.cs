using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExpenseIt
{
    class Sync
    {
        private List<Progetto> progetti = new List<Progetto>();
        private List<Progetto> progettiSync = new List<Progetto>();
        private int num_cliente;

        public Sync(List<Progetto> progetti, int num_cliente)
        {
            this.progetti = progetti;
            this.num_cliente = num_cliente;
        }

        public void readSyncProject(string path)
        {
            Console.WriteLine("Read Sync Projects");
            List<string> lines = new List<string>();
            //Console.WriteLine(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + ".csv");
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
                        progettiSync.Add(new Progetto(num, nome, tipoOP, tipoOP, data, Globals.CLIENTI[num_cliente].getNomeCliente(), Globals.CLIENTI[num_cliente].getSuffisso()));
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("E09 - Il file " + path + " non esiste o è aperto da un altro programma");
            }
        }

        public List<Progetto>[] compareSyncProject()
        {
            /* se l'array che ritorna si chiama "compare"
             * Console.WriteLine("Progetti uguali = "+ compare[0].Count +"\nProgetti mancanti localmente = "+ compare[1].Count + "\nProgetti in più = "+ compare[2].Count);
             */
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
                    list[2].Add(p);
                }
            }
            foreach(Progetto ps in progettiSync)
            {
                if (ps.sync==false)
                {
                    Console.WriteLine("Un progetto mancante localmente: " + ps.sigla + " -->" + ps.data);
                    list[1].Add(ps);
                }
            }
            return list;
        }

        private List<Progetto> progettiUguali(){
           return null;
        }

        
    }
}
