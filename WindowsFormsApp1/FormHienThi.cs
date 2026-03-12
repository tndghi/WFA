using System;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class panelMain : Form
    {
        DatabaseDataContext db = new DatabaseDataContext();

        public panelMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDanhSachLop();
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void LoadDanhSachLop()
        {
            var dsLop = db.Lops.ToList();
            cbLop.DataSource = dsLop;
            cbLop.DisplayMember = "TenLop";
            cbLop.ValueMember = "MaLop";
        }

        private void LoadDanhSachSinhVien(string maLop = "")
        {
            var query = from sv in db.SinhViens
                        join l in db.Lops on sv.MaLop equals l.MaLop
                        select new
                        {
                            sv.MaSV,
                            sv.HoTen,
                            sv.NgaySinh,
                            sv.GioiTinh,
                            sv.Email,
                            sv.SoDienThoai,
                            TenLop = l.TenLop,
                            sv.MaLop
                        };

            if (!string.IsNullOrEmpty(maLop))
                query = query.Where(sv => sv.MaLop == maLop);

            dgvSinhVien.DataSource = query.ToList();
            dgvSinhVien.Columns["MaLop"].Visible = false;
        }

        private void cbLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLop.SelectedValue == null) return;
            LoadDanhSachSinhVien(cbLop.SelectedValue.ToString());
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cell = dgvSinhVien.Rows[e.RowIndex].Cells["MaSV"].Value;
            if (cell == null) return;

            string maSV = cell.ToString();

            var diem = from ds in db.DiemSos
                       join lh in db.LichHocs on ds.MaLich equals lh.MaLich
                       join mh in db.MonHocs on lh.MaMon equals mh.MaMon
                       where ds.MaSV == maSV
                       select new
                       {
                           mh.TenMon,
                           mh.SoTinChi,
                           ds.DiemCC,
                           ds.DiemGK,
                           ds.DiemCK,
                           ds.DiemTB
                       };

            dgvDIem.DataSource = diem.ToList();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadDanhSachSinhVien();
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnLoad_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void panelSidebar_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void panelHeader_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
    }
}