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
            String nome = textBox1.Text.ToString();
            string suffisso = textBox2.Text.ToString();

            if (nome.Equals(""))
            {
                MessageBox.Show("Inserire un nome valido", "Nome assente", 
                    MessageBoxButtons.OK , MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 ,MessageBoxOptions.RightAlign);
                return;
            }
            
            //Console.WriteLine("PRIMA: " + nome);
            nome = nome.Replace(" ", "_");

            if (suffisso.Equals(""))
            {
                MessageBox.Show("Inserire un suffisso valido.", "Suffisso assente", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                return;
            }

            foreach(char c in suffisso)
            {
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z')&& (c < '1' || c > '9') )

                    {
                    MessageBox.Show("Inserire un suffisso composto esclusivamente da lettere e numeri.Inserire un suffisso composto esclusivamente da lettere e numeri.Inserire un suffisso composto esclusivamente da lettere e numeri.", "Suffisso non alfanumerico", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

                    return;

                    }
            }

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
                        MessageBox.Show("E16 - La cartella " + Globals.PROGETTI + nome + " non è stato creata per un problema", "E16"
                                     ,MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
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
                        MessageBox.Show("E17 - Il file " + Globals.DATI + nome + ".csv" + " non è stato creata per un problema", "E17"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    }
                    try
                    {
                        //creare nuovo file CSV in DATI
                        File.Create(Globals.DATI + nome + "date.csv");
                        Console.WriteLine("Creato file: " + Globals.DATI + nome + "date.csv");
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("E18 - Il file " + Globals.DATI + nome + "date.csv" + " non è stato creata per un problema", "E18"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    }
                }


                Globals.CLIENTI.Add(new Cliente(nome, suffisso, 0, 0));
                MainWindow m = new MainWindow();
                m.salvaClientiCSV();

                this.Close();
                
            }
            catch (IOException)
            {
                MessageBox.Show("E19 - Il file " + file + " non esiste o è aperto da un altro programma", "E19"
                                     , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
            }
        }
    }
}
