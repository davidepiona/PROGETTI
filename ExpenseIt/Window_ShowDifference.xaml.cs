using System;
using System.Collections.Generic;
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
        public Window_ShowDifference(List<Confronto> list)
        {
            this.list = list;
            InitializeComponent();
            Console.WriteLine("Create List");
            Globals.log.Info("Create List");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.Items.Clear();
            int i = 0;
            foreach (Confronto c in list)
            {
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
    }
}
