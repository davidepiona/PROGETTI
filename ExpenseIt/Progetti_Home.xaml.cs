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
        private bool back = false;
        private int num_cliente;
        private int ProgSelezionato;
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
                MessageBox.Show("E01 - Il file " + Globals.DATI + @"\CLIENTI.csv" + " non esiste o è aperto da un altro programma. \n L'APPLICAZIONE SARA' CHIUSA");
                Environment.Exit(0);
            }

            num_cliente = Globals.CLIENTI.FindIndex(x => x.getSuffisso().Equals(Globals.LAST_CLIENT));
            string cliente = Globals.CLIENTI.Find(x => x.getSuffisso().Equals(Globals.LAST_CLIENT)).getNomeCliente();
            if (checkFolderandCsv(Globals.CLIENTI[num_cliente].getNomeCliente()))
            {
                initialize();
            }

        }
        public bool checkFolderandCsv(string cliente)
        {
            if (!System.IO.Directory.Exists(Globals.PROGETTI + cliente) || !System.IO.File.Exists(Globals.DATI + cliente + ".csv"))
            {
                MessageBox.Show("La cartella o il file csv del cliente attuale " + cliente + " non è presente");
                back = true;
                return false;
            }
            return true;
        }
        private void initialize()
        {
            readProjects();
            ProgSelezionato = Globals.CLIENTI[num_cliente].getlastId();
            ultimaModifica = new UltimaModifica(Globals.CLIENTI[num_cliente]);
            if (!ultimaModifica.readByCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv"))
            {
                MessageBox.Show("E02 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv" + " non esiste o è aperto da un altro programma.\n\nLe ultime modifiche dei progetti non saranno caricate da file.");
            }
            ultimaModifica.aggiornoModifiche(progetti);
            check_sync();
            updateList("");
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + " " + Globals.CLIENTI[num_cliente].getSuffisso();
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            initCheck();
            setVisibility();

        }

        private void setVisibility()
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
        private void readProjects()
        {
            Console.WriteLine("Read Projects");
            List<string> lines = new List<string>();
            //Console.WriteLine(Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + ".csv");
            try
            {
                using (var reader = new CsvFileReader(Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + ".csv"))
                {

                    while (reader.ReadRow(lines) && lines.Count != 0 && lines != null)
                    {
                        //Console.WriteLine(lines[0]);
                        int num = Int32.Parse(lines[0]);
                        reader.ReadRow(lines);
                        string nome = lines[0];
                        reader.ReadRow(lines);
                        string tipoPLC = lines[0];
                        reader.ReadRow(lines);
                        string tipoOP = lines[0];
                        reader.ReadRow(lines);
                        string data = lines[0];
                        progetti.Add(new Progetto(num, nome, tipoOP, tipoOP, data, Globals.CLIENTI[num_cliente].getSuffisso()));
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("E03 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + ".csv" + " non esiste o è aperto da un altro programma");
            }
        }


        private void createList()
        {
            Console.WriteLine("Create List");
            //string path = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + cliente.getSuffisso();
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
                    dataGrid.ScrollIntoView(progetti[i]);

                }
                i++;
            }

            //dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[10].Index;
            //

        }

        private Progetto updateList(string filter)
        {
            Console.WriteLine("Update list1");
            Progetto primo = new Progetto(0, null, null, null, null, null);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            ultimaModifica.aggiornoModifiche(progetti);
            //Console.WriteLine("filter: <" + filter + ">");
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
                        if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId()))
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

        private void updateListNewProject(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("UpdateList2");
            progetti = new List<Progetto>();
            readProjects();
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            ultimaModifica.aggiornoModifiche(progetti);
            //Console.WriteLine("filter: <" + filter + ">");
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

        private void Button_Open_Folder(object sender, RoutedEventArgs e)
        {
            Globals.CLIENTI[num_cliente].setLastId(ProgSelezionato);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            string path = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() + @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + Globals.CLIENTI[num_cliente].getlastId();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void Button_New_Project(object sender, RoutedEventArgs e)
        {
            if (checkFolderandCsv(Globals.CLIENTI[num_cliente].getNomeCliente()))
            {
                Console.WriteLine("\nNew Project");
                Form1 testDialog = new Form1(Globals.CLIENTI[num_cliente]);

                testDialog.FormClosed
                    += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
                testDialog.ShowDialog();
                //Console.WriteLine("\nNew Project");
                //FormNuovoCliente testDialog = new FormNuovoCliente();
                //testDialog.ShowDialog();
            }
            else
            {
                Progetti_Home_Loaded(null, null);
            }
        }

        private void changePreview(object sender, EventArgs e)
        {
            Console.WriteLine("Change Preview");
            try
            {
                //Globals.CLIENTI[num_cliente].setLastId(((Progetto)((DataGrid)sender).SelectedValue).getNumProject());
                ProgSelezionato = ((Progetto)((DataGrid)sender).SelectedValue).getNumProject();
                //Globals.CLIENTI[num_cliente].setLastId(12);
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;

                Button button = this.FindName("buttonOpenDocx") as Button;
                Image image = this.FindName("image") as Image;
                if (Globals.ANTEPRIME)
                {
                    richTextBox.Visibility = Visibility.Visible;
                    button.Visibility = Visibility.Visible;
                }

                //Console.WriteLine("PATHHH:  " + Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() +
                //@"\" + Globals.CLIENTI[num_cliente].getSuffisso() + Globals.CLIENTI[num_cliente].getlastId() + @"\progetto.docx");
                string file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() +
                    @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\progetto.docx";
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

                file = Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() +
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
                }

            }
            catch (NullReferenceException nre)
            {
                //Console.WriteLine("ECCEZIONE: " + nre);
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Progetto p = updateList(((TextBox)sender).Text);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.SelectedIndex = 0;
            dataGrid.ScrollIntoView(p);
        }

        private void Button_Apri_Docx(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() +
                @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\progetto.docx");
        }

        private void Apri_Immagine(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() +
                @"\" + Globals.CLIENTI[num_cliente].getSuffisso() + ProgSelezionato + @"\anteprima.jpg");
        }


        private void Button_Ultime_Modifiche(object sender, RoutedEventArgs e)
        {
            //UltimaModifica u = new UltimaModifica(cliente);
            //Console.WriteLine(u.modificheByFile(PROGETTI + cliente.getSuffisso() + @"\PATA190"));
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
                ultimaModifica.ricercaLenta(Globals.PROGETTI + Globals.CLIENTI[num_cliente].getSuffisso() + @"\");
                if (!ultimaModifica.writeInCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv"))
                {
                    MessageBox.Show("E04 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv" + " non esiste o è aperto da un altro programma. \n\nNon è stato possibile salvare i dati relativi alle ultime modifiche.");
                }
                ultimaModifica.aggiornoModifiche(progetti);


            }).ContinueWith(task =>
            {

                buttonModifiche.IsEnabled = true;
                buttonClone.IsEnabled = true;
                buttonPush.IsEnabled = true;
                buttonMerge.IsEnabled = true;
                check_sync();
                updateList("");
                MessageBox.Show("Le ultime modifiche di tutti i progetti di " + Globals.CLIENTI[num_cliente].getNomeCliente() + " sono state aggiornate e caricate nel relativo file csv.");

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }


        private void check_sync()
        {
            if (!ultimaModifica.readSync(Globals.DATIsync + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv"))
            {
                //MessageBox.Show("E05 - Il file " + Globals.DATIsync + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv" + " non esiste o è aperto da un altro programma.\n\nNon è possibile effettuare la sincronizzazione.");
                Console.WriteLine("E05 - Il file " + Globals.DATIsync + Globals.CLIENTI[num_cliente].getSuffisso() + "date.csv" + " non esiste o è aperto da un altro programma.\n\nNon è possibile effettuare la sincronizzazione.");
            }
            else
            {
                if (!ultimaModifica.confrontoSync(progetti))
                {
                    MessageBox.Show("E' necessario aver caricato almeno una volta le date di ultime modifiche prima di effettuare la sincronizzazione");
                }

            }
        }

        private void Button_Merge(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
            Sync s = new Sync(progetti, num_cliente);
            s.readSyncProject(Globals.DATIsync + Globals.CLIENTI[num_cliente].getSuffisso() + ".csv");
            List<Progetto>[] compare = s.compareSyncProject();
            Console.WriteLine("Progetti uguali = " + compare[0].Count + "\nProgetti mancanti localmente = " + compare[1].Count + "\nProgetti in più = " + compare[2].Count);
            Form_ShowDifference form = new Form_ShowDifference(compare[1], num_cliente);
            form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.updateListNewProject);
            form.ShowDialog();
        }

        private void Button_Cambia_Pagina(object sender, RoutedEventArgs e)
        {
            Clienti_Home clienti_home = new Clienti_Home();
            this.NavigationService.Navigate(clienti_home);
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Open_Folder(sender, null);
        }

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

        private void initCheck()
        {
            if (progetti.Count != 0 && progetti[progetti.Count - 1].numero != Globals.CLIENTI[num_cliente].getMaxId())
            {
                MessageBox.Show("ALLARME IN INIZIALIZZAZIONE: numero di progetti segnati diverso dal numero di progetti effettivi" +
                    progetti[progetti.Count - 1].numero + "  " + Globals.CLIENTI[num_cliente].getMaxId());
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_GitHubClone(object sender, RoutedEventArgs e)
        {
            GitCommands git = new GitCommands();
            git.clone();
            check_sync();
            updateList("");
        }
        private void Button_GitHubPush(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler caricare i progetti attuali online", "Caricare online?", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                GitCommands git = new GitCommands();
                if (git.copyFolder())
                {
                    if (git.push())
                    {
                        MessageBox.Show("Upload sul repository " + Globals.GITURL + " riuscito correttamente!");
                    }
                }
            }
            else if (dialogResult == MessageBoxResult.No)
            {

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
