using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ExpenseIt
{
    /// <summary>
    /// Form per il cambio del percorso dell'eseguibile git.exe e il cambio del repository github
    /// - verifica che il percorso per l'eseguibile porti ad un file esistente
    /// - verifica se il repository all'indirizzo fornito è raggiungibile
    /// </summary>
    public partial class Form_github : Form
    {
        string valoreIniziale = "@@@";

        /// <summary>
        /// Costruttore classico in cui vengono impostati i valori delle textbox
        /// </summary>
        public Form_github()
        {
            InitializeComponent();
            textBox1.Text = Globals.GITURL;
            textBox2.Text = Globals.GITPATH;
        }

        /// <summary>
        /// Metodo che si attiva quando viene modificato il testo nella textbox del repository
        /// - controlla se il valore iniziale è '@@@' (ciò indicherebbe che è già in corso una verifica dell'indirizzo, quindi interrompe il metodo)
        /// - toglie entrambe le icone V e X
        /// - NUOVO TASK 
        ///     - git ls-remote GITURL
        ///     - se restituisce un messaggio che inizia con "fatal", "remote" o "git" allora NON esiste, altrimenti SI --> imposto le icone X e V
        ///     - se uscendo mi accorgo che nel frattempo è cambiata la stringa chiamo il metodo secondoGiro
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Console.WriteLine("VALORE INIZIALE: <" + valoreIniziale + ">");
            if (!valoreIniziale.Equals("@@@"))
            {
                Console.WriteLine("cambiato mentre ancora cercavo, ESCO");
                return;
            }
            bool trovata = true;
            pictureBox2.Visible = false;
            pictureBox1.Visible = false;
            valoreIniziale = textBox1.Text.ToString();
            Task.Factory.StartNew(() =>
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\Git\cmd\git.exe",
                        Arguments = "ls-remote " + textBox1.Text.ToString(),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (!proc.StandardError.EndOfStream)
                {
                    string err = proc.StandardError.ReadLine();
                    Console.WriteLine("ERR: " + err);
                    string[] split = err.Split(':');
                    if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                    {
                        trovata = false;
                    }
                }
            }).ContinueWith(task =>
            {
                if (!trovata)
                {
                    pictureBox2.Visible = false;
                    pictureBox1.Visible = true;
                }
                else
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                }
                Console.WriteLine("USCENDO VEDO: "+textBox1.Text.ToString()+ "  PRIMA VEDEVO: "+ valoreIniziale);
                if (!textBox1.Text.ToString().Equals(valoreIniziale))
                    {
                        Console.WriteLine("ALTRO GIRO");
                        secondoGiro();
                    }
                else
                {
                    valoreIniziale = "@@@";
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Esegue continuamente la verifica se l'attuale indirizzo è raggiungibile fino a quando al termine della
        /// verifica non trova la stessa stringa che ha appena verificato. 
        /// Al termine reimposta la stringa '@@@' in modo che sia nuovamente accessibile il metodo textBox1_TextChanged
        /// </summary>
        private void secondoGiro()
        {
            bool trovata = true;
            pictureBox2.Visible = false;
            pictureBox1.Visible = false;
            string valoreIniziale2 = textBox1.Text.ToString();
            Task.Factory.StartNew(() =>
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\Git\cmd\git.exe",
                        Arguments = "ls-remote " + textBox1.Text.ToString(),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (!proc.StandardError.EndOfStream)
                {
                    string err = proc.StandardError.ReadLine();
                    Console.WriteLine("ERR: " + err);
                    string[] split = err.Split(':');
                    if (split[0].Equals("remote") || split[0].Equals("fatal") || split[0].Equals("git"))
                    {
                        trovata = false;
                    }
                }
            }).ContinueWith(task =>
            {
                if (!trovata)
                {
                    pictureBox2.Visible = false;
                    pictureBox1.Visible = true;
                }
                else
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                }
                Console.WriteLine("2) USCENDO VEDO: " + textBox1.Text.ToString() + "  PRIMA VEDEVO: " + valoreIniziale2);
                if (!textBox1.Text.ToString().Equals(valoreIniziale2))
                {
                    Console.WriteLine("2) ALTRO GIRO");
                    secondoGiro();
                }
                else
                {
                    valoreIniziale = "@@@";
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Quando viene modificato il testo della textbox verifica se esiste il file nel percorso specificato.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(textBox2.Text.ToString()))
            {
                pictureBox4.Visible = true;
            }
            else
            {
                pictureBox4.Visible = false;
            }
        }

        /// <summary>
        /// Apre la finestra per navigare il filesystem e scrive il risultato nella textbox
        /// </summary>
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = textBox2.Text;
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.FileName = textBox2.Text.Split('\\').Last();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                textBox2.Text = path;
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
        /// Da all'utente la possibilità di scegliere se apportare le modifiche o no.
        /// In seguito aggiorna Globals.GITURL e Globals.GITPATH (se modificati) e riscrive il file SETTINGS.csv
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string tb1 = textBox1.Text.ToString();
            string tb2 = textBox2.Text.ToString();
            string nonEsiste = "";
            if (!File.Exists(tb2))
            {
                nonEsiste = "Il percorso impostato per l'eseguibile 'git.exe' non esiste.\n";
            }
            MessageBoxResult me = System.Windows.MessageBox.Show(
                  nonEsiste + "Salvare l'attuale percorso dell'eseguibile e l'attuale indirizzo per il repository GitHub usato dall'applicazione?",
                  "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            if (me == MessageBoxResult.No)
            {
                return;
            }
            if (!tb1.Equals(Globals.GITURL) || !tb1.Equals(Globals.GITPATH))
            {
                Globals.GITURL = tb1;
                Globals.GITPATH = tb2;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
                Globals.log.Info("Github settings");
            }
            this.Close();
        }
    }
}
