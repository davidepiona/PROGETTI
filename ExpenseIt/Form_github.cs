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
    public partial class Form_github : Form
    {
        string valoreIniziale = "@@@";
        public Form_github()
        {
            InitializeComponent();
            textBox1.Text = Globals.GITURL;
            textBox2.Text = Globals.GITPATH;
            
        }

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
                  nonEsiste +"Salvare l'attuale percorso dell'eseguibile e l'attuale indirizzo per il repository GitHub usato dall'applicazione?",
                  "Conferma",
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Question,  System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("Entrato text changed box1: " + pictureBox1.Visible + " box2: "+pictureBox2.Visible);
            //if (!pictureBox1.Visible && !pictureBox2.Visible)
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
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
