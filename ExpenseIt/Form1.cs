using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ExpenseIt
{
    /// <summary>
    /// Form per la creazione di un nuovo progetto
    /// - verifica che siano immessi dati adeguati
    /// - aggiunge il progetto alla lista e al file *CLIENTE*.csv
    /// </summary>
    public partial class Form1 : Form
    {
        private int ultimoProgetto;

        /// <summary>
        /// Costruttore che carica i valori dei possibili clienti e seleziona l'attuale
        /// Riceve il numero dell'ultimoProgetto creato, così da memorizzare il nuovo progetto con il numero seguente
        /// </summary>
        public Form1(Cliente cliente, int ultimoProgetto)
        {
            InitializeComponent();
            foreach (Cliente c in Globals.CLIENTI)
            {
                this.comboBox1.Items.Add(c.getNomeCliente());
            }
            this.ultimoProgetto = ultimoProgetto;
            this.comboBox1.SelectedItem = cliente.getNomeCliente();
        }

        // <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Metodo che prova a creare un nuovo progetto.
        /// - se qualcuno dei parametri è una stringa vuota lo sostituisce con un '.'
        /// - data acquisisce il valore del timestamp attuale
        /// - cerca il numero del cliente selezionato per questo progetto
        /// - aggiugne il progetto al file *CLIENTE*.csv in DATI
        /// - crea la cartella del progetto (se non esiste)
        /// - crea il file docx con dentro le informazioni di base
        /// - aggiorna MaxId e LastId e scrive il file CLIENTI.csv
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string cliente = comboBox1.Text.ToString();
            string titolo = textBox1.Text.ToString();
            string tipoPLC = textBox2.Text.ToString();
            string tipoOP = textBox3.Text.ToString();
            if (titolo.Equals(""))
                titolo = ".";
            if (tipoPLC.Equals(""))
                tipoPLC = ".";
            if (tipoOP.Equals(""))
                tipoOP = ".";
            string data = (DateTime.Now).ToString();
            int numCliente = 0;
            int i = 0;
            foreach (Cliente c in Globals.CLIENTI)
            {
                if (c.getNomeCliente().Equals(cliente))
                {
                    numCliente = i;
                }
                i++;
            }
            string numProgetto = (ultimoProgetto + 1).ToString();
            string file = Globals.DATI + cliente + ".csv";
            try
            {
                string projectDetails = numProgetto + Environment.NewLine + titolo + Environment.NewLine + tipoPLC + Environment.NewLine + tipoOP + Environment.NewLine + data + Environment.NewLine;
                File.AppendAllText(file, projectDetails);
                string msg = "Nuovo progetto; l'ultimo aveva indice: " + numProgetto;
                Console.WriteLine(msg);
                Globals.log.Info(msg);
                try
                {
                    Directory.CreateDirectory(Globals.PROGETTI + cliente + @"\" + cliente + numProgetto);
                }
                catch (IOException)
                {
                    string msg2 = "E09 - La cartella " + Globals.PROGETTI + cliente + @"\" + cliente + numProgetto + " non è stata creata per un problema";
                    MessageBox.Show(msg2, "E09"
                                         , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg2);
                }
                string fileName = Globals.PROGETTI + cliente + @"\" + cliente + numProgetto + @"\progetto.docx";
                try
                {
                    var doc = Xceed.Words.NET.DocX.Create(fileName);
                    doc.InsertParagraph(cliente + " " + numProgetto).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO:" + titolo);
                    doc.InsertParagraph("\n TIPO DI PLC: " + tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
                catch (IOException)
                {
                    string msg2 = "E07 - Il file " + fileName + " non è stato creato per un problema";
                    MessageBox.Show(msg2, "E07", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg2);
                }
                Globals.CLIENTI[numCliente].setMaxId(Globals.CLIENTI[numCliente].getMaxId() + 1);
                Globals.CLIENTI[numCliente].setLastId(Globals.CLIENTI[numCliente].getMaxId());
                MainWindow m = new MainWindow();
                m.salvaClientiCSV();
                Globals.log.Info("Aggiunto progetto");
                this.Close();
            }
            catch (IOException)
            {
                string msg = "E06 - Il file " + file + " non esiste o è aperto da un altro programma";
                MessageBox.Show(msg, "E06" , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }
    }
}
