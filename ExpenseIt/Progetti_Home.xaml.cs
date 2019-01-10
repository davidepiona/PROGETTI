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
        private int num_cliente;
        private List<Progetto> progetti = new List<Progetto>();
        private List<Progetto> progettiSync = new List<Progetto>();
        private UltimaModifica ultimaModifica;
        public Progetti_Home(int num_cliente)
        {
            InitializeComponent();
            this.num_cliente = num_cliente;

            initialize();

        }

        public Progetti_Home()
        {

            InitializeComponent();
            Loaded += Progetti_Home_Loaded;
            if (Globals.CLIENTI == null)
            {
                var file = File.OpenRead(Globals.DATI + @"\CLIENTI.csv");
                var reader = new StreamReader(file);
                reader.ReadLine();
                Globals.LAST_CLIENT = reader.ReadLine();
                Globals.CLIENTI = new List<Cliente>();
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');
                    if (line.Length == 4)
                    {
                        Globals.CLIENTI.Add(new Cliente(line[0], line[1], Int32.Parse(line[2]), Int32.Parse(line[3])));
                    }

                }
                file.Close();
            }

            num_cliente = Globals.CLIENTI.FindIndex(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT));
            initialize();

        }

        private void initialize()
        {
            readProjects();
            ultimaModifica = new UltimaModifica(Globals.CLIENTI[num_cliente]);
            ultimaModifica.readByCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv");
            ultimaModifica.aggiornoModifiche(progetti);
            updateList("");
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + " " + Globals.CLIENTI[num_cliente].getNomeCliente();

        }
        private void Progetti_Home_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }
        private void readProjects()
        {
            Console.WriteLine("Read Projects");
            List<string> lines = new List<string>();
            int i = 0;
            int j = 0;
            using (var reader = new CsvFileReader(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + ".csv"))
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
                    progetti.Add(new Progetto(num, nome, tipoOP, tipoOP, data, Globals.CLIENTI[num_cliente].getNomeCliente()));
                    i++;
                }
            }
        }


        private void createList()
        {
            Console.WriteLine("Create List");
            //string path = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + cliente.getNomeCliente();
            //string[] fileEntries = Directory.GetFileSystemEntries(path);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;

            dataGrid.SelectionChanged += new SelectionChangedEventHandler(changePreview);
            dataGrid.Items.Clear();
            //dataGrid.SelectionChanged += new SelectionChangedEventHandler(changePreview);
            //foreach (string fileEntry in fileEntries)
            int i = 0;
            foreach (Progetto p in progetti)
            {
                dataGrid.Items.Add(p);
                if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId()))
                {
                    dataGrid.SelectedIndex = i;
                    dataGrid.ScrollIntoView(progetti[i + 5]);

                }
                i++;
            }

            //dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[10].Index;
            //

        }

        private void updateList(string filter)
        {
            Console.WriteLine("Update list1");
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
            Console.WriteLine("UpdateList2");
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

            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            string path = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void New_Project(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\nNew Project");
            Form1 testDialog = new Form1(Globals.CLIENTI[num_cliente]);

            testDialog.FormClosed
                += new System.Windows.Forms.FormClosedEventHandler(this.updateList);
            testDialog.ShowDialog();
            //Console.WriteLine("\nNew Project");
            //FormNuovoCliente testDialog = new FormNuovoCliente();
            //testDialog.ShowDialog();
        }

        private void changePreview(object sender, EventArgs e)
        {
            Console.WriteLine("Change Preview");
            try
            {
                Globals.CLIENTI[num_cliente].setLastId(((Progetto)((DataGrid)sender).SelectedValue).getNumProject());

                //Globals.CLIENTI[num_cliente].setLastId(12);
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                richTextBox.Visibility = Visibility.Visible;
                Button button = this.FindName("buttonOpenDocx") as Button;
                button.Visibility = Visibility.Visible;
                Image image = this.FindName("image") as Image;

                //Console.WriteLine("PATHHH:  " + Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                //@"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId() + @"\progetto.docx");
                string file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId() + @"\progetto.docx";
                if (File.Exists(file))
                {
                    var doc = Xceed.Words.NET.DocX.Load(file);
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.AppendText(doc.Text);
                }
                else
                {
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.Visibility = Visibility.Hidden;
                    button.Visibility = Visibility.Hidden;
                }

                file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                   @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId() + @"\anteprima.jpg";
                if (File.Exists(file))
                {
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.CacheOption = BitmapCacheOption.OnLoad;
                    bmi.UriSource = new Uri(file, UriKind.Absolute);
                    bmi.DecodePixelWidth = 320;
                    bmi.EndInit();
                    bmi.Freeze(); // important
                    image.Source = bmi;
                }
                else
                {
                    image.Source = null;
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
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId() + @"\progetto.docx");
        }

        private void Apri_Immagine(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId() + @"\anteprima.jpg");
        }


        private void Ultime_Modifiche(object sender, RoutedEventArgs e)
        {
            //UltimaModifica u = new UltimaModifica(cliente);
            //Console.WriteLine(u.modificheByFile(PROGETTI + cliente.getNomeCliente() + @"\PATA190"));
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            Button buttonSync = this.FindName("BottSync") as Button;
            buttonModifiche.IsEnabled = false;
            buttonSync.IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                ultimaModifica.ricercaLenta(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\");
                ultimaModifica.writeInCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv");
                ultimaModifica.aggiornoModifiche(progetti);


            }).ContinueWith(task =>
            {
                updateList("");
                buttonModifiche.IsEnabled = true;
                buttonSync.IsEnabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());


        }


        private void Sync(object sender, RoutedEventArgs e)
        {
            ultimaModifica.readSync(Globals.DATIsync + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv");
            ultimaModifica.confrontoSync(progetti);
            updateList("");



        }

        private void Altro(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        private void Cambia_Pagina(object sender, RoutedEventArgs e)
        {
            Clienti_Home clienti_home = new Clienti_Home();
            this.NavigationService.Navigate(clienti_home);
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Open_Folder(sender, null);
        }
    }
}
