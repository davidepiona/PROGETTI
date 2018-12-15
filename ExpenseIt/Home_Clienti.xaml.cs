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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpenseIt
{
    /// <summary>
    /// Logica di interazione per ExpenseItViewExpense.xaml
    /// </summary>
    public partial class Home_Clienti : Page
    {
        public Home_Clienti()
        {
            InitializeComponent();
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI.txt");
            // Display the file contents by using a foreach loop.
            System.Console.WriteLine("Contents of WriteLines2.txt = ");
            Button[] buttonArray = new Button[lines.Length];
            int i = 0;
            Grid grid = this.FindName("grid") as Grid;

            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                
                buttonArray[i] = new Button();
                buttonArray[i].Width = 125;
                buttonArray[i].Height = 30;
                buttonArray[i].Name = "button" + i;
                buttonArray[i].Content = line;
                buttonArray[i].Click += new RoutedEventHandler(button_Click);
                Grid.SetColumn(buttonArray[i], i%4);
                Grid.SetRow(buttonArray[i], 1+ (i/4));
                grid.Children.Add(buttonArray[i]);
                Console.WriteLine("\t" + line);
                i++;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            // CAMBIARE PAGINA
            string client = ((Button)sender).Content.ToString();
            Progetti_Home progetti_home = new Progetti_Home(client);
            this.NavigationService.Navigate(progetti_home);
        }
    }
}
