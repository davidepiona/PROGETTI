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
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;

namespace ExpenseIt
{
    /// <summary>
    /// Logica di interazione per ExpenseItHome.xaml
    /// </summary>
    public partial class ExpenseItHome : Page
    {
        public ExpenseItHome()
        {
            InitializeComponent();
            string targetDirectory = "C:/Users/attil/source/repos/ExpenseIt/ExpenseIt";
            string[] fileEntries = Directory.GetFileSystemEntries(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // View Expense Report
            ExpenseItViewExpense expenseItViewExpense = new ExpenseItViewExpense();
            this.NavigationService.Navigate(expenseItViewExpense);

        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            string path = "C:/Users/attil/source/repos/ExpenseIt/ExpenseIt";
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory(path);
            }
        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFileSystemEntries(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            //string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            //foreach (string subdirectory in subdirectoryEntries)
            //    ProcessDirectory(subdirectory);

            
            

            // CAMBIARE PAGINA
            //ExpenseItViewExpense expenseItViewExpense = new ExpenseItViewExpense();
            //this.NavigationService.Navigate(expenseItViewExpense);
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string path)
        {
            ListView listView = this.FindName("ListView") as ListView;
            listView.Items.Add(path);
            Console.WriteLine("Processed file '{0}'.", path);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            ListView listView = this.FindName("ListView") as ListView;
            string path = listView.SelectedItem.ToString();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
           
            
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            string fileName = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\read2.docx";

            // Create a document in memory: 
            // var doc = Xceed.Words.NET.DocX.Create(fileName);

            var doc = Xceed.Words.NET.DocX.Load(fileName);

            // Insert a paragrpah:
            doc.InsertParagraph("This is my first paragraph");

            // Save to the output directory:
            doc.Save();

            Console.WriteLine(doc.Text);

            RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            richTextBox.AppendText(doc.Text);

            // Open in Word: 
            // Process.Start("WINWORD.EXE", fileName);
        }

        private void ListView_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Console.Write(e);
            Console.Write("aaa");
            MessageBox.Show("show");
            //System.Diagnostics.Process.Start(path);
        }
    }
}
