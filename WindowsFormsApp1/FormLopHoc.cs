using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormLopHoc : Form
    {
        DatabaseDataContext db = new DatabaseDataContext();
        public FormLopHoc()
        {
            InitializeComponent();
        }
        private void FormLopHoc_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDanhSachKhoa();
                LoadDanhSachLop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Chi tiết lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //DSKhoa
        private void LoadDanhSachKhoa()
        {
            var dsKhoa = db.Khoas.ToList();
            cbKhoa.DataSource = dsKhoa;
            cbKhoa.DisplayMember = "TenKhoa";
            cbKhoa.ValueMember = "MaKhoa";
        }
        //DSLop
        private void LoadDanhSachLop(string tuKhoa = "")
        {
            var dsLop = (from l in db.Lops
                         join k in db.Khoas on l.MaKhoa equals k.MaKhoa
                         select new
                         {
                             l.MaLop,
                             l.TenLop,
                             l.NamBatDau,
                             TenKhoa=k.TenKhoa,
                             l.MaKhoa
                         }).ToList();
            if (!string .IsNullOrEmpty(tuKhoa) )
            {
                string kw = tuKhoa.ToLower(); //chuyen thanh chu thuong
                dsLop = dsLop.Where(l =>
                    l.TenLop.ToLower().Contains(kw)|| //ten lop co chua tu khoa
                    l.MaLop.ToLower().Contains(kw) //ma lop co chua tu khoa
                    ).ToList(); // tolower() de tim ko phan biet chu hoa/thuong
            }
            dgvLop.DataSource = dsLop;
            dgvLop.Columns["MaKhoa"].Visible = false;
        }
        //dgvLop
        private void dgvLop_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvLop.Rows[e.RowIndex];
            txtMaLop.Text = row.Cells["MaLop"].Value?.ToString();
            txtTenLop.Text = row.Cells["TenLop"].Value?.ToString();
            txtNamBatDau.Text = row.Cells["NamBatDau"].Value?.ToString();
            
            //set khoa
            string maKhoa = row.Cells["MaKhoa"].Value?.ToString();
            if (maKhoa != null)
                cbKhoa.SelectedValue = maKhoa ;
            
            //loadSV
            LoadSinhVienTheoLop(txtMaLop.Text);
        }

        private void LoadSinhVienTheoLop(string maLop)
        {
            var dsSV = (from sv in db.SinhViens
                        where sv.MaLop == maLop
                        select new
                        {
                            sv.MaSV,
                            sv.HoTen,
                            sv.NgaySinh,
                            sv.GioiTinh,
                            sv.Email,
                            sv.SoDienThoai
                        }).ToList();

            dgvSinhVien.DataSource = dsSV;
        }

        // ── THÊM lớp ─────────────────────────────────────────────────────
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLop.Text) ||
                string.IsNullOrEmpty(txtTenLop.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Lớp và Tên Lớp!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var kiemTra = db.Lops.FirstOrDefault(l => l.MaLop == txtMaLop.Text);
            if (kiemTra != null)
            {
                MessageBox.Show("Mã Lớp đã tồn tại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var lopMoi = new Lop
            {
                MaLop = txtMaLop.Text.Trim(),
                TenLop = txtTenLop.Text.Trim(),
                NamBatDau = string.IsNullOrEmpty(txtNamBatDau.Text)
                                ? (int?)null
                                : int.Parse(txtNamBatDau.Text),
                MaKhoa = cbKhoa.SelectedValue?.ToString()
            };

            db.Lops.InsertOnSubmit(lopMoi);
            db.SubmitChanges();

            MessageBox.Show("Thêm lớp thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        // ── SỬA lớp ──────────────────────────────────────────────────────
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLop.Text))
            {
                MessageBox.Show("Vui lòng chọn lớp cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lop = db.Lops.FirstOrDefault(l => l.MaLop == txtMaLop.Text);
            if (lop == null)
            {
                MessageBox.Show("Không tìm thấy lớp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lop.TenLop = txtTenLop.Text.Trim();
            lop.NamBatDau = string.IsNullOrEmpty(txtNamBatDau.Text)
                                ? (int?)null
                                : int.Parse(txtNamBatDau.Text);
            lop.MaKhoa = cbKhoa.SelectedValue?.ToString();

            db.SubmitChanges();

            MessageBox.Show("Cập nhật lớp thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        // ── XÓA lớp ──────────────────────────────────────────────────────
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLop.Text))
            {
                MessageBox.Show("Vui lòng chọn lớp cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra lớp có sinh viên không
            var coSV = db.SinhViens.Any(sv => sv.MaLop == txtMaLop.Text);
            if (coSV)
            {
                MessageBox.Show("Không thể xóa! Lớp này vẫn còn sinh viên.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa lớp {txtTenLop.Text}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;

            var lop = db.Lops.FirstOrDefault(l => l.MaLop == txtMaLop.Text);
            if (lop == null) return;

            db.Lops.DeleteOnSubmit(lop);
            db.SubmitChanges();

            MessageBox.Show("Xóa lớp thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        // ── TÌM KIẾM ─────────────────────────────────────────────────────
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadDanhSachLop(txtTimKiem.Text.Trim());
        }

        private void txtTimKiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                btnTimKiem_Click(sender, e);
        }

        // ── LÀM MỚI ──────────────────────────────────────────────────────
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void LamMoi()
        {
            txtMaLop.Clear();
            txtTenLop.Clear();
            txtNamBatDau.Clear();
            txtTimKiem.Clear();
            dgvSinhVien.DataSource = null;

            db = new DatabaseDataContext();

            LoadDanhSachLop();
            txtMaLop.Focus();
        }
        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dgvLop_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
