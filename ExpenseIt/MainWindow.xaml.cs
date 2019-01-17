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
            ShowsNavigationUI = false;
            if (Globals.CLIENTI == null)
            {
                leggiSETTINGS();
            }
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Closing called");
            salvaClientiCSV();
            // If data is dirty, notify user and ask for a response
            //if (Globals.IS_DATA_DIRTY)
            //{
            //    Console.WriteLine("Data is dirty");

            //}
        }

        public void salvaClientiCSV()
        {
            //Console.WriteLine("COsa è?? " + Globals.LAST_CLIENT);
            string[] lines = new string[2 + Globals.CLIENTI.Count];
            lines[0] = "CLIENTE,SUFFISSO,LAST_ID,MAX_ID";
            lines[1] = Globals.LAST_CLIENT;
            Cliente cl = Globals.CLIENTI.Find(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT));
            lines[2] = cl.getNomeCliente() + "," + cl.getNomeCliente() + "," + cl.getlastId() + "," + cl.getMaxId();
            int i = 3;
            foreach (Cliente c in Globals.CLIENTI)
            {
                if (!c.getNomeCliente().Equals(Globals.LAST_CLIENT))
                {
                    lines[i] = c.getNomeCliente() + "," + c.getNomeCliente() + "," + c.getlastId() + "," + c.getMaxId();
                    i++;
                }
                //Console.WriteLine(c.getNomeCliente() + "," + c.getNomeCliente() + "," + c.getlastId() + "," + c.getMaxId());
            }
            try
            {
                File.WriteAllLines(Globals.DATI + @"\CLIENTI.csv", lines);
                
              
            }
            catch (IOException)
            {
                MessageBox.Show(
               "E08 - Il file " + Globals.DATI + @"\CLIENTI.csv" + " non esiste o è aperto da un altro programma." +
               " \n\nChiudere i programmi che utilizzano questo file e riprovare.",
               "Chiusura applicazione",
               MessageBoxButton.OK,
               MessageBoxImage.Stop);
                salvaClientiCSV();
            }
        }

        public void leggiSETTINGS()
        {
            Console.WriteLine("Leggo SETTINGS");
            try
            {
                var file = File.OpenRead(Directory.GetCurrentDirectory() + @"\SETTINGS.csv");
                var reader = new StreamReader(file);
                
                Globals.GITURL = reader.ReadLine();
                Globals.GITPATH = reader.ReadLine();
                Globals.PROGETTI = reader.ReadLine();
                Globals.DATI = reader.ReadLine();
                Globals.DATIsync = reader.ReadLine();
                Globals.ANTEPRIME = reader.ReadLine().Equals("True") ? true : false;
                Globals.SINCRONIZZAZIONE = reader.ReadLine().Equals("True") ? true : false;
                //Console.WriteLine("<" + reader.ReadLine() + "><" + reader.ReadLine() + ">");
                file.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("E11 - non è stato possibile aprire il file " + Directory.GetCurrentDirectory() + @"\SETTINGS.csv" +
                    " quindi sono state caricate le impostazioni di default");
            }
        }

        public void scriviSETTINGS()
        {
            Console.WriteLine("Scrivo SETTINGS");

            string[] lines = new string[7];
            lines[0] = Globals.GITURL;
            lines[1] = Globals.GITPATH;
            lines[2] = Globals.PROGETTI;
            lines[3] = Globals.DATI;
            lines[4] = Globals.DATIsync;
            lines[5] = Globals.ANTEPRIME.ToString();
            lines[6] = Globals.SINCRONIZZAZIONE.ToString();
            try
            {
                File.WriteAllLines(Directory.GetCurrentDirectory() + @"\SETTINGS.csv", lines);
            }
            catch (IOException)
            {
                MessageBox.Show(
               "E14 - Il file " + Directory.GetCurrentDirectory() + @"\SETTINGS.csv" + " non esiste o è aperto da un altro programma." +
               " \n\nNon è possibile salvare le nuove preferenze.",
               "File bloccato",
               MessageBoxButton.OK,
               MessageBoxImage.Information);
            }
        }
    }
    public static class Globals
    {
        //public const Int32 BUFFER_SIZE = 512; // Unmodifiable
        //public static Boolean IS_DATA_DIRTY = false; // Modifiable
        public static String LAST_CLIENT; // Modifiable
        public static List<Cliente> CLIENTI;

        public static String GITURL = "https://github.com/davidepiona/DATIsync.git";
        public static String GITPATH = @"C:\Program Files\Git\cmd\git.exe";
        public static String PROGETTI = @"R:\PROGETTI\";
        public static String DATI = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\";
        public static String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync\";
        public static bool ANTEPRIME = true;
        public static bool SINCRONIZZAZIONE = true;
        public static bool changed = false;
        //public static readonly String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync";// Unmodifiable
    }




}
