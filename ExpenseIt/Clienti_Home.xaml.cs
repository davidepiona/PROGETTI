﻿using System;
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
    /// Logica di interazione per ExpenseItViewExpense.xaml
    /// </summary>
    public partial class Clienti_Home : Page
    {
        private List<Cliente> clienti = new List<Cliente>();
        private string lastOpen;
        public Clienti_Home()
        {
            InitializeComponent();
            var file = File.OpenRead(Globals.DATI + @"\CLIENTI.csv");
            var reader = new StreamReader(file); string info = reader.ReadLine();
            lastOpen = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                if (line.Length == 4)
                {
                    clienti.Add(new Cliente(line[0], line[1], Int32.Parse(line[2]), Int32.Parse(line[3])));
                }

            }
            file.Close();
            int i = clienti.Count;
            Button[] buttonArray = new Button[i];
            Grid grid = this.FindName("grid") as Grid;
            int j = clienti.Count;
            i = 0;
            foreach (Cliente cliente in clienti)
            {
                buttonArray[i] = new Button();
                buttonArray[i].Width = 140;
                buttonArray[i].Height = 50;
                buttonArray[i].Name = "button" + i;
                buttonArray[i].Content = cliente.getNomeCliente();
                buttonArray[i].Click += new RoutedEventHandler(button_Click);
                buttonArray[i].Background = Brushes.White;
                buttonArray[i].Foreground = Brushes.Black;

                Grid.SetColumn(buttonArray[i], i % 4 +1 );
                Grid.SetRow(buttonArray[i], 1 + (i / 4));
                grid.Children.Add(buttonArray[i]);
                Console.WriteLine("\t" + cliente.getNomeCliente());
                i++;
            }
            InitializeComponent();

        }

        private void button_Click(object sender, EventArgs e)
        {
            // CAMBIARE PAGINA
            string clienteAttuale = ((Button)sender).Content.ToString();
            int n = clienti.FindIndex(x => x.getNomeCliente().Equals(clienteAttuale));
            // se la cartella o il file csv di quel clienteAttualee non esiste
            if (!System.IO.Directory.Exists(Globals.PROGETTI + clienteAttuale) || !System.IO.File.Exists(Globals.DATI + clienteAttuale + ".csv"))
            {
                MessageBox.Show("La cartella o il file csv del clienteAttualee " + clienteAttuale + " non è presente");
            }
            
            else
            {
                Progetti_Home progetti_home = new Progetti_Home(clienti[n]);
                this.NavigationService.Navigate(progetti_home);
            }
        }
    }
}


//string[] lines = System.IO.File.ReadAllLines(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI.txt");
//// Display the file contents by using a foreach loop.
//System.Console.WriteLine("Contents of WriteLines2.txt = ");
//            Button[] buttonArray = new Button[lines.Length];
//int i = 0;
//Grid grid = this.FindName("grid") as Grid;

//            foreach (string line in lines)
//            {
//                // Use a tab to indent each line of the file.
                
//                buttonArray[i] = new Button();
//buttonArray[i].Width = 125;
//                buttonArray[i].Height = 30;
//                buttonArray[i].Name = "button" + i;
//                buttonArray[i].Content = line;
//                buttonArray[i].Click += new RoutedEventHandler(button_Click);
//Grid.SetColumn(buttonArray[i], i%4);
//                Grid.SetRow(buttonArray[i], 1+ (i/4));
//                grid.Children.Add(buttonArray[i]);
//                Console.WriteLine("\t" + line);
//                i++;
//            }