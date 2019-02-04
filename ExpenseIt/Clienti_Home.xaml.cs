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
    /// Logica di interazione per Clienti_Home.xaml
    /// ORGANIZZAZIONE:
    /// - COSTRUTTORE
    /// - FUNZIONI PRINCIPALI
    /// - CONTROLLI GENERALI
    /// - BOTTONI
    /// - INTERAZIONE CON L'UTENTE
    /// - MENU DI IMPOSTAZIONI
    /// </summary>
    public partial class Clienti_Home : Page
    {
        private List<Button> buttonList;
        private List<Progetto> progettiAll;
        private int numClientiTemp;
        public Clienti_Home()
        {
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            Loaded += Clienti_Home_Loaded;
            if (Globals.CLIENTI == null)
            {
                Globals.log.Info("Lettura CLIENTI.csv");
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
            Grid grid = this.FindName("buttonGrid") as Grid;
            int i = 0;
            foreach (Cliente cliente in Globals.CLIENTI)
            {
                if (i % 4 == 0)
                {
                    RowDefinition r = new RowDefinition
                    {
                        Height = new GridLength(140, GridUnitType.Star),
                        MinHeight = 140
                    };
                    grid.RowDefinitions.Add(new RowDefinition());
                }
                Button b = new Button
                {
                    Width = 165,
                    Height = 50,
                    Name = "button" + i,
                    Margin = new Thickness(25),
                    Content = cliente.getNomeCliente().Replace("_", "__")
                };
                b.Click += new RoutedEventHandler(Button_Click);
                b.Background = Brushes.White;
                b.Foreground = Brushes.Black;
                buttonList.Add(b);
                Grid.SetColumn(buttonList[i], i % 4);
                Grid.SetRow(buttonList[i], (i / 4));
                grid.Children.Add(buttonList[i]);
                i++;
            }
            InitializeComponent();
            SetVisibility();
        }
       
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                            FUNZIONI PRINCIPALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Se è stato aggiunto un nuovo cliente aggiunge il bottone corrispondende nella posizione corretta.
        /// Ogni 4 bottoni aggiunge una riga. 
        /// </summary>
        private void UpdateClientList(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (numClientiTemp >= Globals.CLIENTI.Count)
            {
                return;
            }
            Console.WriteLine("UpdateClientList");
            Globals.log.Info("UpdateClientList");
            Grid grid = this.FindName("buttonGrid") as Grid;

            int i = Globals.CLIENTI.Count - 1;
            if (i % 4 == 0)
            {
                RowDefinition r = new RowDefinition
                {
                    Height = new GridLength(140, GridUnitType.Star),
                    MinHeight = 140
                };
                grid.RowDefinitions.Add(new RowDefinition());
            }
            Cliente cliente = Globals.CLIENTI[i];
            Button b = new Button
            {
                Width = 165,
                Height = 50,
                Margin = new Thickness(25),
                Name = "button" + i,
                Content = cliente.getNomeCliente().Replace("_", "__")
            };
            b.Click += new RoutedEventHandler(Button_Click);
            b.Background = Brushes.White;
            b.Foreground = Brushes.Black;
            buttonList.Add(b);
            Grid.SetColumn(buttonList[i], i % 4);
            Grid.SetRow(buttonList[i], (i / 4));
            grid.Children.Add(buttonList[i]);
            Console.WriteLine("Nuovo cliente: " + cliente.getNomeCliente());
            Globals.log.Info("Nuovo cliente: " + cliente.getNomeCliente());
        }

        /// <summary>
        /// Legge i progetti da TUTTI i file .csv e li salva nella lista progettiAll.
        /// </summary>
        private void ReadProjects()
        {
            Globals.log.Info("Read all projects");
            progettiAll = new List<Progetto>();
            for (int i = 0; i < Globals.CLIENTI.Count; i++)
            {
                int j = 0;
                List<string> lines = new List<string>();
                try
                {   
                    Console.WriteLine("Leggo:" + Globals.CLIENTI[i].getNomeCliente());
                    using (var reader = new CsvFileReader(Globals.DATI + Globals.CLIENTI[i].getNomeCliente() + ".csv"))
                    {
                        while (reader.ReadRow(lines) && lines.Count != 0 && lines != null)
                        {
                            
                            int num = Int32.Parse(lines[0]);
                            reader.ReadRow(lines);
                            string nome = lines[0];
                            reader.ReadRow(lines);
                            string tipoPLC = lines[0];
                            reader.ReadRow(lines);
                            string tipoOP = lines[0];
                            reader.ReadRow(lines);
                            string data = lines[0];
                            progettiAll.Add(new Progetto(num, nome, tipoOP, tipoOP, data, Globals.CLIENTI[i].getNomeCliente(), Globals.CLIENTI[i].getSuffisso()));
                            j++;
                        }
                    }
                }
                catch (IOException)
                {
                    string msg = "E17 - Il file " + Globals.DATI + Globals.CLIENTI[i].getNomeCliente() + ".csv" + " non esiste o è aperto da un altro programma";
                    //MessageBox.Show(msg);
                    Console.WriteLine(msg);
                    Globals.log.Error(msg);
                }
                catch (FormatException)
                {
                    string msg = "E30 - Il file " + Globals.DATI + Globals.CLIENTI[i].getNomeCliente() + ".csv" + " è in un formato non corretto" +
                        "\nProblema riscontrato al progetto numero: " + j;
                    Console.WriteLine(msg);
                    Globals.log.Error(msg);
                }
            }
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid:
        /// - aggiunge tutti i progetti presenti e FILTRATI dopo aver svuotato la DataGrid
        /// - mentre scorre i progetti restituisce il primo visualizzato per permettere di selezionarlo durante la ricerca
        /// </summary>
        private Progetto UpdateList(string filter)
        {
            Progetto primo = new Progetto(0, null, null, null, null, null, null);
            if (progettiAll != null)
            {
                Console.WriteLine("Update list1");
                Globals.log.Info("Update list1");
                if (this.FindName("dataGrid") is DataGrid dataGrid)
                {
                    dataGrid.Items.Clear();
                    int i = 0;
                    foreach (Progetto p in progettiAll)
                    {
                        if (p.ToName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            if (i == 0)
                            {
                                primo = p;
                            }
                            dataGrid.Items.Add(p);
                            i++;
                        }
                    }
                }
            }
            return primo;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             CONTROLLI GENERALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dopo aver caricato la pagina da il focus alla textbox.
        /// </summary>
        public void Clienti_Home_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        /// <summary>
        /// Funzione per la creazione dei necessari file .csv in DATI.
        /// - creazione del file *CLIENTE*.csv
        /// - creazione del file *CLIENTE*date.csv
        /// </summary>
        private void CreateClientCSV(string cliente)
        {
            try
            {
                File.Create(Globals.DATI + cliente + ".csv");
                string msg = "Creato file: " + Globals.DATI + cliente + ".csv";
                Console.WriteLine(msg);
                Globals.log.Info(msg);
            }
            catch (IOException)
            {
                string msg = "E21 - Il file " + Globals.DATI + cliente + ".csv" + " non è stato creata per un problema";
                MessageBox.Show(msg, "E21"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            try
            {
                File.Create(Globals.DATI + cliente + "date.csv");
                string msg = "Creato file: " + Globals.DATI + cliente + "date.csv";
                Console.WriteLine(msg);
                Globals.log.Info(msg);
            }
            catch (IOException)
            {
                string msg = "E22 - Il file " + Globals.DATI + cliente + "date.csv" + " non è stato creato per un problema";
                MessageBox.Show(msg, "E22"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Funzione per la creazione di una nuova cartella in progetti.
        /// </summary>
        private void CreateClientDirectory(string cliente)
        {
            try
            {
                Directory.CreateDirectory(Globals.PROGETTI + cliente);
                string msg = "Creata cartella: " + Globals.PROGETTI + cliente;
                Console.WriteLine(msg);
                Globals.log.Info(msg);
            }
            catch (IOException)
            {
                string msg = "E23 - La cartella " + Globals.PROGETTI + cliente + " non è stata creata per un problema";
                MessageBox.Show(msg, "E23"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                        INTERAZIONE CON L'UTENTE                            ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Open_Folder(null, null);
        }

        /// <summary>
        /// Richiamato ogni volta che viene modificato il testo nella TextBox di ricerca.
        /// Alla prima chiamata rende il bottone visibile e carica per la prima volta i file.
        /// Aggiorna e filtra la lista e poi seleziona la prima riga visualizzata.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            Button button = this.FindName("BottApri2") as Button;
            if (progettiAll == null)
            {
                ReadProjects();
                button.Visibility = Visibility.Visible;
                dataGrid.Visibility = Visibility.Visible;
            }
            Progetto ultimo = UpdateList(((TextBox)sender).Text);
            dataGrid.SelectedIndex = 0;
            dataGrid.ScrollIntoView(ultimo);
        }

        /// <summary>
        /// Funzioni per consentire di navigare la DataGrid con le freccie su e giù mentre si effettua una ricerca.
        /// Con invio si apre la cartella del filesystem del progetto selezionato
        /// </summary>
        private void PreviewKeyDown2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                Console.WriteLine("su");
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                dataGrid.Focus();
                if (dataGrid.SelectedIndex > 0)
                {
                    dataGrid.SelectedIndex = dataGrid.SelectedIndex - 1;
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            if (e.Key == Key.Down)
            {
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                Console.WriteLine("giu");
                dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            if (e.Key == Key.Enter)
            {
                Console.WriteLine("invio");
                Open_Folder(null, null);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                                BOTTONI                                     ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bottone apertura cartella del filesystem.
        /// Imposta questo come ultimo progetto aperto.
        /// </summary>
        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            Progetto p = dataGrid.SelectedItem as Progetto;
            string path = Globals.PROGETTI + p.nomeCliente + @"\" + p.nomeCliente + p.numero;
            int n = Globals.CLIENTI.FindIndex(x => x.getNomeCliente().Equals(p.nomeCliente));
            Globals.CLIENTI[n].setLastId(p.numero);
            Globals.LAST_CLIENT = p.nomeCliente;
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        /// <summary>
        /// Funzione che si attiva quando un bottone-cliente viene premuto.
        /// Cerca di aprire la Progetti_Home del cliente selezionato.
        /// - sistema il problema degli underscore usati come RecognizesAccessKey
        /// - cerca il cliente con nome come quello selezionato e lo imposta come ultimo aperto
        /// - nel caso la cartella di progetto o il file .csv non esistano chiede se si intende crearli
        /// </summary>
        private void Button_Click(object sender, EventArgs e)
        {
            string clienteAttuale = ((Button)sender).Content.ToString().Replace("__", "_");
            int n = Globals.CLIENTI.FindIndex(x => x.getNomeCliente().Equals(clienteAttuale));
            Globals.LAST_CLIENT = Globals.CLIENTI[n].getNomeCliente();
            if (!Directory.Exists(Globals.PROGETTI + clienteAttuale) || !File.Exists(Globals.DATI + clienteAttuale + ".csv"))
            {
                MessageBoxResult mbr = MessageBox.Show("La cartella o il file csv del cliente attuale " + clienteAttuale + " non è presente.\nCrearli?",
                    "File inesistenti", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                if (mbr == MessageBoxResult.Yes)
                {
                    if (!Directory.Exists(Globals.PROGETTI + clienteAttuale))
                    {
                        CreateClientDirectory(clienteAttuale);
                    }
                    if (!File.Exists(Globals.DATI + clienteAttuale + ".csv"))
                    {
                        CreateClientCSV(clienteAttuale);
                    }
                }
            }
            else
            {
                Progetti_Home progetti_home = new Progetti_Home(n);
                this.NavigationService.Navigate(progetti_home);
            }
        }

        /// <summary>
        /// Bottone per la creazione di un nuovo cliente.
        /// Chiama il FORM Form_nuovoCliente. 
        /// </summary>
        private void Button_New_Client(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\nNew Client");
            Globals.log.Info("\nNew Client");
            Form_nuovoCliente form = new Form_nuovoCliente();
            numClientiTemp = Globals.CLIENTI.Count;
            form.FormClosed
                += new System.Windows.Forms.FormClosedEventHandler(this.UpdateClientList);
            form.ShowDialog();
        }

        /// <summary>
        /// Bottone che effettua il clone del repository online.
        /// Poi aggiorna la DataGrid. 
        /// [Disabilita i bottoni di sincronizzazione] 
        /// </summary>
        private void Button_GitHubCloneAll(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler SCARICARE TUTTI i progetti dal repository online e salvarli sul tuo computer in DATIsync?",
                "Effettuare clone?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                GitCommands git = new GitCommands();
                if (git.clone())
                {
                    string msg = "Clone del repository " + Globals.GITURL + " nella cartella " + Globals.DATIsync + " riuscito correttamente!";
                    MessageBox.Show(msg, "Clone riuscito"
                                     , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Info(msg);
                }
                else
                {
                    string msg = "Clone del repository " + Globals.GITURL + " nella cartella " + Globals.DATIsync + " NON riuscito.";
                    MessageBox.Show(msg, "Clone non riuscito"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
                UpdateList("");
            }
        }

        /// <summary>
        /// Bottone che effettua il push dei file del cliente in DATI, mettendoli sul repository online.
        /// - chiede conferma
        /// - sostituisce nella cartella DATIsync
        /// - effettua il push e da all'utente informazioni sull'esito
        /// - ricarica la DataGrid
        /// [Disabilita i bottoni di sincronizzazione] 
        /// </summary>
        private void Button_GitHubPushAll(object sender, RoutedEventArgs e)
        {
            Button buttonClone = this.FindName("BottGitClone2") as Button;
            Button buttonPush = this.FindName("BottGitPush2") as Button;
            buttonClone.IsEnabled = false;
            buttonPush.IsEnabled = false;
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler CARICARE TUTTI i progetti attuali online?",
                "Caricare online?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                GitCommands git = new GitCommands();
                if (git.copyFolder())
                {
                    List<string> pushInfo = git.push("");
                    if (pushInfo != null)
                    {

                        if (pushInfo.Count != 0)
                        {
                            string message = "";
                            foreach (string f in pushInfo)
                            {
                                message += "\n" + f;
                            }
                            string msg = "Upload sul repository " + Globals.GITURL + " riuscito correttamente! File caricati:\n" + message;
                            MessageBox.Show(msg, "Upload riuscito"
                                         , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            Globals.log.Info(msg);
                        }
                        else
                        {
                            string msg = "Upload sul repository " + Globals.GITURL + " non effettuato perchè non c'era niente da caricare";
                            MessageBox.Show(msg, "Niente da caricare"
                                        , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                            Globals.log.Info(msg);
                        }
                    }
                    else
                    {
                        string msg = "Upload sul repository " + Globals.GITURL + " non riuscito.";
                        MessageBox.Show(msg, "Errore upload"
                                            , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                }
            }
            //else if (dialogResult == MessageBoxResult.No) { }
            buttonClone.IsEnabled = true;
            buttonPush.IsEnabled = true;
            UpdateList("");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                          MENU DI IMPOSTAZIONI                              ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Apre il FORM Form_percorsi per modificare i path in cui il programma cerca i file.
        /// </summary>
        private void Menu_percorsi(object sender, RoutedEventArgs e)
        {
            Form_percorsi form = new Form_percorsi();
            form.ShowDialog();
            this.InvalidateVisual();
        }

        /// <summary>
        /// Apre il FORM Form_github per modificare il path di git.exe e del repository github.
        /// </summary>
        private void Menu_github(object sender, RoutedEventArgs e)
        {
            Form_github form = new Form_github();
            form.ShowDialog();
        }

        /// <summary>
        /// Imposta la visibilità dell'anteprima dell'immagine e del docx.
        /// Aggiorna la variabile Globals.ANTEPRIME e scrive sul .csv.
        /// </summary>
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

        /// <summary>
        /// Aggiorna la variabile Globals.SINCRONIZZAZIONE e scrive sul .csv.
        /// </summary>
        private void Menu_sync(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;
            if (value != Globals.SINCRONIZZAZIONE)
            {
                Globals.SINCRONIZZAZIONE = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }
            Button buttonClone = this.FindName("BottGitClone2") as Button;
            Button buttonPush = this.FindName("BottGitPush2") as Button;
            if (value)
            {
                buttonClone.Visibility = Visibility.Visible;
                buttonPush.Visibility = Visibility.Visible;
            }
            else
            {
                buttonClone.Visibility = Visibility.Hidden;
                buttonPush.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Imposta le checkbox delle voci di ANTEPRIME e SINCRONIZZAZIONE nel menù, coerenti con il loro valore in Globals.
        /// </summary>
        private void SetVisibility()
        {
            Console.WriteLine("Set visibility");
            Globals.log.Info("Set visibility");
            MenuItem ma = this.FindName("Menu_anteprima_check") as MenuItem;
            MenuItem ms = this.FindName("Menu_sync_check") as MenuItem;
            ma.IsChecked = Globals.ANTEPRIME;
            ms.IsChecked = Globals.SINCRONIZZAZIONE;
            if (!Globals.SINCRONIZZAZIONE)
            {
                Button buttonClone = this.FindName("BottGitClone2") as Button;
                Button buttonPush = this.FindName("BottGitPush2") as Button;
                buttonClone.Visibility = Visibility.Hidden;
                buttonPush.Visibility = Visibility.Hidden;
            }
        }

        private void Menu_importa_CSV(object sender, RoutedEventArgs e)
        {
            Form_aggiornaCSV form = new Form_aggiornaCSV();
            form.ShowDialog();
        }

    }
}


