using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATA
{
    /// <summary>
    /// Form per la modifica di un progetto esistente
    /// - verifica che siano immessi dati adeguati
    /// - aggiunge il programma alla lista e al file *CLIENTE*.csv
    /// - aggiorna o crea il file .docx
    /// </summary>
    public partial class Form_Modifica : Form
    {

        private Progetto prog;
        private List<Progetto> progetti;
        /// <summary>
        /// Costruttore a cui viene passato  il progetto da modificare
        /// Legge il file tipiPLC.csv
        /// Visualizza i dati relativi a quel progetto
        /// </summary>
        public Form_Modifica(List<Progetto> progetti, Progetto prog)
        {
            this.prog = prog;
            this.progetti = progetti;
            InitializeComponent();
            label4.Text = "Id" + prog.numero;
            
            textBox1.Text = prog.nome;
            textBox3.Text = prog.tipoOP;
            
            foreach (Cliente c in Globals.CLIENTI)
            {
                this.comboBox1.Items.Add(c.getNomeCliente());
            }
            List<string> tipi = readCSV_tipiPLC(Globals.LOG + "tipiPLC.csv");
            foreach (string s in tipi)
            {
                this.comboBox2.Items.Add(s);
            }
            this.comboBox1.SelectedItem = prog.nomeCliente;
            this.comboBox2.SelectedItem= prog.tipoPLC;
        }

        /// <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Metodo che apporta le modifiche all'interno di un progetto.
        /// - se ci sono stringhe vuote le sostituisce con "."
        /// - sostituisce il progetto nella lista progetti
        /// - Appende le modifiche in fondo al file di word (o lo crea se non esiste)
        /// - riscrive il file *CLIENTE*.csv
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            prog.nomeCliente = comboBox1.Text.ToString();
            prog.nome = textBox1.Text.ToString();
            prog.tipoOP = textBox3.Text.ToString();
            prog.tipoPLC = comboBox2.Text.ToString();
            if (prog.nome.Equals(""))
                prog.nome = ".";
            if (prog.tipoOP.Equals(""))
                prog.tipoOP = ".";
            if (prog.tipoPLC.Equals(""))
                prog.tipoPLC = ".";
            if (prog.nomeCliente.Equals(""))
                prog.nomeCliente = ".";
            prog.modifica = (DateTime.Now).ToString();
            progetti[progetti.FindIndex(r => r.numero == prog.numero)] = prog;
            string msg = "Aggiornato il progetto con indice " + prog.numero;
            Console.WriteLine(msg);
            Globals.log.Info(msg);
            string fileName = Globals.PROGETTI + prog.nomeCliente + @"\" + prog.suffisso + prog.numero + @"\progetto.docx";
            try
            {
                if (!File.Exists(fileName))
                {
                    var doc = Xceed.Words.NET.DocX.Create(fileName);
                    doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                    doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
                else
                {
                    var doc = Xceed.Words.NET.DocX.Load(fileName);
                    doc.InsertParagraph("Aggiornamento del " + prog.modifica).Bold();
                    doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                    doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
            }
            catch (IOException)
            {
                string msg2 = "E50 - Il file " + fileName + " non è stato modificato per un problema";
                MessageBox.Show(msg2, "E50", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg2);
            }
            if (scriviCSV())
            {
                Globals.log.Info("Progetto modificato correttamente");
                System.Windows.MessageBox.Show("Progetto modificato correttamente", "Modificato", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
            else
            {
                Globals.log.Error("Progetto NON modificato");
                System.Windows.MessageBox.Show("Progetto NON modificato", "NON modificato", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
            this.Close();

        }

        /// <summary>
        /// Metodo per l'eliminazione del progetto selezionato
        /// - chiede all'utente conferma dell'operazione
        /// - rimuove il progetto dalla lista progetti
        /// - riscrive il file *CLIENTE*.csv
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Sei sicuro di voler ELIMINARE il progetto " +
                prog.nome + " con codice " + prog.nomeCliente + prog.numero + "?",
               "Conferma Eliminazione", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
            if (dialogResult == System.Windows.MessageBoxResult.Yes)
            {
                if (prog.numero != 0)
                {
                    Console.WriteLine("Rimuovo");
                    progetti.Remove(prog);
                    int numCliente = 0;
                    int i = 0;
                    foreach (Cliente c in Globals.CLIENTI)
                    {
                        if (c.getNomeCliente().Equals(prog.nomeCliente))
                        {
                            numCliente = i;
                        }
                        i++;
                    }
                    Globals.CLIENTI[numCliente].setMaxId(progetti.Count);
                    Globals.CLIENTI[numCliente].setLastId(prog.numero+1);
                    if (scriviCSV())
                    {
                        Globals.log.Info("Progetto eliminato");
                        System.Windows.MessageBox.Show("Progetto eliminato", "Eliminato", System.Windows.MessageBoxButton.OK,
                               System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                    }
                    else
                    {
                        Globals.log.Error("Progetto NON eliminato");
                        System.Windows.MessageBox.Show("Progetto NON eliminato", "NON Eliminato", System.Windows.MessageBoxButton.OK,
                               System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                    }
                }
            }
            this.Close();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             SCRIVI E LEGGI CSV                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Metodo che legge i tipi di PLC da tipiPLC.csv in DATA e li restituisce
        /// Restituisce false se per qualche ragione è stata sollevata un IOException o FormatException
        /// </summary>
        public List<string> readCSV_tipiPLC(string file)
        {
            List<string> tipi = new List<string>();
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
                            tipi.Add(lines[0]);
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
                string msg = "E45 - Il file " + file + " non esiste o è aperto da un altro programma";
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return null;
            }
            catch (FormatException)
            {
                string msg = "E46 - Il file " + file + " è in un formato non corretto.\nProblema riscontrato all riga numero: " + j;
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return null;
            }
            Globals.log.Info("TipiPLC letti da tipiPLC.csv");
            return tipi;
        }

        /// <summary>
        /// Metodo per la riscrittura di progetti nel file *CLIENTE*.csv
        /// </summary>
        private bool scriviCSV()
        {
            List<string> lines = new List<string>();
            int i = 0;
            foreach (Progetto p in progetti)
            {
                lines.Add(p.numero + Environment.NewLine + p.nome + Environment.NewLine + p.tipoPLC + Environment.NewLine + p.tipoOP +
                    Environment.NewLine + p.data);
                i++;
            }
            try
            {
                File.WriteAllLines(Globals.DATI + prog.nomeCliente + ".csv", lines);
            }
            catch (IOException)
            {
                string msg = "E51 - errore nella scrittura del file";
                System.Windows.MessageBox.Show(msg, "E51", System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            return true;
        }
    }
}
