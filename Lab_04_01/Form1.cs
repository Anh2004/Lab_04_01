using Lab_04_01.Models;
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
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;


namespace Lab_04_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Faculty> listFalcultys = context.Faculties.ToList(); //lấy các khoa
                List<Student> listStudent = context.Students.ToList(); //lấy sinh viên
                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Hàm binding list có tên hiện thị là tên khoa, giá trị là Mã khoa
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.comboBox1.DataSource = listFalcultys;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }
        //Hàm binding gridView từ list sinh viên
        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = item.FullName;
                dataGridView1.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (textBox1.Text.Length != 5)
            {
                MessageBox.Show("Mã số sinh viên phải có 5 ký tự!");
                return;
            }

            float averageScore;
            if (!float.TryParse(textBox3.Text, out averageScore))
            {
                MessageBox.Show("Điểm trung bình phải là số!");
                return;
            }

            using (var context = new StudentContextDB())
            {
                var query = "INSERT INTO Student (StudentID, FullName, AverageScore, FacultyID) " +
                            "VALUES (@p0, @p1, @p2, @p3)";
                context.Database.ExecuteSqlCommand(query, textBox1.Text, textBox2.Text, averageScore, comboBox1.SelectedValue);

                MessageBox.Show("Thêm mới dữ liệu thành công!");
                Form1_Load(sender, e); // Tải lại dữ liệu sau khi thêm
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            float averageScore;
            if (!float.TryParse(textBox3.Text, out averageScore))
            {
                MessageBox.Show("Điểm trung bình phải là số!");
                return;
            }

            using (var context = new StudentContextDB())
            {
                var query = "UPDATE Student SET FullName = @p0, AverageScore = @p1, FacultyID = @p2 WHERE StudentID = @p3";
                int rowsAffected = context.Database.ExecuteSqlCommand(query, textBox2.Text, averageScore, comboBox1.SelectedValue, textBox1.Text);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật dữ liệu thành công!");
                    Form1_Load(sender, e); // Tải lại dữ liệu sau khi sửa
                }
                else
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                var query = "DELETE FROM Student WHERE StudentID = @p0";
                int rowsAffected = context.Database.ExecuteSqlCommand(query, textBox1.Text);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa sinh viên thành công!");
                    Form1_Load(sender, e); // Tải lại dữ liệu sau khi xóa
                }
                else
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                }
            }
        }

        private void LoadStudentData()
        {
            using (var context = new StudentContextDB())
            {
                var students = context.Students
                                      .Include(s => s.Faculty) // Load cả thông tin Faculty liên kết
                                      .Select(s => new
                                      {
                                          s.StudentID,
                                          s.FullName,
                                          s.AverageScore,
                                          FacultyName = s.Faculty.FacultyName // Hiển thị tên khoa
                                      })
                                      .ToList();
                dataGridView1.DataSource = students;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

