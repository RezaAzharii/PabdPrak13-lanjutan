using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace praktikum13
{
    public partial class printData : Form
    {
        string strKoneksi = "Data Source = LAPTOP-552SL4BV\\REZAAZHARI; Initial Catalog = informasiMhs;" +
          "Integrated Security = True;MultipleActiveResultSets=true";
        CrystalReport2 cr = new CrystalReport2();
        string nmKar = "";

        public printData(string nmaKar)
        {
            InitializeComponent();
            nmKar = nmaKar;
            crystalReportViewer1.ReportSource = null;
        }

        private void NIM_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (NIM.Text.Length == 7)
                {
                    string sql = "SELECT Nama FROM InfoMahasiswa WHERE NIM = @nim";
                    SqlConnection connection = new SqlConnection(strKoneksi);
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@nim", NIM.Text);

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        cbSmst.Enabled = true;
                        if (cbSmst.Items.Count.Equals(0))
                        {
                            cbSmst.Items.Add("GASAL");
                            cbSmst.Items.Add("GENAP");
                        }
                    }
                    else
                    {
                        cbSmst.Items.Clear();
                        cbSmst.Enabled = false;
                        MessageBox.Show("NIM tidak ada", "Kesalahan pada input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Kesalahan Inpu Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
         
        private void cbSmst_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbTA.Enabled = true;
            int tahun = DateTime.Now.Year;
            if (cbTA.Items.Count.Equals(0))
            {
                for (int i = 2010; i <= tahun; i++)
                {
                    string item = Convert.ToString(i) + "/" + Convert.ToString(i + 1);
                    cbTA.Items.Add(item);
                    btnCari.Enabled = true;
                }
            }
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
                string str = "select k.NIM, mhs.Nama as 'NamaMahasiswa',m.kdMK, m.namaMK as 'MataKuliah', k.Semester, k.ThAjar," +
                    "m.sks as 'SKS', k.totalsks as 'TotalSKS' from KRS k " +
                    "join MK m on k.kdMK = k.kdMK join InfoMahasiswa mhs on k.NIM = mhs.nim " +
                    "where k.NIM = '" + NIM.Text + "' and k.Semester = '" + cbSmst.Text + "' and k.ThAjar = '" + cbTA.Text + "'";

                SqlConnection koneksi = new SqlConnection();
                koneksi.ConnectionString = strKoneksi;
                koneksi.Open();
                SqlDataAdapter ad = new SqlDataAdapter(str, koneksi);
                DataTable ds = new DataTable();
                ad.Fill(ds);
                if (ds.Rows.Count > 0)
                {
                    koneksi.Close();
                    cr.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = cr;
                    crystalReportViewer1.Refresh();
                    crystalReportViewer1.AllowedExportFormats = (int)(ViewerExportFormats.PdfFormat);
                }
                else
                {
                    crystalReportViewer1.ReportSource = null;
                    MessageBox.Show("Data tidak ditemukan", "Kesalahan pada input", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void printData_FormClosed(object sender, FormClosedEventArgs e)
        {
            dashboard dashboard = new dashboard(nmKar);
            dashboard.Show();
            this.Hide();
        }
    }
}
