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
    /// Logica di interazione per ExpenseItViewExpense.xaml
    /// </summary>
    public partial class Clienti_Home : Page
    {
        private List<Button> buttonList;
        public Clienti_Home()
        {
            InitializeComponent();
            if (Globals.CLIENTI == null)
            {
                var file = File.OpenRead(Globals.DATI + @"\CLIENTI.csv");
                var reader = new StreamReader(file); string info = reader.ReadLine();
                Globals.LAST_CLIENT = reader.ReadLine();
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

            buttonList = new List<Button>();
            Grid grid = this.FindName("grid") as Grid;
            //int j = Globals.CLIENTI.Count;
            int i = 0;
            foreach (Cliente cliente in Globals.CLIENTI)
            {
                Button b = new Button();
                b.Width = 140;
                b.Height = 50;
                b.Name = "button" + i;
                b.Content = cliente.getNomeCliente();
                b.Click += new RoutedEventHandler(button_Click);
                b.Background = Brushes.White;
                b.Foreground = Brushes.Black;
                buttonList.Add(b);
                Grid.SetColumn(buttonList[i], i % 4 + 1);
                Grid.SetRow(buttonList[i], 1 + (i / 4));
                grid.Children.Add(buttonList[i]);
                Console.WriteLine("\t" + cliente.getNomeCliente());
                i++;
            }
            InitializeComponent();
            setVisibility();
        }

        private void updateClientList(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("UpdateClientList");
            Grid grid = this.FindName("grid") as Grid;
            
            int i = Globals.CLIENTI.Count-1;
            Cliente cliente = Globals.CLIENTI[i];
                Button b = new Button();
                b.Width = 140;
                b.Height = 50;
                b.Name = "button" + i;
                b.Content = cliente.getNomeCliente();
                b.Click += new RoutedEventHandler(button_Click);
                b.Background = Brushes.White;
                b.Foreground = Brushes.Black;
                buttonList.Add(b);
                Grid.SetColumn(buttonList[i], i % 4 + 1);
                Grid.SetRow(buttonList[i], 1 + (i / 4));
                grid.Children.Add(buttonList[i]);
                Console.WriteLine("\t" + cliente.getNomeCliente());
               

        }
        private void button_Click(object sender, EventArgs e)
        {
            // CAMBIARE PAGINA
            string clienteAttuale = ((Button)sender).Content.ToString();
            int n = Globals.CLIENTI.FindIndex(x => x.getNomeCliente().Equals(clienteAttuale));
            Globals.LAST_CLIENT = Globals.CLIENTI[n].getNomeCliente();
            // se la cartella o il file csv di quel clienteAttualee non esiste
            if (!Directory.Exists(Globals.PROGETTI + clienteAttuale) || !File.Exists(Globals.DATI + clienteAttuale + ".csv"))
            {
                MessageBoxResult mbr = MessageBox.Show("La cartella o il file csv del cliente attuale " + clienteAttuale + " non è presente.\nCrearli?", 
                    "File inesistenti", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(mbr == MessageBoxResult.Yes)
                {
                    if(!Directory.Exists(Globals.PROGETTI + clienteAttuale))
                    {
                        createClientDirectory(clienteAttuale);
                    }
                    if(!File.Exists(Globals.DATI + clienteAttuale + ".csv"))
                    {
                        createClientCSV(clienteAttuale);
                    }
                }
            }

            else
            {
                Progetti_Home progetti_home = new Progetti_Home(n);
                this.NavigationService.Navigate(progetti_home);
            }
        }

        private void createClientCSV(string cliente)
        {
            try
            {
                //creare nuovo file CSV in DATI
                File.Create(Globals.DATI+ cliente + ".csv");
                Console.WriteLine("Creato file: " + Globals.DATI + cliente + ".csv");
            }
            catch (IOException)
            {
                MessageBox.Show("E17 - Il file " + Globals.DATI + cliente + ".csv" + " non è stato creata per un problema");
            }
            try
            {
                //creare nuovo file CSV in DATI
                File.Create(Globals.DATI + cliente + "date.csv");
                Console.WriteLine("Creato file: " + Globals.DATI + cliente + "date.csv");
            }
            catch (IOException)
            {
                MessageBox.Show("E18 - Il file " + Globals.DATI + cliente + "date.csv" + " non è stato creata per un problema");
            }
        }

        private void createClientDirectory(string cliente)
        {
            try
            {
                //creare nuovo cartella in progetti
                Directory.CreateDirectory(Globals.PROGETTI + cliente);
                Console.WriteLine("Creata cartella: " + Globals.PROGETTI + cliente);
            }
            catch (IOException)
            {
                MessageBox.Show("E16 - La cartella " + Globals.PROGETTI + cliente + " non è stato creata per un problema");
            }
        }

        private void Menu_percorsi(object sender, RoutedEventArgs e)
        {
            Form_percorsi form = new Form_percorsi();
            form.ShowDialog();
        }
        private void Menu_github(object sender, RoutedEventArgs e)
        {
            Form_github form = new Form_github();
            form.ShowDialog();
        }
        private void Menu_anteprima(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;
            if (value != Globals.ANTEPRIME)
            {
                Globals.ANTEPRIME = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }
        }

        private void Menu_sync(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;

            if (value != Globals.SINCRONIZZAZIONE)
            {
                Globals.SINCRONIZZAZIONE = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }

        }

        private void setVisibility()
        {
            Console.WriteLine("Set visibility");
            MenuItem ma = this.FindName("Menu_anteprima_check") as MenuItem;
            MenuItem ms = this.FindName("Menu_sync_check") as MenuItem;
            ma.IsChecked = Globals.ANTEPRIME;
            ms.IsChecked = Globals.SINCRONIZZAZIONE;
        }

        private void Button_New_Client(object sender, RoutedEventArgs e)
        {
            
                Console.WriteLine("\nNew Client");
            Form_nuovoCliente form = new Form_nuovoCliente();
            //Cliente c = new Cliente("NUOVO", "NU", 1, 1);
            //Globals.CLIENTI.Add(c);
                form.FormClosed
                    += new System.Windows.Forms.FormClosedEventHandler(this.updateClientList);
                form.ShowDialog();
        }
    }
}


//string[] lines = System.IO.File.ReadAllLines(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI.txt");
//// Display the file contents by using a foreach loop.
//System.Console.WriteLine("Contents of WriteLines2.txt = ");
//            Button[] buttonArray = new Button[lines.Length];
//int i = 0;
//Grid grid = this.FindName("grid") as Grid;

//            foreach (string line in lines)
//            {
//                // Use a tab to indent each line of the file.
                
//                buttonArray[i] = new Button();
//buttonArray[i].Width = 125;
//                buttonArray[i].Height = 30;
//                buttonArray[i].Name = "button" + i;
//                buttonArray[i].Content = line;
//                buttonArray[i].Click += new RoutedEventHandler(button_Click);
//Grid.SetColumn(buttonArray[i], i%4);
//                Grid.SetRow(buttonArray[i], 1+ (i/4));
//                grid.Children.Add(buttonArray[i]);
//                Console.WriteLine("\t" + line);
//                i++;
//            }