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
    /// Logica di interazione per Progetti_Home.xaml
    /// </summary>
    public partial class Progetti_Home : Page
    {
        private string client;

        public Progetti_Home(string client)
        {
            InitializeComponent();
            this.client = client;
            createList();
            Label titolo = this.FindName("titolo") as Label;
            titolo.Content = titolo.Content.ToString() + client;

        }

        private void createList()
        {
            string path = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\PROGETTI\" + client;
            string[] fileEntries = Directory.GetFileSystemEntries(path);
            foreach (string fileEntry in fileEntries)
            {
                ListView listView = this.FindName("ListView") as ListView;
                listView.Items.Add(fileEntry);
                Console.Write(fileEntry);
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
            ListView listView = this.FindName("ListView") as ListView;
            string path = listView.SelectedItem.ToString();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        


        }

        private void New_Project(object sender, RoutedEventArgs e)
        {
            Form1 testDialog = new Form1();
            testDialog.ShowDialog();
        }



    }
}
