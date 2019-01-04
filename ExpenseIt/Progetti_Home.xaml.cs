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
    /// Logica di interazione per Progetti_Home.xaml
    /// </summary>
    public partial class Progetti_Home : Page
    {
        private Cliente cliente;
        private string lastProject;
        private List<Progetto> progetti = new List<Progetto>();
        private static string PROGETTI = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\";
        public Progetti_Home(Cliente cliente)
        {
            InitializeComponent();
            this.cliente = cliente;
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + cliente.getNomeCliente();

        }

        private void createList()
        {
            Console.WriteLine("\nCreate List");
            List<string> lines = new List<string>();
            int i = 0;
            int j = 0;
            using (var reader = new CsvFileReader(@"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATI\CLIENTI\"+cliente.getNomeCliente()+".csv"))
            {
                i = 0;
                while (reader.ReadRow(lines))
                {
                    int num = Int32.Parse(lines[0]);
                    string numero = string.Format("{0:D3}", num);
                    reader.ReadRow(lines);
                    string nome = lines[0];
                    reader.ReadRow(lines);
                    string tipoPLC = lines[0];
                    reader.ReadRow(lines);
                    string tipoOP = lines[0];
                    reader.ReadRow(lines);
                    string data = lines[0];

                    progetti.Add(new Progetto(numero, nome, tipoOP, tipoOP, data));
                    i++;
                }
                
            }

            //string path = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + cliente.getNomeCliente();
            //string[] fileEntries = Directory.GetFileSystemEntries(path);

            ListView listView = this.FindName("ListView") as ListView;
            listView.SelectionChanged += new SelectionChangedEventHandler(changePreview);
            //foreach (string fileEntry in fileEntries)
                foreach (Progetto p in progetti)
                {
                
                listView.Items.Add(cliente.getNomeCliente()+p.ToName());
                
            }
        }

        private void ListView_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Console.Write(e);
            Console.Write("aaa");
            MessageBox.Show("show");
            //System.Diagnostics.Process.Start(path);
        }

        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\nOpen Folder");
            ListView listView = this.FindName("ListView") as ListView;
            string lastProject = (listView.SelectedValue.ToString()).Split(null)[0];
            string path = PROGETTI + cliente.getNomeCliente() + @"\" + lastProject;
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void New_Project(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\nNew Project");
            Form1 testDialog = new Form1(cliente);
            testDialog.ShowDialog();
            //Console.WriteLine("\nNew Project");
            //FormNuovoCliente testDialog = new FormNuovoCliente();
            //testDialog.ShowDialog();
        }

        private void changePreview(object sender, EventArgs e)
        {
            Console.WriteLine("\nChange Preview");
            lastProject = (((ListView)sender).SelectedValue.ToString()).Split(null)[0];
            try
            {   
                var doc = Xceed.Words.NET.DocX.Load(PROGETTI+cliente.getNomeCliente()+@"\"+lastProject+ @"\progetto.docx");
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                richTextBox.Document.Blocks.Clear();
                richTextBox.AppendText(doc.Text);
            }
            catch(FileNotFoundException)
            {
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                richTextBox.Document.Blocks.Clear();
            }
            catch (IOException)
            {
                
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
            }         

            //// Insert a paragrpah:
            //doc.InsertParagraph("This is my first paragraph");

            //// Save to the output directory:
            //doc.Save();

            //Console.WriteLine(doc.Text);

            
        }

    }
}
