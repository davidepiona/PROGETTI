using DATA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace DATA
{
    /// <summary>
    /// Form che permette di selezionare le cartelle di origine e destinazione del processo di 
    /// modifica dei file .csv utilizzati dal programma MATRIX per ottenere dei file .csv leggibili da DATA
    /// </summary>
    public partial class Form_aggiornaCSV : Form
    {
        /// <summary>
        /// Costruttore classico in cui vengono impostati i valori delle textbox
        /// </summary>
        public Form_aggiornaCSV()
        {
            InitializeComponent();
            textBox1.Text = @"R:\PROGETTI\MATRIX\BASE_DATI\";
            textBox2.Text = Globals.DATI;
        }

        /// <summary>
        /// Apre la finestra per navigare il filesystem e scrive il risultato nella textbox
        /// Se il percorso termina senza '\', ne aggiunge una (compatibilità col resto del codice)
        /// </summary>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }
                textBox1.Text = path;
            }
        }

        /// <summary>
        /// Apre la finestra per navigare il filesystem e scrive il risultato nella textbox
        /// Se il percorso termina senza '\', ne aggiunge una (compatibilità col resto del codice)
        /// </summary>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox2.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }
                textBox2.Text = path;
            }
        }

        /// <summary>
        /// Quando viene modificato il testo della textbox verifica se esiste la cartella specificata.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox1.Text.ToString()))
            {
                pictureBox4.Visible = true;
            }
            else
            {
                pictureBox4.Visible = false;
            }
        }

        /// <summary>
        /// Quando viene modificato il testo della textbox verifica se esiste la cartella specificata.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox2.Text.ToString()))
            {
                pictureBox5.Visible = true;
            }
            else
            {
                pictureBox5.Visible = false;
            }
        }


        /// <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Se i percorsi immessi portano a cartelle esistenti effettua la conversione.
        /// A conversione terminata un MessageBox visualizza l'esito ottenuto.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string tb1 = textBox1.Text.ToString();
            string tb2 = textBox2.Text.ToString();
            if (!Directory.Exists(tb1) || !Directory.Exists(tb2))
            {
                string msg = "Uno dei percorsi inseriti non esiste, impossibile eseguire l'operazione.";
                System.Windows.MessageBoxResult me = System.Windows.MessageBox.Show(
                    msg, "Percorso inesistente", MessageBoxButton.OK,
                    MessageBoxImage.Question, MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Warn(msg);
                return;
            }
            ImportaCSV importaCSV = new ImportaCSV(tb1, tb2);
            Globals.log.Info("Importazione CSV MATIX da " + tb1 + " a " + tb2);
            string risultato = "Risultato: \n" + importaCSV.importazione();
            System.Windows.MessageBox.Show(risultato);
            Globals.log.Info(risultato);
            this.Close();
        }
    }
}
