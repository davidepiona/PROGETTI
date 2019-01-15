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

namespace ExpenseIt
{
    public partial class ShowDifference : Form
    {
        private System.Windows.Forms.DataGridView dataGridView1;
        private List<Progetto> progetti;
        private int num_cliente;
        DataGridViewCheckBoxColumn col;
        public ShowDifference(List<Progetto> progetti, int num_cliente)
        {   
        
            InitializeComponent();
            this.Size = new Size(1000, 800);
            this.progetti = progetti;
            this.num_cliente = num_cliente;
            createDataGridView();
            
                populateDataGrid();
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void createDataGridView()
        {
            dataGridView1.ColumnCount = 4;
            dataGridView1.Left = 100;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            //dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe", 13);
            //new Font(, FontStyle.Bold);

            dataGridView1.Name = "dataGridView1";
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            //dataGridView1.GridColor = Color.Black;
            dataGridView1.RowHeadersVisible = false;

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
            col = new DataGridViewCheckBoxColumn();
            col.Name = "Aggiungere?";
            col.Width = 100;
            dataGridView1.Columns.Add(col);

            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Segoe", 10);
            dataGridView1.Columns[0].Width = 120;
            dataGridView1.Columns[1].Width = 371;
            dataGridView1.Columns[2].Width = 145;
            dataGridView1.Columns[3].Width = 145;

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.Dock = DockStyle.Fill;


        }

        private void populateDataGrid()
        {
            foreach (Progetto p in progetti)
            {
                p.sync = true;
                dataGridView1.Rows.Add(p.sigla, p.nome, p.data, p.tipoPLC, true);
            }

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (progetti.Count != 0)
            {
                string file = Globals.DATI + progetti[0].cliente + ".csv";
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


                    Console.WriteLine("INDICE: " + i);
                    Globals.CLIENTI[num_cliente].setMaxId(Globals.CLIENTI[num_cliente].getMaxId() + i);
                    Globals.CLIENTI[num_cliente].setLastId(Globals.CLIENTI[num_cliente].getMaxId());
                    Console.WriteLine("MAX: " + Globals.CLIENTI[num_cliente].getMaxId());
                    MainWindow m = new MainWindow();
                    m.salvaClientiCSV();


                }
                catch (IOException)
                {
                    MessageBox.Show("E10 - Il file " + file + " non esiste o è aperto da un altro programma");
                }
            }
            this.Close();
            
        }

        private void ShowDifference_Load(object sender, EventArgs e)
        {

        }
    }
}

