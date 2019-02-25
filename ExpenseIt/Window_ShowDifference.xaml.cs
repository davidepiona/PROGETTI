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
                //if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId()))
                //{
                //    dataGrid.SelectedIndex = i;
                //    dataGrid.ScrollIntoView(progetti[i]);
                //}
                i++;
            }
        }
        /// <summary>
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

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

        private void BottModifiche_Click(object sender, RoutedEventArgs e)
        {
            List<Progetto> progSelez = new List<Progetto>();
            List<string> tipoSelez = new List<string>();
            foreach (Confronto c in list)
            {
                if (c.scelto1 && c.scelto2)
                {
                    string msg = "E53 - Errore, non è possibile lo stato con entrambi i progSelez selezionati";
                    MessageBox.Show(msg, "E53"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                    return;
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
            foreach (Progetto prog in progSelez)
            {
                if (prog.sync == false) //NUOVO
                {
                    Console.WriteLine(prog.numero + ") " + prog.nome + " " + prog.nomeCliente + " " + prog.data);
                    //prog.modifica = (DateTime.Now).ToString();
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
                        progetti.Insert(prog.numero-1, prog);
                        Globals.CLIENTI[numCliente].setLastId(prog.numero);
                    }
                    else
                    {
                        progetti.Add(prog);
                        Globals.CLIENTI[numCliente].setMaxId(prog.numero);
                        Globals.CLIENTI[numCliente].setLastId(prog.numero);
                    }
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
                        var doc = Xceed.Words.NET.DocX.Create(fileName);
                        doc.InsertParagraph(prog.nomeCliente + " " + prog.numero).Bold();
                        doc.InsertParagraph("\n TITOLO DEL PROGETTO: " + prog.nome);
                        doc.InsertParagraph("\n TIPO DI PLC: " + prog.tipoPLC);
                        doc.InsertParagraph("\n TIPO DI OP: " + prog.tipoOP);
                        doc.InsertParagraph("\n DATA INIZIO: " + prog.data);
                        doc.InsertParagraph("\n Note:");
                        doc.Save();
                    }
                    catch (IOException)
                    {
                        string msg2 = "E56 - Il file " + fileName + " non è stato creato per un problema";
                        System.Windows.MessageBox.Show(msg2, "E56", System.Windows.MessageBoxButton.OK,
                           System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg2);
                    }
                    MainWindow m = new MainWindow();
                    m.salvaClientiCSV();
                    Globals.log.Info("Aggiunto progetto");
                }
                if (prog.sync == null) //SCELTO
                {
                    Console.WriteLine(prog.numero + ") " + prog.nome + " " + prog.nomeCliente + " " + prog.data);
                    //prog.modifica = (DateTime.Now).ToString();
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
                }
            }

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
