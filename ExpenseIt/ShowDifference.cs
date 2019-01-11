using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public ShowDifference(List<Progetto> progetti)
        {   
        
            InitializeComponent();
            this.Size = new Size(1650, 1050);
            this.progetti = progetti;   
            createDataGridView();
            populateDataGrid();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void createDataGridView()
        {
            dataGridView1.ColumnCount = 4;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            //dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe", 13);
            //new Font(, FontStyle.Bold);

            dataGridView1.Name = "dataGridView1";
            dataGridView1.Location = new Point(8, 8);
            dataGridView1.Size = new Size(1600, 1000);
            //dataGridView1.AutoSizeRowsMode =  DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            //dataGridView1.GridColor = Color.Black;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns[0].Name = "Id";
            dataGridView1.Columns[1].Name = "Nome";
            dataGridView1.Columns[2].Name = "Creazione";
            dataGridView1.Columns[3].Name = "Modifica";
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Segoe", 12);
            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Segoe", 12);
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Segoe", 10);
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Segoe", 10);
            dataGridView1.Columns[0].Width = 210;
            dataGridView1.Columns[1].Width = 640;
            dataGridView1.Columns[2].Width = 244;
            dataGridView1.Columns[3].Width = 244;

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.Dock = DockStyle.Fill;

            
        }

        private void populateDataGrid()
        {
            foreach (Progetto p in progetti)
            {
                dataGridView1.Rows.Add(p.sigla, p.nome, p.data, p.modifica);
            }

        }
    }
}

