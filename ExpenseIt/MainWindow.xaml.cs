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
        //public StreamWriter sw;
        public MainWindow()
        {
            //FileStream fs = new FileStream("Test.txt", FileMode.Create);
            //// First, save the standard output.
            //TextWriter tmp = Console.Out;
            //sw = new StreamWriter(fs);
            //Console.SetOut(sw);
            
            ShowsNavigationUI = false;
            if (Globals.CLIENTI == null)
            {
                Globals.SETTINGS = Directory.GetCurrentDirectory() + @"\SETTINGS.csv";
                Console.WriteLine("Settings lette da: "+ Globals.SETTINGS);
                leggiSETTINGS(null,null);
                log4net.GlobalContext.Properties["LogFileName"] = Directory.GetCurrentDirectory() + @"\APP.log"; 
                Globals.log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                Globals.log.Info("Settings lette da: " + Globals.SETTINGS);
                InitializeComponent();
            }
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Closing called");
            Globals.log.Info("Closing called");
            salvaClientiCSV();
        }

        public void salvaClientiCSV()
        {
            string[] lines = new string[2 + Globals.CLIENTI.Count];
            lines[0] = "CLIENTE,SUFFISSO,LAST_ID,MAX_ID";
            lines[1] = Globals.LAST_CLIENT;
            Cliente cl = Globals.CLIENTI.Find(x => x.getNomeCliente().Equals(Globals.LAST_CLIENT));
            lines[2] = cl.getNomeCliente() + "," + cl.getSuffisso() + "," + cl.getlastId() + "," + cl.getMaxId();
            int i = 3;
            foreach (Cliente c in Globals.CLIENTI)
            {
                if (!c.getNomeCliente().Equals(Globals.LAST_CLIENT))
                {
                    lines[i] = c.getNomeCliente() + "," + c.getSuffisso() + "," + c.getlastId() + "," + c.getMaxId();
                    i++;
                }
            }
            try
            {
                File.WriteAllLines(Globals.DATI + @"\CLIENTI.csv", lines);
            }
            catch (IOException)
            {
                string msg = "E08 - Il file " + Globals.DATI + @"\CLIENTI.csv" + " non esiste o è aperto da un altro programma." +
                " \n\nChiudere i programmi che utilizzano questo file e riprovare.";
                MessageBox.Show(msg, "E08 Chiusura applicazione", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                salvaClientiCSV();
            }
        }

        public void leggiSETTINGS(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("Leggo SETTINGS");
            try
            {
                var file = File.OpenRead(Globals.SETTINGS);
                var reader = new StreamReader(file);
                Globals.GITURL = reader.ReadLine();
                Globals.GITPATH = reader.ReadLine();
                Globals.PROGETTI = reader.ReadLine();
                Globals.DATI = reader.ReadLine();
                Globals.DATIsync = reader.ReadLine();
                Globals.ANTEPRIME = reader.ReadLine().Equals("True") ? true : false;
                Globals.SINCRONIZZAZIONE = reader.ReadLine().Equals("True") ? true : false;
                file.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("E11 - non è stato possibile aprire il file " + Globals.SETTINGS +
                    " indicare il percorso ", "E11"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Form_initialSettings form = new Form_initialSettings();
                form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.leggiSETTINGS);
                form.ShowDialog();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("E36 - Il file " + Globals.SETTINGS +
                    " non è nel formato richiesto. \nVerranno caricate alcune impostazioni di base ma la funzionalità non è garantita.", "E36"
                    , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
        
        }

        public void scriviSETTINGS()
        {
            Console.WriteLine("Scrivo SETTINGS");
            Globals.log.Info("Scrivo SETTINGS");
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
                File.WriteAllLines(Globals.SETTINGS, lines);
            }
            catch (IOException)
            {
                string msg = "E14 - Il file " + Globals.SETTINGS + " non esiste o è aperto da un altro programma." +
                " \n\nNon è possibile salvare le nuove preferenze.";
                MessageBox.Show(msg,"E14 File bloccato", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
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
        public static String SETTINGS = @"";
        public static String PROGETTI = @"R:\PROGETTI\";
        public static String DATI = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\";
        public static String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync\";
        public static bool ANTEPRIME = true;
        public static bool SINCRONIZZAZIONE = true;
        //public static readonly String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync";// Unmodifiable

        public static log4net.ILog log; 
    }




}
