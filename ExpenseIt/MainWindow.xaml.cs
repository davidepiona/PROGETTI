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
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Closing called");
            salvaClientiCSV();
            // If data is dirty, notify user and ask for a response
            if (Globals.IS_DATA_DIRTY)
            {
                Console.WriteLine("Data is dirty");
                
            }
        }

        private void salvaClientiCSV()
        {
            string[] lines = new string[2 + Globals.CLIENTI.Count];
            lines[0] = "CLIENTE,SUFFISSO,LAST_ID,MAX_ID";
            lines[1] = Globals.LAST_CLIENT;
            int i = 2;
            foreach (Cliente c in Globals.CLIENTI)
            {
                lines[i] = c.getNomeCliente() + "," + c.getSuffisso() + "," + c.getlastId() + "," + c.getMaxId();
                i++;
            }
            File.WriteAllLines(Globals.DATI + @"\CLIENTI.csv", lines);

        }

        
    }

    public static class Globals
    {
        //public const Int32 BUFFER_SIZE = 512; // Unmodifiable
        public static Boolean IS_DATA_DIRTY = false; // Modifiable
        public static String LAST_CLIENT; // Modifiable
        public static List<Cliente> CLIENTI;
        public static readonly String PROGETTI = @"R:\PROGETTI\"; //Unmodifiable
        public static readonly String DATI = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\";// Unmodifiable
        public static readonly String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync\";// Unmodifiable
    }



   
}
