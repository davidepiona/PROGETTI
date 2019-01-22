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
    /// ORGANIZZAZIONE:
    /// - COSTRUTTORI
    /// - FUNZIONI PRINCIPALI
    /// - CONTROLLI GENERALI
    /// - BOTTONI
    /// - INTERAZIONE CON L'UTENTE
    /// - MENU DI IMPOSTAZIONI
    /// </summary>
    public partial class Progetti_Home : Page
    {
        private bool back = false;
        private int num_cliente;
        private int ProgSelezionato;
        private List<Progetto> progetti = new List<Progetto>();
        private List<Progetto> progettiSync = new List<Progetto>();
        private UltimaModifica ultimaModifica;

        /// <summary>
        /// Costruttore 1, per quando si avvia partendo da questa pagina.
        /// </summary>
        public Progetti_Home()
        {
            InitializeComponent();
            Loaded += Progetti_Home_Loaded;
            try
            {
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
            }
            catch (IOException)
            {
                MessageBox.Show("E01 - Il file " + Globals.DATI + @"\CLIENTI.csv" +
                    " non esiste o è aperto da un altro programma. \n L'APPLICAZIONE SARA' CHIUSA", "E01"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Environment.Exit(0);
            }

            num_cliente = Globals.CLIENTI.FindIndex(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT));
            string cliente = Globals.CLIENTI.Find(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT)).getNomeCliente();
            if (CheckFolderandCsv(Globals.CLIENTI[num_cliente].getNomeCliente()))
            {
                initialize();
            }

        }

        /// <summary>
        /// Costruttore 2, per quando viene chiamato dall'altra pagina.
        /// </summary>
        public Progetti_Home(int num_cliente)
        {
            InitializeComponent();
            this.num_cliente = num_cliente;

            initialize();

        }

        /// <summary>
        /// Procedura inziale: 
        /// - legge i progetti da file
        /// - legge le ultime modifiche da file (poi mette i risultati in progetti)
        /// - controlla la sincronizzazione
        /// - crea la lista
        /// - initcheck: guarda se coincidono il numero di progetti e l'ultimo progetto
        /// - imposta la visibilità degli elementi
        /// </summary>
        private void initialize()
        {
            readProjects();
            ProgSelezionato = Globals.CLIENTI[num_cliente].getlastId();
            ultimaModifica = new UltimaModifica(Globals.CLIENTI[num_cliente]);
            if (!ultimaModifica.readByCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"))
            {
                MessageBox.Show("E02 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    "date.csv" + " non esiste o è aperto da un altro programma.\n\nLe ultime modifiche dei progetti non " +
                    "saranno caricate da file.", "E02"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
            ultimaModifica.aggiornoModifiche(progetti);
            //updateList("");
            CheckBox_sync_ultima_modifica();
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + " " + Globals.CLIENTI[num_cliente].getNomeCliente().Replace("_", "__");
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            InitCheck();
            SetVisibility();

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                            FUNZIONI PRINCIPALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Legge i progetti da file .csv e li salva nella lista progetti.
        /// </summary>
        private void readProjects()
        {
            Console.WriteLine("Read Projects");
            List<string> lines = new List<string>();
            try
            {
                using (var reader = new CsvFileReader(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + ".csv"))
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
                        progetti.Add(new Progetto(num, nome, tipoOP, tipoOP, data, Globals.CLIENTI[num_cliente].getNomeCliente(), Globals.CLIENTI[num_cliente].getSuffisso()));
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("E03 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    ".csv" + " non esiste o è aperto da un altro programma", "E03"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
        }

        /// <summary>
        /// Aggiunge la lista di progetti alla DataGrid dopo averla svuotata.
        /// Mentre li scorre cerca quello che era stato aperto per ultimo e quando lo trova lo seleziona.
        /// </summary>
        private void createList()
        {
            Console.WriteLine("Create List");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(ChangePreview);
            dataGrid.Items.Clear();
            int i = 0;
            foreach (Progetto p in progetti)
            {
                dataGrid.Items.Add(p);
                if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId()))
                {
                    dataGrid.SelectedIndex = i;
                    dataGrid.ScrollIntoView(progetti[i]);
                }
                i++;
            }
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid:
        /// - controlla la sincronizzazione
        /// - aggiorna le ultime modifiche in progetti
        /// - aggiunge tutti i progetti presenti e FILTRATI dopo aver svuotato la DataGrid
        /// - mentre scorre i progetti restituisce il primo visualizzato per permettere di selezionarlo durante la ricerca
        /// </summary>
        private Progetto updateList(string filter)
        {
            CheckBox_sync_ultima_modifica();
            Console.WriteLine("Update list1");
            Progetto primo = new Progetto(0, null, null, null, null, null, null);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            ultimaModifica.aggiornoModifiche(progetti);
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                int i = 0;
                foreach (Progetto p in progetti)
                {
                    if (p.ToName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        if (i == 0)
                        {
                            primo = p;
                        }
                        dataGrid.Items.Add(p);
                        if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId())) //forse inutile questo if
                        {
                            dataGrid.SelectedIndex = i;
                            dataGrid.ScrollIntoView(progetti[i]);
                        }
                        i++;
                    }
                }
            }
            return primo;
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid DOPO AVER CREATO NUOVI PROGETTI:
        /// - LEGGE I PROGETTI DA FILE (unica cosa in più del precedente)
        /// - controlla la sincronizzazione
        /// - aggiorna le ultime modifiche in progetti
        /// - aggiunge tutti i progetti presenti dopo aver svuotato la DataGrid
        /// - mentre scorre i progetti cerca quello che era stato aperto per ultimo e quando lo trova lo seleziona.
        /// </summary>
        private void updateListNewProject(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("UpdateList2");
            progetti = new List<Progetto>();
            readProjects();
            CheckBox_sync_ultima_modifica();
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            ultimaModifica.aggiornoModifiche(progetti);
            if (dataGrid != null)
            {
                int i = 0;
                dataGrid.Items.Clear();
                foreach (Progetto p in progetti)
                {
                    dataGrid.Items.Add(p);
                    if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId()))
                    {
                        dataGrid.SelectedIndex = i;
                        dataGrid.ScrollIntoView(progetti[i]);
                    }
                    i++;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             CONTROLLI GENERALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Controlla che ci siano cartella e file .csv 
        /// </summary>
        public bool CheckFolderandCsv(string cliente)
        {
            if (!System.IO.Directory.Exists(Globals.PROGETTI + cliente) || !System.IO.File.Exists(Globals.DATI + cliente + ".csv"))
            {
                MessageBox.Show("La cartella o il file csv del cliente attuale " + cliente + " non è presente", "Informazioni assenti"
                                     , MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                back = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Dopo aver caricato la pagina da il focus alla textbox e controlla se è necessario tornare a Clienti_Home
        /// </summary>
        public void Progetti_Home_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
            if (back)
            {
                Clienti_Home clienti_home = new Clienti_Home();
                this.NavigationService.Navigate(clienti_home);
            }
        }

        /// <summary>
        /// Legge il file di sincronizzazione ed effettua il confronto sui progetti attuali aggiornando la voce sync
        /// </summary>
        private void CheckBox_sync_ultima_modifica()
        {
            if (!ultimaModifica.readSync(Globals.DATIsync + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"))
            {
                //MessageBox.Show("E05 - Il file " + Globals.DATIsync + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv" + " non esiste o è aperto da un altro programma.\n\nNon è possibile effettuare la sincronizzazione.");
                Console.WriteLine("E05 - Non è possibile effettuare la sincronizzazione. - Il file " + Globals.DATIsync +
                    Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv" + " non esiste o è aperto da un altro programma.", "E05"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
            else
            {
                if (!ultimaModifica.confrontoSync(progetti))
                {
                    MessageBox.Show("E' necessario aver caricato almeno una volta le date di ultime modifiche prima di effettuare " +
                        "la sincronizzazione", "Sincronizzazione non avvenuta"
                                     , MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                }
            }
        }

        /// <summary>
        /// Controllo iniziale: controlla che il numero di progetti in CLIENTI.csv coincida col numero dell'ultimo progetto
        /// </summary>
        private void InitCheck()
        {
            if (progetti.Count != 0 && progetti[progetti.Count - 1].numero != Globals.CLIENTI[num_cliente].getMaxId())
            {
                MessageBox.Show("Indice ultimo progetto diverso dal numero di progetti di questo cliente" +
                    progetti[progetti.Count - 1].numero + "  " + Globals.CLIENTI[num_cliente].getMaxId(), "Allarme inizializzazione"
                                     , MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
        }

        /// <summary>
        /// Impostazione iniziale della visibilità dei bottoni secondo le impostazioni
        /// </summary>
        private void SetVisibility()
        {
            Console.WriteLine("Set visibility");
            MenuItem ma = this.FindName("Menu_anteprima_check") as MenuItem;
            MenuItem ms = this.FindName("Menu_sync_check") as MenuItem;
            ma.IsChecked = Globals.ANTEPRIME;
            ms.IsChecked = Globals.SINCRONIZZAZIONE;
            if (!Globals.ANTEPRIME)
            {
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                Button button = this.FindName("buttonOpenDocx") as Button;
                Image image = this.FindName("image") as Image;

                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;

            }
            if (!Globals.SINCRONIZZAZIONE)
            {
                Button buttonModifiche = this.FindName("BottModifiche") as Button;
                Button buttonClone = this.FindName("BottGitClone") as Button;
                Button buttonPush = this.FindName("BottGitPush") as Button;
                Button buttonMerge = this.FindName("BottMerge") as Button;
                buttonModifiche.Visibility = Visibility.Hidden;
                buttonClone.Visibility = Visibility.Hidden;
                buttonPush.Visibility = Visibility.Hidden;
                buttonMerge.Visibility = Visibility.Hidden;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                                BOTTONI                                     ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bottone apertura cartella del filesystem.
        /// Imposta questo come ultimo progetto aperto.
        /// </summary>
        private void Button_Open_Folder(object sender, RoutedEventArgs e)
        {
            Globals.CLIENTI[num_cliente].setLastId(ProgSelezionato);
            string path = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + Globals.CLIENTI[num_cliente].getlastId();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        /// <summary>
        /// Bottone per la creazione di un nuovo progetto.
        /// Chiama il FORM Form1. (se non ci sono le condizioni può portare a Clienti_Home)
        /// </summary>
        private void Button_New_Project(object sender, RoutedEventArgs e)
        {
            if (CheckFolderandCsv(Globals.CLIENTI[num_cliente].getNomeCliente()))
            {
                Console.WriteLine("\nNew Project");
                Form1 testDialog = new Form1(Globals.CLIENTI[num_cliente]);
                testDialog.FormClosed
                    += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
                testDialog.ShowDialog();
            }
            else
            {
                Progetti_Home_Loaded(null, null);
            }
        }

        /// <summary>
        /// Apre il file docx attualmente visualizzato
        /// </summary>
        private void Button_Apri_Docx(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + ProgSelezionato + @"\progetto.docx");
        }

        /// <summary>
        /// Apre l'immagine su cui si è fatto click
        /// </summary>
        private void Apri_Immagine(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + ProgSelezionato + @"\anteprima.jpg");
        }

        /// <summary>
        /// Bottone che fa tornare alla pagina Clienti_Home
        /// </summary>
        private void Button_Cambia_Pagina(object sender, RoutedEventArgs e)
        {
            Clienti_Home clienti_home = new Clienti_Home();
            this.NavigationService.Navigate(clienti_home);
        }

        /// <summary>
        /// Bottone che attiva il controllo delle ultime modifiche di tutti i progetti nella cartella di questo cliente.
        /// (su un altro thread)
        /// Le aggiorna in progetti e ricarica la DataGrid. (se tutto è andato bene)
        /// [Disabilita i bottoni di sincronizzazione]
        /// </summary>
        private void Button_Ultime_Modifiche(object sender, RoutedEventArgs e)
        {
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            Button buttonClone = this.FindName("BottGitClone") as Button;
            Button buttonPush = this.FindName("BottGitPush") as Button;
            Button buttonMerge = this.FindName("BottMerge") as Button;
            buttonModifiche.IsEnabled = false;
            buttonClone.IsEnabled = false;
            buttonPush.IsEnabled = false;
            buttonMerge.IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                if (ultimaModifica.ricercaLenta(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\"))
                {
                    if (!ultimaModifica.writeInCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"))
                    {
                        MessageBox.Show("E04 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"
                            + " non esiste o è aperto da un altro programma. \n\nNon è stato possibile salvare i dati relativi alle" +
                            " ultime modifiche.", "E04"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    }
                    ultimaModifica.aggiornoModifiche(progetti);
                }
                else
                {
                    MessageBox.Show("E26 non riuscito aggiornamento ultime modifiche", "E26"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                }

            }).ContinueWith(task =>
            {
                buttonModifiche.IsEnabled = true;
                buttonClone.IsEnabled = true;
                buttonPush.IsEnabled = true;
                buttonMerge.IsEnabled = true;
                updateList("");
                MessageBox.Show("Le ultime modifiche di tutti i progetti di " + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    " sono state aggiornate e caricate nel relativo file csv.", "Modifiche aggiornate"
                                     , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Bottone che confronta i progetti in DATI e in DATIsync.
        /// Apre il FORM Form_ShowDifference.
        /// </summary>
        private void Button_Merge(object sender, RoutedEventArgs e)
        {
            Sync s = new Sync(progetti, num_cliente);
            s.readSyncProject(Globals.DATIsync + Globals.CLIENTI[num_cliente].getNomeCliente() + ".csv");
            List<Progetto>[] compare = s.compareSyncProject();
            Console.WriteLine("Progetti uguali = " + compare[0].Count + "\nProgetti mancanti localmente = " + compare[1].Count + "\nProgetti in più = " + compare[2].Count);
            Form_ShowDifference form = new Form_ShowDifference(compare[1], num_cliente);
            form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
            form.ShowDialog();
        }

        /// <summary>
        /// Bottone che effettua il clone del repository online.
        /// Poi aggiorna la DataGrid. 
        /// [Disabilita i bottoni di sincronizzazione] 
        /// </summary>
        private void Button_GitHubClone(object sender, RoutedEventArgs e)
        {
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            Button buttonClone = this.FindName("BottGitClone") as Button;
            Button buttonPush = this.FindName("BottGitPush") as Button;
            Button buttonMerge = this.FindName("BottMerge") as Button;
            buttonModifiche.IsEnabled = false;
            buttonClone.IsEnabled = false;
            buttonPush.IsEnabled = false;
            buttonMerge.IsEnabled = false;
            GitCommands git = new GitCommands();
            if (git.clone())
            {

                MessageBox.Show("Clone del repository " + Globals.GITURL + " nella cartella " + Globals.DATIsync + " riuscito correttamente!", "Clone riuscito"
                                 , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
            buttonModifiche.IsEnabled = true;
            buttonClone.IsEnabled = true;
            buttonPush.IsEnabled = true;
            buttonMerge.IsEnabled = true;
            updateList("");
        }

        /// <summary>
        /// Bottone che effettua il push dei file del cliente in DATI, mettendoli sul repository online.
        /// - chiede conferma
        /// - sostituisce nella cartella DATIsync
        /// - effettua il push e da all'utente informazioni sull'esito
        /// - ricarica la DataGrid
        /// [Disabilita i bottoni di sincronizzazione] 
        /// </summary>
        private void Button_GitHubPush(object sender, RoutedEventArgs e)
        {
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            Button buttonClone = this.FindName("BottGitClone") as Button;
            Button buttonPush = this.FindName("BottGitPush") as Button;
            Button buttonMerge = this.FindName("BottMerge") as Button;
            buttonModifiche.IsEnabled = false;
            buttonClone.IsEnabled = false;
            buttonPush.IsEnabled = false;
            buttonMerge.IsEnabled = false;
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler caricare i progetti attuali online",
                "Caricare online?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                GitCommands git = new GitCommands();
                if (git.copyFolder(Globals.CLIENTI[num_cliente].getNomeCliente()))
                {
                    List<string> pushInfo = git.push(Globals.CLIENTI[num_cliente].getNomeCliente());
                    if (pushInfo != null)
                    {

                        if (pushInfo.Count != 0)
                        {
                            string message = "";
                            foreach (string f in pushInfo)
                            {
                                message += "\n" + f;
                            }
                            MessageBox.Show("Upload sul repository " + Globals.GITURL + " riuscito correttamente! File caricati:\n" + message, "Upload riuscito"
                                         , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        }
                        else
                        {
                            MessageBox.Show("Upload sul repository " + Globals.GITURL + " non effettuato perchè non c'era niente da caricare", "Niente da caricare"
                                        , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Upload sul repository " + Globals.GITURL + " non riuscito.", "Errore upload"
                                            , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    }
                }
            }
            //else if (dialogResult == MessageBoxResult.No) { }
            buttonModifiche.IsEnabled = true;
            buttonClone.IsEnabled = true;
            buttonPush.IsEnabled = true;
            buttonMerge.IsEnabled = true;
            updateList("");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                        INTERAZIONE CON L'UTENTE                            ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Open_Folder(sender, null);
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
                Button_Open_Folder(null, null);
            }
        }

        /// <summary>
        /// Richiamato ogni volta che cambia la selezione nella DataGrid.
        /// - aggiorna ProgSelezionato.
        /// - carica immagine di anteprima e docx (se disponibili)
        /// NullReferenceException spesso sollevata perchè quando si cerca si ricarica la DataGrid e 
        /// per un momento non c'è nessun progetto selezionato. 
        /// </summary>
        private void ChangePreview(object sender, EventArgs e)
        {
            Console.WriteLine("Change Preview");
            try
            {
                ProgSelezionato = ((Progetto)((DataGrid)sender).SelectedValue).getNumProject();
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                Button button = this.FindName("buttonOpenDocx") as Button;
                Image image = this.FindName("image") as Image;
                if (Globals.ANTEPRIME)
                {
                    richTextBox.Visibility = Visibility.Visible;
                    button.Visibility = Visibility.Visible;
                }
                string file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                              @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + ProgSelezionato + @"\progetto.docx";
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
                   @"\" + Globals.CLIENTI[num_cliente].getNomeCliente() + ProgSelezionato + @"\anteprima.jpg";
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
            catch (NullReferenceException) 
            {
                //Console.WriteLine("ECCEZIONE: " + nre);
            }
        }

        /// <summary>
        /// Richiamato ogni volta che viene modificato il testo nella TextBox di ricerca.
        /// Aggiorna e filtra la lista e poi seleziona la prima riga visualizzata.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Progetto p = updateList(((TextBox)sender).Text);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.SelectedIndex = 0;
            dataGrid.ScrollIntoView(p);
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
            RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            Button button = this.FindName("buttonOpenDocx") as Button;
            Image image = this.FindName("image") as Image;
            if (value)
            {
                button.Visibility = Visibility.Visible;
                richTextBox.Visibility = Visibility.Visible;
                image.Visibility = Visibility.Visible;
            }
            else
            {
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
            }
            if (value != Globals.ANTEPRIME)
            {
                Globals.ANTEPRIME = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }
        }

        /// <summary>
        /// Imposta la visibilità dei bottoni di sincronizzazione.
        /// Aggiorna la variabile Globals.SINCRONIZZAZIONE e scrive sul .csv.
        /// </summary>
        private void Menu_sync(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            Button buttonClone = this.FindName("BottGitClone") as Button;
            Button buttonPush = this.FindName("BottGitPush") as Button;
            Button buttonMerge = this.FindName("BottMerge") as Button;
            if (value)
            {
                buttonModifiche.Visibility = Visibility.Visible;
                buttonClone.Visibility = Visibility.Visible;
                buttonPush.Visibility = Visibility.Visible;
                buttonMerge.Visibility = Visibility.Visible;
            }
            else
            {
                buttonModifiche.Visibility = Visibility.Hidden;
                buttonClone.Visibility = Visibility.Hidden;
                buttonPush.Visibility = Visibility.Hidden;
                buttonMerge.Visibility = Visibility.Hidden;
            }
            if (value != Globals.SINCRONIZZAZIONE)
            {
                Globals.SINCRONIZZAZIONE = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }

        }
    }
}
