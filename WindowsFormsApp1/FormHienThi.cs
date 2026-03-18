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
            if (db == null) 
            { 
                db = new DatabaseDataContext();
            }
            cbGioiTinh.Items.Clear();
            cbGioiTinh.Items.Add("Nam");
            cbGioiTinh.Items.Add("Nữ");
            cbGioiTinh.SelectedIndex = 0;
            try
            {
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void LoadDanhSachSinhVien(string maLop = "", string tuKhoa = "")
        {
            var dsSV = (from sv in db.SinhViens
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
                        }).ToList(); 

            if (!string.IsNullOrEmpty(maLop))
                dsSV = dsSV.Where(sv => sv.MaLop == maLop).ToList();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                string kw = tuKhoa.ToLower();
                dsSV = dsSV.Where(sv =>
                    sv.HoTen.ToLower().Contains(kw) ||
                    sv.MaSV.ToLower().Contains(kw)).ToList();
            }

            dgvSinhVien.DataSource = dsSV;
            dgvSinhVien.Columns["MaLop"].Visible = false;
        }
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvSinhVien.Rows[e.RowIndex];
            txtMaSV.Text = row.Cells["MaSV"].Value?.ToString();
            txtHoTen.Text = row.Cells["HoTen"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtSDT.Text = row.Cells["SoDienThoai"].Value?.ToString();
            txtLopQL.Text = row.Cells["TenLop"].Value?.ToString();

            if (row.Cells["NgaySinh"].Value != null)
                dTPNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);

            cbGioiTinh.SelectedItem = row.Cells["GioiTinh"].Value?.ToString();

            string maSV = row.Cells["MaSV"].Value?.ToString();
            if (maSV == null) return;

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

            dgvDiem.DataSource = diem.ToList();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text) ||
                string.IsNullOrEmpty(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã SV và Họ Tên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var kiemTra = db.SinhViens.FirstOrDefault(sv => sv.MaSV == txtMaSV.Text);
            if (kiemTra != null)
            {
                MessageBox.Show("Mã SV đã tồn tại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var svMoi = new SinhVien
            {
                MaSV = txtMaSV.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dTPNgaySinh.Value.Date,
                GioiTinh = cbGioiTinh.SelectedItem?.ToString(),
                Email = txtEmail.Text.Trim(),
                SoDienThoai = txtSDT.Text.Trim(),
            };

            db.SinhViens.InsertOnSubmit(svMoi);
            db.SubmitChanges();

            MessageBox.Show("Thêm sinh viên thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sv = db.SinhViens.FirstOrDefault(s => s.MaSV == txtMaSV.Text);
            if (sv == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sv.HoTen = txtHoTen.Text.Trim();
            sv.NgaySinh = dTPNgaySinh.Value.Date;
            sv.GioiTinh = cbGioiTinh.SelectedItem?.ToString();
            sv.Email = txtEmail.Text.Trim();
            sv.SoDienThoai = txtSDT.Text.Trim();

            db.SubmitChanges();

            MessageBox.Show("Cập nhật thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa sinh viên {txtHoTen.Text}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;

            var sv = db.SinhViens.FirstOrDefault(s => s.MaSV == txtMaSV.Text);
            if (sv == null) return;

            var diem = db.DiemSos.Where(d => d.MaSV == sv.MaSV);
            db.DiemSos.DeleteAllOnSubmit(diem);

            db.SinhViens.DeleteOnSubmit(sv);
            db.SubmitChanges();

            MessageBox.Show("Xóa thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LamMoi();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadDanhSachSinhVien(tuKhoa: txtTimKiem.Text.Trim());
        }

        private void txtTimKiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                btnTimKiem_Click(sender, e);
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }
        private void LamMoi()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtEmail.Clear();
            txtSDT.Clear();
            txtTimKiem.Clear();
            txtLopQL.Clear();
            dTPNgaySinh.Value = DateTime.Today;
            cbGioiTinh.SelectedIndex = 0;
            dgvDiem.DataSource = null;
            LoadDanhSachSinhVien();
            txtMaSV.Focus();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadDanhSachSinhVien();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnLoad_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void panelSidebar_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void panelHeader_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormLopHoc().Show();
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnQLLop_Click(object sender, EventArgs e)
        {
            new FormLopHoc().Show();
        }
    }
}