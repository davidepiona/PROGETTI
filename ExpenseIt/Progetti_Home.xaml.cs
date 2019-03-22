using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DATA
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
            int j = 0;
            try
            {
                if (Globals.CLIENTI == null)
                {
                    Globals.log.Info("Lettura CLIENTI.csv");
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
                        j++;
                    }
                    file.Close();
                }
            }
            catch (IOException)
            {
                string msg = "E01 - Il file " + Globals.DATI + @"CLIENTI.csv" +
                    " non esiste o è aperto da un altro programma. \n L'APPLICAZIONE SARA' CHIUSA";
                MessageBox.Show(msg, "E01"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
                Environment.Exit(0);
            }
            catch (FormatException)
            {
                string msg = "E35 - Il file " + Globals.DATI + @"CLIENTI.csv" +
                    " è in un formato non corretto. \nProblema riscontrato al cliente numero: " + j;
                MessageBox.Show(msg, "E35", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            try
            {
                num_cliente = Globals.CLIENTI.FindIndex(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT));
                string cliente = Globals.CLIENTI.Find(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT)).getNomeCliente();
            }
            catch (NullReferenceException)
            {
                string msg = "E34 - non è possibile raggiungere alcune informazioni relative al cliente" + Globals.LAST_CLIENT + ". \nL'applicazione sarà chiusa";
                MessageBox.Show(msg, "E34"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
                Environment.Exit(0);
            }
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
            Globals.log.Info("Leggo date.csv");
            if (!ultimaModifica.readByCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"))
            {
                string msg = "E02 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    "date.csv" + " non esiste o è aperto da un altro programma.\n\nLe ultime modifiche dei progetti non " +
                    "saranno caricate da file.";
                //MessageBox.Show(msg, "E02", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Warn(msg);
            }
            ultimaModifica.aggiornoModifiche(progetti);
            CheckBox_sync_ultima_modifica();
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + " " + Globals.CLIENTI[num_cliente].getNomeCliente().Replace("_", "__");
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            //InitCheck(); //disabilitato perchè si è cambiato il significato di MaxId in Clienti
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
            Globals.log.Info("Read Projects");
            List<string> lines = new List<string>();
            int j = 0;
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
                        progetti.Add(new Progetto(num, nome, tipoPLC, tipoOP, data, Globals.CLIENTI[num_cliente].getNomeCliente(), Globals.CLIENTI[num_cliente].getSuffisso()));
                        j++;
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E03 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    ".csv" + " non esiste o è aperto da un altro programma";
                MessageBox.Show(msg, "E03"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            catch (FormatException)
            {
                string msg = "E31 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + ".csv" +
                    " è in un formato non corretto.\nProblema riscontrato al progetto numero: " + j;
                MessageBox.Show(msg, "E31", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            catch (ArgumentOutOfRangeException)
            {
                string msg = "E37 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + ".csv" +
                    " è in un formato non corretto.\nProblema riscontrato al progetto numero: " + j + "  - ArgumentOutOfRangeException";
                MessageBox.Show(msg, "E37", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Aggiunge la lista di progetti alla DataGrid dopo averla svuotata.
        /// Mentre li scorre cerca quello che era stato aperto per ultimo e quando lo trova lo seleziona.
        /// </summary>
        private void createList()
        {
            Console.WriteLine("Create List");
            Globals.log.Info("Create List");
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
            Globals.log.Info("Update list1");
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
            Globals.log.Info("UpdateList2");
            progetti = new List<Progetto>();
            readProjects();
            CheckBox_sync_ultima_modifica();
            ultimaModifica.checkLastModifyProject(progetti.Last().sigla, Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\" + progetti.Last().sigla + @"\");
            
            ultimaModifica.aggiornoModifiche(progetti);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
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
                string msg = "La cartella o il file csv del cliente attuale " + cliente + " non è presente";
                MessageBox.Show(msg, "Informazioni assenti"
                                     , MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Warn(msg);
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
                string msg = "E05 - Non è possibile effettuare la sincronizzazione. - Il file " + Globals.DATIsync +
                    Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv" + " non esiste o è aperto da un altro programma.";
                //MessageBox.Show(msg , "E05" , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Console.WriteLine(msg);
                Globals.log.Error(msg);
            }
            else
            {
                if (!ultimaModifica.confrontoSync(progetti))
                {
                    string msg = "E' necessario aver caricato almeno una volta le date di ultime modifiche prima di effettuare la sincronizzazione";
                    //MessageBox.Show(msg, "Sincronizzazione non avvenuta", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
            }
        }

        /// <summary>
        /// Controllo iniziale: controlla che il numero di progetti in CLIENTI.csv coincida col numero dell'ultimo progetto
        /// NON PIU' UTILIZZATO
        /// </summary>
        private void InitCheck()
        {
            if (progetti.Count != 0 && progetti[progetti.Count - 1].numero != Globals.CLIENTI[num_cliente].getMaxId())
            {
                string msg = "L'indice ultimo progetto è '"+ progetti[progetti.Count - 1].numero  + "' ed è diverso dal numero di progetti del cliente " 
                    + Globals.CLIENTI[num_cliente].getNomeCliente() + " che sono '" + Globals.CLIENTI[num_cliente].getMaxId() +
                    "'\nAggiornare il numero modificando l'indice di ultimo progetto?";
                Globals.log.Warn(msg);
                MessageBoxResult m = MessageBox.Show(msg, "Allarme inizializzazione"
                                     , MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                if (m == MessageBoxResult.Yes)
                {
                    Globals.CLIENTI[num_cliente].setMaxId(progetti.Count);
                    MainWindow mw = new MainWindow();
                    mw.salvaClientiCSV();
                }
            }
        }

        /// <summary>
        /// Impostazione iniziale della visibilità dei bottoni secondo le impostazioni
        /// </summary>
        private void SetVisibility()
        {
            Console.WriteLine("Set visibility");
            Globals.log.Info("Set visibility");
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
                Button buttonClone = this.FindName("BottGitClone") as Button;
                Button buttonPush = this.FindName("BottGitPush") as Button;
                Button buttonMerge = this.FindName("BottMerge") as Button;
                buttonClone.Visibility = Visibility.Hidden;
                buttonPush.Visibility = Visibility.Hidden;
                buttonMerge.Visibility = Visibility.Hidden;
            }
            if (!Globals.importCsvEnabled)
            {
                MenuItem menu = this.FindName("Menu") as MenuItem;
                MenuItem importCsv = this.FindName("importCsv") as MenuItem;
                menu.Items.Remove(importCsv);
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
            string path = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + Globals.CLIENTI[num_cliente].getlastId();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        /// <summary>
        /// Bottone per la creazione di un nuovo progetto.
        /// Chiama il FORM Form1. (se non ci sono le condizioni può portare a Clienti_Home)
        /// </summary>
        private void Button_New_Project(object sender, RoutedEventArgs e)
        {
            if (CheckFolderandCsv(Globals.CLIENTI[num_cliente].getNomeCliente()))
            {
                Console.WriteLine("New Project");
                Globals.log.Info("New Project");
                int numUltimoProgetto; 
                if (progetti.Count == 0)
                {
                    numUltimoProgetto = 0;
                }
                else
                {
                    numUltimoProgetto = progetti[progetti.Count - 1].numero;
                }
                Form1 testDialog = new Form1(Globals.CLIENTI[num_cliente], numUltimoProgetto);

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
        /// Controlla se esiste un file progetto.docx nel progetto attualmente visualizzato 
        /// - se esist: apre il file docx
        /// - se non esiste : crea un file docx con tutte le informazioni del progetto
        /// </summary>
        private void Button_Apri_Docx(object sender, RoutedEventArgs e)
        {
            string file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\progetto.docx";
            if (File.Exists(file))
            {
                System.Diagnostics.Process.Start(file);
            }
            else
            {
                Progetto progetto = progetti.Find(x => x.sigla.Equals(Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato));
                if (progetto != null)
                {
                    try
                    {
                        var doc = Xceed.Words.NET.DocX.Create(file);
                        doc.InsertParagraph(Globals.CLIENTI[num_cliente].getNomeCliente() + " " + ProgSelezionato).Bold();
                        doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + progetto.nome);
                        doc.InsertParagraph("\n TIPO DI PLC: " + progetto.tipoPLC);
                        doc.InsertParagraph("\n TIPO DI OP: " + progetto.tipoOP);
                        doc.InsertParagraph("\n DATA INIZIO: " + progetto.data);
                        doc.InsertParagraph("\n Note:");
                        doc.Save();
                    }
                    catch (IOException)
                    {
                        string msg = "E44 - Il file " + file + " non è stato creato per un problema";
                        MessageBox.Show(msg, "E44", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                    DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                    ChangePreview(dataGrid, null);
                }
            }
        }

        /// <summary>
        /// Apre l'immagine su cui si è fatto click
        /// </summary>
        private void Apri_Immagine(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\anteprima.jpg");
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
                        string msg = "E04 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"
                            + " non esiste o è aperto da un altro programma. \n\nNon è stato possibile salvare i dati relativi alle" +
                            " ultime modifiche.";
                        MessageBox.Show(msg, "E04"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                    ultimaModifica.aggiornoModifiche(progetti);
                }
                else
                {
                    string msg = "E26 non riuscito aggiornamento ultime modifiche";
                    MessageBox.Show(msg, "E26"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }

            }).ContinueWith(task =>
            {
                buttonModifiche.IsEnabled = true;
                buttonClone.IsEnabled = true;
                buttonPush.IsEnabled = true;
                buttonMerge.IsEnabled = true;
                updateList("");
                string msg = "Le ultime modifiche di tutti i progetti di " + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    " sono state aggiornate e caricate nel relativo file csv.";
                MessageBox.Show(msg, "Modifiche aggiornate"
                                     , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Info(msg);
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
            //List<Progetto>[] compare = s.compareSyncProject();
            //Console.WriteLine("Progetti uguali = " + compare[0].Count + "\nProgetti mancanti localmente = " + compare[1].Count + "\nProgetti in più = " + compare[2].Count);
            //Globals.log.Info("Progetti uguali = " + compare[0].Count + "\nProgetti mancanti localmente = " + compare[1].Count + "\nProgetti in più = " + compare[2].Count);
            //Form_ShowDifference form = new Form_ShowDifference(compare[1], num_cliente);
            //form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
            //form.ShowDialog();
            List < Confronto > list = s.compareProjectsLists(); 
                Window_ShowDifference win2 = new Window_ShowDifference(list, progetti);
            win2.Closed
                            += new EventHandler(updateListAfterMerge);
            win2.ShowDialog();
        }

        private void updateListAfterMerge(object sender, EventArgs e)
        {
            Console.WriteLine("UpdateList3");
            Globals.log.Info("UpdateList3");
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
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler SCARICARE i progetti di questo cliente dal repository online e salvarli sul tuo computer in DATIsync?",
                "Effettuare checkout?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                if (git.Checkout(Globals.CLIENTI[num_cliente].getNomeCliente()))
                {
                    string msg = "Checkout dal repository " + Globals.GITURL + " nella cartella " + Globals.DATIsync + " riuscito correttamente!";
                    MessageBox.Show(msg, "Checkout riuscito"
                                     , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Info(msg);
                }
                else
                {
                    string msg = "Checkout dal repository " + Globals.GITURL + " nella cartella " + Globals.DATIsync + "NON riuscito";
                    MessageBox.Show(msg, "Checkout non riuscito"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
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
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler caricare i progetti di" + Globals.CLIENTI[num_cliente].getNomeCliente() + " online?",
                "Caricare online?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                GitCommands git = new GitCommands();
                if (git.copyFile(Globals.CLIENTI[num_cliente].getNomeCliente()))
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
            buttonModifiche.IsEnabled = true;
            buttonClone.IsEnabled = true;
            buttonPush.IsEnabled = true;
            buttonMerge.IsEnabled = true;
            updateList("");
        }

        /// <summary>
        /// Bottone che apre il form per la modifica del progetto selezionato
        /// Possibilità di cambiarne i parametri
        /// Possibilità di eliminare il progetto
        /// </summary>
        private void Button_Modify(object sender, RoutedEventArgs e)
        {
            if (ProgSelezionato != 0)
            {
                try
                {
                    Progetto itemToRemove = progetti.Find(r => r.numero == ProgSelezionato);
                    Form_Modifica form = new Form_Modifica(progetti, itemToRemove);
                    form.FormClosed
                            += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
                    form.ShowDialog();
                }
                catch (InvalidOperationException)
                {
                    Globals.log.Info("Non è stato trovato nessun elemento con indice " + ProgSelezionato + " , probabilmente è stato eliminato in precedenza");
                }
            }
        }

        /// <summary>
        /// Bottone che apre un popup per visualizzare l'ultimo file modificato e la relativa data di modifica
        /// </summary>
        private void ButtonLastModify(object sender, RoutedEventArgs e)
        {
                string fileName = "";
                string data = "";
                try
                {
                    string path = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() + @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato;
                    TextBox textBox = this.FindName("TextBoxModify") as TextBox;
                    DateTime date = ultimaModifica.modificheByFile2(path, textBox.Text);
                    string fileName2 = ultimaModifica.getFileName();
                    ultimaModifica.setFileName("");
                    fileName = fileName2.Substring(path.Length + 1);
                    data = date.ToString();
                    if (data.Equals("01/01/0001 00:00:00"))
                    {
                        data = "";
                    }
                }catch(ArgumentOutOfRangeException)
                {
                    Globals.log.Error("ArgumentOutOfRangeException in button last modify");
                }
                catch (NullReferenceException)
                {
                    Globals.log.Error("NullReferenceException in button last modify");
                }

                Form_LastModify form = new Form_LastModify(fileName, data);
                form.Show();
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
                dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            if (e.Key == Key.Enter)
            {
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
            Globals.log.Info("Change Preview");
            RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            Button button = this.FindName("buttonOpenDocx") as Button;
            Image image = this.FindName("image") as Image;
            try
            {
                ProgSelezionato = ((Progetto)((DataGrid)sender).SelectedValue).numero;
                
                if (Globals.ANTEPRIME)
                {
                    richTextBox.Visibility = Visibility.Visible;
                    button.Visibility = Visibility.Visible;
                    image.Visibility = Visibility.Visible;

                    string file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                                  @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\progetto.docx";
                    if (File.Exists(file))
                    {
                        try
                        {
                            var doc = Xceed.Words.NET.DocX.Load(file);
                            richTextBox.Document.Blocks.Clear();
                            richTextBox.AppendText(doc.Text);
                        }
                        catch (IOException)
                        {
                            string msg = "E52 - Il file " + file + " non è accessibile";
                            Globals.log.Error(msg);
                        }
                    }
                    else
                    {
                        richTextBox.Document.Blocks.Clear();
                        richTextBox.Visibility = Visibility.Hidden;
                        //button.Visibility = Visibility.Hidden;
                    }

                    file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                       @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\anteprima.jpg";
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
                        image.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                //Console.WriteLine("ECCEZIONE: " + nre);
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                Globals.log.Warn("Eccezione in changePreview: " + nre);
            }
            if (!Globals.ANTEPRIME)
            {
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
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
            if (value != Globals.ANTEPRIME)
            {
                Globals.ANTEPRIME = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }
            if (value)
            {
                button.Visibility = Visibility.Visible;
                richTextBox.Visibility = Visibility.Visible;
                image.Visibility = Visibility.Visible;
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                ChangePreview(dataGrid, null);
            }
            else
            {
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
            }

        }

        /// <summary>
        /// Imposta la visibilità dei bottoni di sincronizzazione.
        /// Aggiorna la variabile Globals.SINCRONIZZAZIONE e scrive sul .csv.
        /// </summary>
        private void Menu_sync(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;
            Button buttonClone = this.FindName("BottGitClone") as Button;
            Button buttonPush = this.FindName("BottGitPush") as Button;
            Button buttonMerge = this.FindName("BottMerge") as Button;
            if (value)
            {
                buttonClone.Visibility = Visibility.Visible;
                buttonPush.Visibility = Visibility.Visible;
                buttonMerge.Visibility = Visibility.Visible;
            }
            else
            {
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

        /// <summary>
        /// Apre il FORM Form_aggiornaCSV importare file .csv per MATRIX e renderli in formato DATA.
        /// </summary>
        private void Menu_importa_CSV(object sender, RoutedEventArgs e)
        {
            Form_aggiornaCSV form = new Form_aggiornaCSV();
            form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
            form.ShowDialog();
        }

        /// <summary>
        /// Se l'utente conferma crea un file progetto.docx per ogni progetto del cliente attuale
        /// Inserisce i dati del progetto
        /// </summary>
        private void Menu_DOCX(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler CREARE un file progetto.docx in ogni progetto di questo cliente?",
                "Creare TUTTI i DOCX?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                foreach (Progetto p in progetti)
                {
                    string file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getNomeCliente() +
                    @"\" + p.sigla + @"\progetto.docx";
                    if (!File.Exists(file))
                    {
                        try
                        {
                            var doc = Xceed.Words.NET.DocX.Create(file);
                            doc.InsertParagraph(Globals.CLIENTI[num_cliente].getNomeCliente() + " " + p.numero).Bold();
                            doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + p.nome);
                            doc.InsertParagraph("\n TIPO DI PLC: " + p.tipoPLC);
                            doc.InsertParagraph("\n TIPO DI OP: " + p.tipoOP);
                            doc.InsertParagraph("\n DATA INIZIO: " + p.data);
                            doc.InsertParagraph("\n Note:");
                            doc.Save();
                        }
                        catch (IOException)
                        {
                            string msg = "E45 - Il file " + file + " non è stato creato per un problema";
                            System.Windows.MessageBoxResult me = System.Windows.MessageBox.Show(
                                    msg, "E45", MessageBoxButton.OK,
                                    MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                            Globals.log.Warn(msg);
                        }
                    }
                }
            }
        }
    }
}
