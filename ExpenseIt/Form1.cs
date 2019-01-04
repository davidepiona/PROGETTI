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
    public partial class Form1: Form
    {   
         
        public Form1(Cliente cliente)
        {
            InitializeComponent();
            List<Cliente> clienti = new List<Cliente>();
            var reader = new StreamReader(File.OpenRead(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI.csv"));
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                if (line.Length == 4)
                {
                    clienti.Add(new Cliente(line[0], line[1], Int32.Parse(line[2]), Int32.Parse(line[3])));
                    this.comboBox1.Items.Add(line[0]);
                }

            }
            //seleziono quello che ho passato
            this.comboBox1.SelectedItem = cliente.getNomeCliente();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {            string cliente = comboBox1.Text.ToString();
            string titolo = textBox1.Text.ToString();
            string tipoPLC = textBox2.Text.ToString();
            string tipoOP = textBox3.Text.ToString();
            string data = textBox4.Text.ToString();

            //leggere l'attuale file coi progetti di quel cliente
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente + ".txt");
            int num_progetti;
            Int32.TryParse(lines[0], out num_progetti);
            Console.WriteLine("\n ultimo progetto= " + num_progetti);
            Console.WriteLine("\n formattazione {0:D3}", num_progetti);
            string numero = string.Format("{0:D3}", num_progetti + 1);

            string file = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente + ".csv";
            string projectDetails = numero + Environment.NewLine + titolo + Environment.NewLine + tipoPLC + Environment.NewLine + tipoOP + Environment.NewLine + data + Environment.NewLine;
            File.AppendAllText(file, projectDetails);


            //creare nuova cartella in progetti e riempirla dei file iniziali
            Directory.CreateDirectory(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + cliente + @"\" + cliente + numero);
            string fileName = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + cliente + @"\" + cliente + numero + @"\progetto.docx";
            // Create a document in memory: 
            var doc = Xceed.Words.NET.DocX.Create(fileName);
            // Insert a paragrpah:
            doc.InsertParagraph("TITOLO DEL PROGETTO:" + titolo);
            doc.InsertParagraph("\n TIPO DI PLC: " + tipoPLC);
            doc.InsertParagraph("\n TIPO DI OP: " + tipoOP);
            doc.InsertParagraph("\n DATA INIZIO: " + data);
            doc.InsertParagraph("\n Note:");
            // Save to the output directory:
            doc.Save();


            //crea nuovo file cliente coi progetti aggiornati

            lines[0] = numero;
            File.WriteAllLines(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente + ".txt", lines);
            using (StreamWriter sw = File.AppendText(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\" + cliente + ".txt"))
            {
                sw.WriteLine(titolo + " " + tipoPLC + " " + tipoOP + " " + data);
            }

            this.Close();



        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
