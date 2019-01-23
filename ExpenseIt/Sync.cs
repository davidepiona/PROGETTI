using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpenseIt
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
            List<string> lines = new List<string>();
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
                MessageBox.Show("E09 - Il file " + path + " non esiste o è aperto da un altro programma", "E09"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
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
    }
}
