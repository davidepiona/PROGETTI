using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ExpenseIt
{
    public partial class Form_percorsi: Form
    {
        public Form_percorsi()
        {
            InitializeComponent();
            textBox1.Text = Globals.PROGETTI;
            textBox2.Text = Globals.DATI;
            textBox3.Text = Globals.DATIsync;
        }

        private void Form_percorsi_Load(object sender, EventArgs e)
        {

        }

        

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }
                textBox1.Text= path;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox2.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }
                textBox2.Text = path;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox3.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }

                textBox3.Text = path;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox1.Text.ToString()))
            {
                pictureBox4.Visible = true;
            }
            else
            {
                pictureBox4.Visible = false;
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox2.Text.ToString()))
            {
                pictureBox5.Visible = true;
            }
            else
            {
                pictureBox5.Visible = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox3.Text.ToString()))
            {
                pictureBox6.Visible = true;
            }
            else
            {
                pictureBox6.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tb1 = textBox1.Text.ToString();
            string tb2 = textBox2.Text.ToString();
            string tb3 = textBox3.Text.ToString();

            if (!Directory.Exists(tb1) || !Directory.Exists(tb2) || !Directory.Exists(tb3)){
                System.Windows.MessageBoxResult me = System.Windows.MessageBox.Show(
                  "Uno dei percorsi inseriti non esiste, sei sicuro di voler continuare e salvare le modifiche?",
                  "Percorso inesistente",
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Question);
                if (me == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (!tb1.Equals(Globals.PROGETTI) || !tb2.Equals(Globals.DATI) || !tb3.Equals(Globals.DATIsync))
            {
                Globals.PROGETTI = tb1;
                Globals.DATI = tb2;
                Globals.DATIsync = tb3;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
