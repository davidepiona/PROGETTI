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
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public static class Globals
    {
        //public const Int32 BUFFER_SIZE = 512; // Unmodifiable
        //public static String FILE_NAME = "Output.txt"; // Modifiable
        public static readonly String PROGETTI = @"R:\PROGETTI\"; //Unmodifiable
        public static readonly String DATI = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\";// Unmodifiable
        public static readonly String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync\";// Unmodifiable
    }
}
