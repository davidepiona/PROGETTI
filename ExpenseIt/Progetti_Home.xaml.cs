using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpenseIt
{
    /// <summary>
    /// Logica di interazione per Progetti_Home.xaml
    /// </summary>
    public partial class Progetti_Home : Page
    {
        private Cliente cliente;
        private string lastProject;
        private List<Progetto> progetti = new List<Progetto>();
        private List<Progetto> progettiSync = new List<Progetto>();
        private List<Cliente> clienti = new List<Cliente>();
        private UltimaModifica ultimaModifica;
        private string lastClient;
        public Progetti_Home(Cliente cliente)
        {
            InitializeComponent();
            this.cliente = cliente;
            initialize();

        }

        public Progetti_Home()
        {
            InitializeComponent();
            var file = File.OpenRead(Globals.DATI + @"\CLIENTI.csv");
            var reader = new StreamReader(file);
            reader.ReadLine();
            lastClient = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                if (line.Length == 4)
                {
                    clienti.Add(new Cliente(line[0], line[1], Int32.Parse(line[2]), Int32.Parse(line[3])));
                }

            }
            file.Close();
            cliente = clienti[clienti.FindIndex(x => x.getNomeCliente().Equals(lastClient))];
            initialize();
            
        }

        private void initialize()
        {
            readProjects();
            ultimaModifica = new UltimaModifica(cliente);
            ultimaModifica.readByCSV(Globals.DATI + cliente.getNomeCliente() + "date.csv");
            ultimaModifica.aggiornoModifiche(progetti);
            updateList("");
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + " " + cliente.getNomeCliente();
        }

        private void readProjects()
        {
            Console.WriteLine("\nCreate List");
            List<string> lines = new List<string>();
            int i = 0;
            int j = 0;
            using (var reader = new CsvFileReader(Globals.DATI + cliente.getNomeCliente() + ".csv"))
            {
                i = 0;
                while (reader.ReadRow(lines))
                {
                    int num = Int32.Parse(lines[0]);
                    //string numero = string.Format("{0:D3}", num);
                    reader.ReadRow(lines);
                    string nome = lines[0];
                    reader.ReadRow(lines);
                    string tipoPLC = lines[0];
                    reader.ReadRow(lines);
                    string tipoOP = lines[0];
                    reader.ReadRow(lines);
                    string data = lines[0];
                    Console.WriteLine(num);
                    progetti.Add(new Progetto(cliente.getNomeCliente() + num, nome, tipoOP, tipoOP, data));
                    i++;
                }
            }
        }


        private void createList()
        {
            //string path = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + cliente.getNomeCliente();
            //string[] fileEntries = Directory.GetFileSystemEntries(path);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;

            dataGrid.SelectionChanged += new SelectionChangedEventHandler(changePreview);
            //dataGrid.SelectionChanged += new SelectionChangedEventHandler(changePreview);
            //foreach (string fileEntry in fileEntries)
            int i = 0;
            foreach (Progetto p in progetti)
            {   
                dataGrid.Items.Add(p);
                if (p.numero.Equals(cliente.getNomeCliente() + cliente.getlastId()))
                {
                    dataGrid.SelectedIndex = i;
                    dataGrid.ScrollIntoView(progetti[i]);
                    
                }
                i++;
            }

            //dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[10].Index;
            //
            
        }

        private void updateList(string filter)
        {
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            //Console.WriteLine("filter: <" + filter + ">");
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                foreach (Progetto p in progetti)
                {
                    if (p.ToName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        dataGrid.Items.Add(p);
                    }
                }
            }
        }

        private void updateList(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {

            readProjects();
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            //Console.WriteLine("filter: <" + filter + ">");
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                foreach (Progetto p in progetti)
                {
                        dataGrid.Items.Add(p);
                }
            }
        }

        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\nOpen Folder");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            string lastProject = dataGrid.SelectedValue.ToString();
            string path = Globals.PROGETTI + cliente.getNomeCliente() + @"\" + lastProject;
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void New_Project(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\nNew Project");
            Form1 testDialog = new Form1(cliente);
            
            testDialog.FormClosed
                += new System.Windows.Forms.FormClosedEventHandler(this.updateList);
            testDialog.ShowDialog();
            //Console.WriteLine("\nNew Project");
            //FormNuovoCliente testDialog = new FormNuovoCliente();
            //testDialog.ShowDialog();
        }

        private void changePreview(object sender, EventArgs e)
        {
            Console.WriteLine("\nChange Preview");
            try
            {
                lastProject = ((DataGrid)sender).SelectedValue.ToString();

                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                Image image = this.FindName("image") as Image;
                try
                {
                    var doc = Xceed.Words.NET.DocX.Load(Globals.PROGETTI + cliente.getNomeCliente() + @"\" + lastProject + @"\progetto.docx");

                    richTextBox.Document.Blocks.Clear();
                    richTextBox.AppendText(doc.Text);

                }
                catch (FileNotFoundException)
                {
                    richTextBox.Document.Blocks.Clear();
                }
                catch (IOException)
                {
                    //the file is unavailable because it is: still being written to or being processed by another thread or does not exist (has already been processed)
                    // Insert a paragrpah:
                    //doc.InsertParagraph("This is my first paragraph");

                    // Save to the output directory:
                    //doc.Save();

                    //Console.WriteLine(doc.Text);
                }

                try
                {
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.UriSource = new Uri(Globals.PROGETTI + cliente.getNomeCliente() + @"\" + lastProject + @"\anteprima.jpg", UriKind.Absolute);
                    bmi.EndInit();
                    image.Source = bmi;
                }
                catch (FileNotFoundException)
                {
                    image.Source = null;
                }
                catch (IOException)
                {
                    //the picture is unavailable because it is: still being open to or being processed by another thread or does not exist (has already been processed)
                }
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("ECCEZIONE: " + nre);
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateList(((TextBox)sender).Text);
        }

        private void Apri_Docx(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + cliente.getNomeCliente() + @"\" + lastProject + @"\progetto.docx");
        }

        private void Apri_Immagine(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + cliente.getNomeCliente() + @"\" + lastProject + @"\anteprima.jpg");
        }


        private void Ultime_Modifiche(object sender, RoutedEventArgs e)
        {
            //UltimaModifica u = new UltimaModifica(cliente);
            //Console.WriteLine(u.modificheByFile(PROGETTI + cliente.getNomeCliente() + @"\PATA190"));
            ultimaModifica.ricercaLenta(Globals.PROGETTI + cliente.getNomeCliente() + @"\");
            ultimaModifica.writeInCSV(Globals.DATI + cliente.getNomeCliente() + "date.csv");
            ultimaModifica.aggiornoModifiche(progetti);
            updateList("");
        }

        
        private void Sync(object sender, RoutedEventArgs e)
        {
            ultimaModifica.readSync(Globals.DATIsync + cliente.getNomeCliente() + "date.csv");
            ultimaModifica.confrontoSync(progetti);
            updateList("");

            

        }

        private void Altro(object sender, RoutedEventArgs e)
        {

        }
    }
}
