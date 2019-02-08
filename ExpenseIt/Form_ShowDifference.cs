using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATA
{
    /// <summary>
    /// Form che confronta i progetti in DATI e in DATIsync.
    /// - crea una DataGrid con i progetti presenti in DATIsync ma non in DATI
    /// - aggiunge al csv dei progetti in DATI i progetti che l'utente seleziona nella checkbox
    /// </summary>
    public partial class Form_ShowDifference : Form
    {
        private List<Progetto> progetti;
        private int num_cliente;
        DataGridViewCheckBoxColumn col;

        /// <summary>
        /// Costruttore in cui si impostano alcuni attributi e si chiamano i due metodi 
        /// per creare e popolare la DataGrid
        /// </summary>
        public Form_ShowDifference(List<Progetto> progetti, int num_cliente)
        {
            InitializeComponent();
            this.progetti = progetti;
            this.num_cliente = num_cliente;
            createDataGridView();
            populateDataGrid();
        }

        /// <summary>
        /// Metodo per la creazione della DataGrid. Le dimensioni sono proporzionali con la finestra
        /// Vengono create le prime 4 colonne e poi aggiunta quella contenente checkbox
        /// </summary>
        private void createDataGridView()
        {
            dataGridView1.ColumnCount = 4;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe", 13);
            dataGridView1.MaximumSize = new Size(9*this.Width/10, 78 * this.Height / 100);
            dataGridView1.Size = new Size(9 * this.Width / 10, 78 * this.Height / 100);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = true;

            dataGridView1.Columns[0].Name = "Id";
            dataGridView1.Columns[1].Name = "Nome";
            dataGridView1.Columns[2].Name = "Creazione";
            dataGridView1.Columns[3].Name = "TipoPLC";
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Segoe", 12);
            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Segoe", 12);
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Segoe", 10);
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Segoe", 10);
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[0].Width = this.Width / 8;
            dataGridView1.Columns[1].Width = this.Width / 3;
            dataGridView1.Columns[2].Width = this.Width / 6;
            dataGridView1.Columns[3].Width = this.Width / 9;

            col = new DataGridViewCheckBoxColumn();
            col.Name = "Aggiungere?";
            col.Width = this.Width / 8;
            col.DefaultCellStyle.Font = new Font("Segoe", 14);
            dataGridView1.Columns.Add(col);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        /// <summary>
        /// Metodo che popola la DataGrid coi valori presenti nella lista 'progetti'
        /// Il valore delle checkbox viene messo a true di default
        /// </summary>
        private void populateDataGrid()
        {
            foreach (Progetto p in progetti)
            {
                dataGridView1.Rows.Add(p.sigla, p.nome, p.data, p.tipoPLC, true);
            }
        }

        /// <summary>
        /// Aggiunge tutti i progetti con checkbox a true al file .csv e all'elenco di progetti
        /// Aggiorna MaxId e LastId e scrive il file CLIENTI.csv 
        /// Chiude il form
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (progetti.Count != 0)
            {
                string file = Globals.DATI + progetti[0].nomeCliente + ".csv";
                try
                {
                    int i = 0;
                    int index = 0;
                    foreach (Progetto p in progetti)
                    {
                        if (((Boolean)dataGridView1.Rows[index].Cells[4].Value) == true)
                        {
                            string projectDetails = p.numero + Environment.NewLine + p.nome + Environment.NewLine + p.tipoPLC + Environment.NewLine + p.tipoOP + Environment.NewLine + p.data + Environment.NewLine;
                            File.AppendAllText(file, projectDetails);
                            Console.WriteLine("\n ultimo progetto= " + p.sigla);
                            i++;
                        }
                        index++;
                    }
                    Globals.CLIENTI[num_cliente].setMaxId(Globals.CLIENTI[num_cliente].getMaxId() + i);
                    Globals.CLIENTI[num_cliente].setLastId(Globals.CLIENTI[num_cliente].getMaxId());
                    Console.WriteLine("MAX: " + Globals.CLIENTI[num_cliente].getMaxId());
                    MainWindow m = new MainWindow();
                    m.salvaClientiCSV();
                    string msg = "Merge effettuato: sono stati aggiunti " + i + " nuovi progetti";
                    Console.WriteLine(msg);
                    Globals.log.Info(msg);
                }
                catch (IOException)
                {
                    string msg = "E10 - Il file " + file + " non esiste o è aperto da un altro programma";
                    MessageBox.Show(msg, "E10", MessageBoxButtons.OK, MessageBoxIcon.Error, 
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
            }
            this.Close();
        }
    }
}
