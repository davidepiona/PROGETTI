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
using System.Windows.Shapes;

namespace DATA
{
    /// <summary>
    /// Logica di interazione per Window_ShowDifference.xaml
    /// </summary>
    public partial class Window_ShowDifference : Window
    {
        private List<Confronto> list;
        private List<Progetto> progetti;
        public Window_ShowDifference(List<Confronto> list, List<Progetto> progetti)
        {
            this.list = list;
            this.progetti = progetti;
            InitializeComponent();
            Console.WriteLine("Create List");
            Globals.log.Info("Create List");
            DataGrid dataGrid = this.FindName("dataGrid2") as DataGrid;
            dataGrid.Items.Clear();
            int i = 0;
            foreach (Confronto c in list)
            {
                if(c.prog2 == null)
                {
                    c.scelto1 = true;
                }
                dataGrid.Items.Add(c);
                i++;
            }
        }

        /// <summary>
        /// Metodo che si attiva quando viene effettuato un click sulla cella contenente la checkbox (scelto1)
        /// - Cambia il valore della CheckBox cliccata 
        /// - (se la casella cliccata è relativa ad un progetto che non esiste non abilita nessuna funzione)
        /// - se la CheckBox opposta (scelto2) è true la mette a false
        /// - cancella e ricarica la DataGrid
        /// </summary>
        void OnClick1(object sender, RoutedEventArgs e)
        {
            bool value = ((DataGridCell)sender).Content.ToString().Split(':').Last().Equals("True") ? true : false;
            DataGridRow r2 = DataGridRow.GetRowContainingElement((DataGridCell)sender);
            if (r2 != null)
            {
                int index = r2.GetIndex();
                if (!value)
                {
                    if (list[index].prog1 != null)
                    {
                        list[index].scelto2 = false;
                        list[index].scelto1 = true;
                    }
                }
                else
                {
                    list[index].scelto1 = false;
                }
                DataGrid dataGrid = this.FindName("dataGrid2") as DataGrid;
                dataGrid.Items.Clear();
                foreach (Confronto c in list)
                {
                    dataGrid.Items.Add(c);
                }
            }
        }

        /// <summary>
        /// Metodo che si attiva quando viene effettuato un click sulla cella contenente la checkbox (scelto2)
        /// - Cambia il valore della CheckBox cliccata 
        /// - (se la casella cliccata è relativa ad un progetto che non esiste non abilita nessuna funzione)
        /// - se la CheckBox opposta (scelto1) è true la mette a false
        /// - cancella e ricarica la DataGrid
        /// </summary>
        void OnClick2(object sender, RoutedEventArgs e)
        {
            bool value = ((DataGridCell)sender).Content.ToString().Split(':').Last().Equals("True") ? true : false;
            DataGridRow r2 = DataGridRow.GetRowContainingElement((DataGridCell)sender);
            int index = r2.GetIndex();
            if (!value)
            {
                if (list[index].prog2 != null)
                {
                    list[index].scelto1 = false;
                    list[index].scelto2 = true;
                }
            }
            else
            {
                list[index].scelto2 = false;
            }
            DataGrid dataGrid = this.FindName("dataGrid2") as DataGrid;
            dataGrid.Items.Clear();
            foreach (Confronto c in list)
            {
                dataGrid.Items.Add(c);
            }
        }
        
        /// <summary>
        /// Metodo che si attiva quando viene cliccato il bottone "Apporta Modifiche"
        /// - chiama il metodo listSelectedProjects che crea una lista dei progetti selezionati dall'utente
        /// - per ogni progetto, se è nuovo lo crea in fondo, se è una modifica di uno esistente la apporta
        /// - scrive la lista completa di progetti sul file *CLIENTE*.csv
        /// </summary>
        private void BottModifiche_Click(object sender, RoutedEventArgs e)
        {
            List<Progetto> progSelez = listSelectedProjects();
            if(progSelez == null)
            {
                return;
            }
            
            foreach (Progetto prog in progSelez)
            {
                if (prog.sync == false) //NUOVO
                {
                    addProject(prog);
                }
                if (prog.sync == null) //SCELTO
                {
                    modifyProject(prog);
                }
            }
            MainWindow m = new MainWindow();
            m.salvaClientiCSV();

            if (scriviCSV())
            {
                Globals.log.Info("Progetti aggironati correttamente");
                System.Windows.MessageBox.Show("Progetti aggiornati correttamente", "aggiornati", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
            else
            {
                Globals.log.Error("Problema nell'aggiornamento dei progetti");
                System.Windows.MessageBox.Show("Problema nell'aggiornamento dei progetti", "Problema aggiornamento", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
            this.Close();
        }

        /// <summary>
        /// Metodo ausiliario che restituisce la lista di progetti selezionati dall'utente
        /// Per ogni progetto definisce l'attributo sync in modo da aver informazioni riguardo la sua natura: 
        /// - nuovo
        /// - eliminato nella nuova versione
        /// - modificato
        /// </summary>
        private List<Progetto> listSelectedProjects()
        {
            List<Progetto> progSelez = new List<Progetto>();
            foreach (Confronto c in list)
            {
                if (c.scelto1 && c.scelto2)
                {
                    string msg = "E53 - Errore, non è possibile lo stato con entrambi i progSelez selezionati";
                    MessageBox.Show(msg, "E53"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                    return null;
                }
                if (c.scelto1)
                {
                    if (c.prog2 == null)
                    {
                        c.prog1.sync = true; //GIA' ESISTENTE
                    }
                    else
                    {
                        c.prog1.sync = null; //SCELTO
                    }
                    progSelez.Add(c.prog1);
                }
                if (c.scelto2)
                {
                    if (c.prog1 == null)
                    {
                        c.prog2.sync = false; // NUOVO
                    }
                    else
                    {
                        c.prog2.sync = null; // SCELTO
                    }
                    progSelez.Add(c.prog2);
                }
            }
            return progSelez;
        }

        /// <summary>
        /// Metodo che aggiunge il progetto passato come parametro alla lista di progetti
        /// Se il progetto ha indice minore dell'ultimo nella lista viene inserito nella corretta posizione
        /// Altrimenti aggiunto alla fine della lista
        /// Se manca crea cartella di progetto e aggiorna o crea il file docx
        /// </summary>
        private void addProject(Progetto prog)
        {
            prog.sync = false;
            int numCliente = 0;
            int i = 0;
            foreach (Cliente c in Globals.CLIENTI)
            {
                if (c.getNomeCliente().Equals(prog.nomeCliente))
                {
                    numCliente = i;
                }
                i++;
            }
            if (prog.numero <= progetti.Last().numero)
            {
                int index = -1;
                for(int j = prog.numero; j>0 && index==-1; j--)
                {
                    index = progetti.FindIndex(r => r.numero == j);
                }
                progetti.Insert(index+1, prog);
            }
            else
            {
                progetti.Add(prog);
            }
            Globals.CLIENTI[numCliente].setMaxId(progetti.Count);
            Globals.CLIENTI[numCliente].setLastId(prog.numero);
            string msg = "Nuovo progetto con indice: " + prog.numero;
            Console.WriteLine(msg);
            Globals.log.Info(msg);
            try
            {
                Directory.CreateDirectory(Globals.PROGETTI + prog.nomeCliente + @"\" + prog.nomeCliente + prog.numero);
            }
            catch (IOException)
            {
                string msg2 = "E55 - La cartella " + Globals.PROGETTI + prog.nomeCliente + @"\" + prog.nomeCliente + prog.numero + " non è stata creata per un problema";
                System.Windows.MessageBox.Show(msg2, "E55", System.Windows.MessageBoxButton.OK,
                   System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg2);
            }
            string fileName = Globals.PROGETTI + prog.nomeCliente + @"\" + prog.nomeCliente + prog.numero + @"\progetto.docx";
            try
            {
                if (!File.Exists(fileName))
                {
                    var doc = Xceed.Words.NET.DocX.Create(fileName);
                    doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                    doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
                else
                {
                    var doc = Xceed.Words.NET.DocX.Load(fileName);
                    doc.InsertParagraph("Aggiornamento del " + prog.modifica).Bold();
                    doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                    doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
            }
            catch (IOException)
            {
                string msg2 = "E56 - Il file " + fileName + " non è stato creato per un problema";
                System.Windows.MessageBox.Show(msg2, "E56", System.Windows.MessageBoxButton.OK,
                   System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg2);
            }
            Globals.log.Info("Aggiunto progetto"+ prog.numero + ") " + prog.nome);
        }

        /// <summary>
        /// Metodo che modifica il progetto passato come parametro.
        /// Aggiorna o crea il file docx. 
        /// </summary>
        private void modifyProject(Progetto prog)
        {
            prog.sync = false;
            progetti[progetti.FindIndex(r => r.numero == prog.numero)] = prog;
            string msg = "Aggiornato il progetto con indice " + prog.numero;
            Console.WriteLine(msg);
            Globals.log.Info(msg);
            string fileName = Globals.PROGETTI + prog.nomeCliente + @"\" + prog.suffisso + prog.numero + @"\progetto.docx";
            try
            {
                if (!File.Exists(fileName))
                {
                    var doc = Xceed.Words.NET.DocX.Create(fileName);
                    doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                    doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
                else
                {
                    var doc = Xceed.Words.NET.DocX.Load(fileName);
                    doc.InsertParagraph("Aggiornamento del " + prog.modifica).Bold();
                    doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                    doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                    doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                    doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                    doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                    doc.InsertParagraph("\n Note:");
                    doc.Save();
                }
            }
            catch (IOException)
            {
                string msg2 = "E54 - Il file " + fileName + " non è stato modificato per un problema";
                System.Windows.MessageBox.Show(msg2, "E54", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg2);
            }
            Globals.log.Info("Modificato progetto" + prog.numero + ") " + prog.nome);
        }

        /// <summary>
        /// Metodo per la riscrittura di progetti nel file *CLIENTE*.csv
        /// </summary>
        private bool scriviCSV()
        {
            List<string> lines = new List<string>();
            int i = 0;
            foreach (Progetto p in progetti)
            {
                lines.Add(p.numero + Environment.NewLine + p.nome + Environment.NewLine + p.tipoPLC + Environment.NewLine + p.tipoOP +
                    Environment.NewLine + p.data);
                i++;
            }
            try
            {
                File.WriteAllLines(Globals.DATI + progetti[0].nomeCliente + ".csv", lines);
            }
            catch (IOException)
            {
                string msg = "E51 - errore nella scrittura del file";
                System.Windows.MessageBox.Show(msg, "E51", System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            return true;
        }
    }
}
