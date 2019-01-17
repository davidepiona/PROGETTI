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

namespace ExpenseIt
{
    public partial class Form_nuovoCliente : Form
    {
        public Form_nuovoCliente()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nome = textBox1.Text.ToString();
            string suffisso = textBox2.Text.ToString();

            if (nome.Equals(""))
                nome = ".";
            if (suffisso.Equals(""))
                suffisso = ".";

            string file = Globals.DATI + @"\CLIENTI.csv";
            try
            {
                string clientDetails = nome + ","+ suffisso+ ","+ 0 + ","+ 0;
                File.AppendAllText(file, clientDetails);
                Console.WriteLine("\n Nuovo cliente= " + clientDetails);
                if (!Directory.Exists(Globals.PROGETTI + nome))
                {
                    try
                    {
                        //creare nuova cartella in progetti
                        Directory.CreateDirectory(Globals.PROGETTI + nome);
                        Console.WriteLine("Creata cartella: " + Globals.PROGETTI + nome);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("E16 - La cartella " + Globals.PROGETTI + nome + " non è stato creata per un problema");
                    }
                }
                if (!File.Exists(Globals.DATI + nome + ".csv"))
                {
                    try
                    {
                        //creare nuovo file CSV in DATI
                        File.Create(Globals.DATI + nome + ".csv");
                        Console.WriteLine("Creato file: " + Globals.DATI + nome + ".csv");
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("E17 - Il file " + Globals.DATI + nome + ".csv" + " non è stato creata per un problema");
                    }
                    try
                    {
                        //creare nuovo file CSV in DATI
                        File.Create(Globals.DATI + nome + "date.csv");
                        Console.WriteLine("Creato file: " + Globals.DATI + nome + "date.csv");
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("E18 - Il file " + Globals.DATI + nome + "date.csv" + " non è stato creata per un problema");
                    }
                }


                Globals.CLIENTI.Add(new Cliente(nome, suffisso, 0, 0));
                MainWindow m = new MainWindow();
                m.salvaClientiCSV();

                this.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("E17 - Il file " + file + " non esiste o è aperto da un altro programma");
            }
        }
    }
}
