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
    /// Form per la creazione di un nuovo cliente
    /// - verifica che siano immessi dati adeguati
    /// - aggiunge il cliente alla lista e al file .csv CLIENTI
    /// - crea la cartella e i file .csv
    /// </summary>
    public partial class Form_nuovoCliente : Form
    {
        public Form_nuovoCliente()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Metodo che prova a creare un nuovo cliente.
        /// - verifica che nome e suffisso non siano stringhe vuote e che il suffisso non abbia caratteri non alfanumerici
        /// - aggiunge il cliente al file CLIENTI.csv
        /// - crea la cartella per i progetti (se non esiste)
        /// - crea i file *CLIENTE*.csv e *CLIENTE*data.csv (se non esistono)
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string nome = textBox1.Text.ToString();
            string suffisso = textBox2.Text.ToString();
            if (nome.Equals(""))
            {
                MessageBox.Show("Inserire un nome valido", "Nome assente",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Warn("Nome cliente assente");
                return;
            }
            if (suffisso.Equals(""))
            {
                MessageBox.Show("Inserire un suffisso valido.", "Suffisso assente",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Warn("Suffisso cliente assente");
                return;
            }
            foreach (char c in suffisso)
            {
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '1' || c > '9'))
                {
                    MessageBox.Show("Inserire un suffisso composto esclusivamente da lettere e numeri.", "Suffisso non alfanumerico",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    Globals.log.Warn("Suffisso cliente non alfanumerico");
                    return;
                }
            }
            nome = nome.Replace(" ", "_");
            string file = Globals.DATI + @"\CLIENTI.csv";
            try
            {
                string clientDetails = nome + "," + suffisso + "," + 0 + "," + 0;
                File.AppendAllText(file, clientDetails);
                Globals.CLIENTI.Add(new Cliente(nome, suffisso, 0, 0));
                Console.WriteLine("Nuovo cliente= " + clientDetails);
                Globals.log.Info("Nuovo cliente= " + clientDetails);
                if (!Directory.Exists(Globals.PROGETTI + nome))
                {
                    try
                    {
                        Directory.CreateDirectory(Globals.PROGETTI + nome);
                        string msg = "Creata cartella: " + Globals.PROGETTI + nome;
                        Console.WriteLine(msg);
                        Globals.log.Info(msg);
                    }
                    catch (IOException)
                    {
                        string msg = "E16 - La cartella " + Globals.PROGETTI + nome + " non è stato creata per un problema";
                        MessageBox.Show(msg, "E16"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                }
                if (!File.Exists(Globals.DATI + nome + ".csv"))
                {
                    try
                    {
                        File.Create(Globals.DATI + nome + ".csv");
                        string msg = "Creato file: " + Globals.DATI + nome + ".csv";
                        Console.WriteLine(msg);
                        Globals.log.Info(msg);
                    }
                    catch (IOException)
                    {
                        string msg = "E17 - Il file " + Globals.DATI + nome + ".csv" + " non è stato creata per un problema";
                        MessageBox.Show(msg, "E17"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                    try
                    {
                        File.Create(Globals.DATI + nome + "date.csv");
                        string msg = "Creato file: " + Globals.DATI + nome + "date.csv";
                        Console.WriteLine(msg);
                        Globals.log.Info(msg);
                    }
                    catch (IOException)
                    {
                        string msg = "E18 - Il file " + Globals.DATI + nome + "date.csv" + " non è stato creata per un problema";
                        MessageBox.Show(msg, "E18"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                }
                this.Close();
            }
            catch (IOException)
            {
                string msg = "E19 - Il file " + file + " non esiste o è aperto da un altro programma";
                MessageBox.Show(msg, "E19", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }
    }
}
