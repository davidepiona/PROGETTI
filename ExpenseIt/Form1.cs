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
        private List<Cliente> clienti = new List<Cliente>();
        private string lastOpen;
        public Form1(Cliente cliente)
        {
            InitializeComponent();
            var file = File.OpenRead(Globals.DATI + @"\CLIENTI.csv");
            var reader = new StreamReader(file);
            string info = reader.ReadLine();
            lastOpen = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                if (line.Length == 4)
                {
                    clienti.Add(new Cliente(line[0], line[1], Int32.Parse(line[2]), Int32.Parse(line[3])));
                    this.comboBox1.Items.Add(line[0]);
                }

            }
            file.Close();  
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
        {   string cliente = comboBox1.Text.ToString();
            string titolo = textBox1.Text.ToString();
            string tipoPLC = textBox2.Text.ToString();
            string tipoOP = textBox3.Text.ToString();
            string data = textBox4.Text.ToString();

            if (titolo.Equals(""))
                titolo = ".";
            if (tipoPLC.Equals(""))
                tipoPLC = ".";
            if (titolo.Equals(""))
                tipoOP = ".";
            if (data.Equals(""))
                data = ".";

            //leggere l'attuale file coi progetti di quel cliente
            int numCliente=0;
            int i = 0;
            foreach (Cliente c in clienti)
            {   
                if(c.getNomeCliente().Equals(cliente))
                {
                    numCliente = i;
                }
                i++;
            }
            string numProgetto = (clienti[numCliente].getMaxId()+1).ToString();
            clienti[numCliente].setMaxId(clienti[numCliente].getMaxId() + 1);
            Console.WriteLine("\n ultimo progetto= " + numProgetto );
            string file = Globals.DATI + cliente + ".csv";
            string projectDetails = numProgetto + Environment.NewLine + titolo + Environment.NewLine + tipoPLC + Environment.NewLine + tipoOP + Environment.NewLine + data + Environment.NewLine;
            File.AppendAllText(file, projectDetails);


            //creare nuova cartella in progetti e riempirla dei file iniziali
            Directory.CreateDirectory(Globals.PROGETTI + cliente + @"\" + cliente + numProgetto);
            string fileName = Globals.PROGETTI + cliente + @"\" + cliente + numProgetto + @"\progetto.docx";
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
            cambiaClientiCSV();

            this.Close();



        }
        
        private void cambiaClientiCSV ()
        {
            string[] lines = new string[2 + clienti.Count];
            lines[0] = "CLIENTE,SUFFISSO,LAST_ID,MAX_ID";
            lines[1] = lastOpen;
            int i = 2;
            foreach(Cliente c in clienti)
            {
                lines[i] = c.getNomeCliente() + "," + c.getSuffisso() + "," + c.getlastId() + "," + c.getMaxId();
                i++;
            }
            File.WriteAllLines(Globals.DATI + @"\CLIENTI.csv", lines);
            
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
