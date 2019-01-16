using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ExpenseIt
{
    public partial class Form_github : Form
    {
        public Form_github()
        {
            InitializeComponent();
            textBox1.Text = Globals.GITURL;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tb1 = textBox1.Text.ToString();
            
            System.Windows.MessageBoxResult me = System.Windows.MessageBox.Show(
                  "Salvare l'attuale indirizzo per il repository GitHub usato dall'applicazione?",
                  "Conferma",
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Stop);
                if (me == MessageBoxResult.No)
                {
                    return;
                }
            
            Globals.GITURL = tb1;
            MainWindow m = new MainWindow();
            m.salvaClientiCSV();

            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool trovata = true;
            pictureBox2.Visible = false;
            pictureBox1.Visible = false;
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
                    //MessageBox.Show("non trovate");
                }
                else
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
