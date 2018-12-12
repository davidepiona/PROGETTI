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
using System.Collections;

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
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);

            ExpenseItViewExpense expenseItViewExpense = new ExpenseItViewExpense();
            this.NavigationService.Navigate(expenseItViewExpense);
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            string path = "C:/Users/attil/source/repos/ExpenseIt/ExpenseIt";
            System.Diagnostics.Process.Start(path);
        }
    }
}
