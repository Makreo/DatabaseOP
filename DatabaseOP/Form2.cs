using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseOP
{
    public partial class Form2 : Form
    {
        DB DataBase = new DB();

        private int DataIdCustomer;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DataBase.startDb();

            DataBase.state();

            LoadDataCSheet();

            ListBoxCustomerSelect();
            AddListEmployees();
            AddListBoxServices();
        }

        // Получить 
        private void button5_Click(object sender, EventArgs e)
        {
            listBox8.DataSource = null;

            ArrayList SheelDataList = new ArrayList();

            foreach (string[] item in DataBase.GetListCSheet($"SELECT * FROM CalculationSheet WHERE Id = '{listBox7.SelectedValue}'"))
            {
                SheelDataList.Add(new CreateListOP(item[0].ToString(), GetNameСustomer(int.Parse(item[1])) + "\t" + GetEmployees(int.Parse(item[2])) + "\t" + GetServices(int.Parse(item[3]))));
            }

            listBox8.DataSource = SheelDataList;
            listBox8.DisplayMember = "GetName";
            listBox8.ValueMember = "GetId";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox8.DataSource = null;

            ArrayList SheelDataList = new ArrayList();

            foreach (string[] item in DataBase.GetListCSheet("SELECT * FROM CalculationSheet"))
            {
                SheelDataList.Add(new CreateListOP(item[0].ToString(), GetNameСustomer(int.Parse(item[1])) + "\t" + GetEmployees(int.Parse(item[2])) + "\t" + GetServices(int.Parse(item[3]))));
            }

            listBox8.DataSource = SheelDataList;
            listBox8.DisplayMember = "GetName";
            listBox8.ValueMember = "GetId";
        }

        private void ListBoxCustomerSelect()
        {
            ArrayList SheelDataList = new ArrayList();

            foreach (string[] item in DataBase.GetListCSheet("SELECT * FROM CalculationSheet"))
            {
                SheelDataList.Add(new CreateListOP(item[0].ToString(), GetNameСustomer(int.Parse(item[1]))));
            }

            listBox7.DataSource = SheelDataList;
            listBox7.DisplayMember = "GetName";
            listBox7.ValueMember = "GetId";

            listBox6.DataSource = SheelDataList;
            listBox6.DisplayMember = "GetName";
            listBox6.ValueMember = "GetId";

            listBox3.DataSource = SheelDataList;
            listBox3.DisplayMember = "GetName";
            listBox3.ValueMember = "GetId";
        }

        private string GetNameСustomer(int id)
        {
            SqlCommand data = DataBase.command("SELECT [Name] FROM [Customer] WHERE [Id] = '" + id + "'");
            SqlDataReader reader = data.ExecuteReader();

            reader.Read();
            string NameCustomer = reader[0].ToString();
            reader.Close();

            return NameCustomer;
        }
        private string GetEmployees(int id)
        {
            SqlCommand data = DataBase.command("SELECT Name FROM Employees WHERE Id = '" + id + "'");
            SqlDataReader reader = data.ExecuteReader();

            reader.Read();
            string Employees = reader[0].ToString();
            reader.Close();

            return Employees;
        }

        private string GetServices(int id)
        {
            SqlCommand data = DataBase.command("SELECT * FROM Services WHERE Id = '" + id + "'");
            SqlDataReader reader = data.ExecuteReader();

            reader.Read();
            string Services = reader[1].ToString() + " " + reader[2].ToString() + "руб";

            reader.Close();

            return Services;
        }

        private void AddListEmployees()
        {
            ArrayList allDataEm = new ArrayList();

            SqlCommand data = DataBase.command("SELECT * FROM Employees");
            SqlDataReader SDR = data.ExecuteReader();

            foreach (DbDataRecord rezult in SDR)
            {
                allDataEm.Add(rezult);
            }
            SDR.Close();

            listBox4.DataSource = allDataEm;
            listBox4.DisplayMember = "Name";
            listBox4.ValueMember = "Id";

            listBox1.DataSource = allDataEm;
            listBox1.DisplayMember = "Name";
            listBox1.ValueMember = "Id";
        }

        private void AddListBoxServices()
        {
            ArrayList allData = new ArrayList();

            SqlCommand data = DataBase.command("SELECT * FROM Services");
            SqlDataReader SDR = data.ExecuteReader();

            foreach (DbDataRecord rezult in SDR)
            {
                allData.Add(rezult);
            }

            SDR.Close();

            listBox2.DataSource = allData;
            listBox2.DisplayMember = "Name";
            listBox2.ValueMember = "Id";

            listBox5.DataSource = allData;
            listBox5.DisplayMember = "Name";
            listBox5.ValueMember = "Id";
        }

        private void LoadDataCSheet()
        {
            foreach (string[] item in DataBase.GetListCSheet("SELECT * FROM CalculationSheet"))
            {
                item[1] = GetNameСustomer(int.Parse(item[1]));
                item[2] = GetEmployees(int.Parse(item[2]));
                item[3] = GetServices(int.Parse(item[3]));

                dataGridView1.Rows.Add(item);
            }
        }

        /*
         * Delete
         */

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand data = DataBase.command($"SELECT * FROM CalculationSheet WHERE Id = '{listBox3.SelectedValue}'");
            SqlDataReader reader = data.ExecuteReader();

            reader.Read();

            int IdCustomer = int.Parse(reader[1].ToString());
            int IdCSheel = int.Parse(reader[0].ToString());

            reader.Close();

            DataBase.command($"DELETE FROM [Customer] WHERE Id = '" + IdCustomer + "'").ExecuteNonQuery();
            DataBase.command($"DELETE FROM [CalculationSheet] WHERE Id = '" + IdCSheel + "'").ExecuteNonQuery();

            MessageBox.Show("Запись с клиентом удалина!");
        }

        /*
         * Добавить
         */
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                DataBase.command(
                    $"INSERT INTO [Customer] ([Name]) VALUES (N'{textBox2.Text}')"
                    ).ExecuteNonQuery();

                SqlCommand commandLastId = DataBase.command(
                    "SELECT @@IDENTITY"
                    );

                int lastId = int.Parse(commandLastId.ExecuteScalar().ToString());

                DataBase.command($"INSERT INTO [CalculationSheet] (Id_Customer, Id_Employees, Id_Services) VALUES ('{lastId}', '{listBox4.SelectedValue}', '{listBox5.SelectedValue}')").ExecuteNonQuery();

                MessageBox.Show("Запись добавлена успешно!!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                DataBase.command($"UPDATE [CalculationSheet] SET Id_Employees = '{listBox1.SelectedValue}', Id_Services = '{listBox2.SelectedValue}' WHERE Id = '{listBox6.SelectedValue}'").ExecuteNonQuery();


                DataBase.command($"UPDATE [Customer] SET Name = N'{textBox1.Text}' WHERE Id = '{DataIdCustomer}'").ExecuteNonQuery();

                MessageBox.Show("Данные обновлены!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();

            foreach (string[] item in DataBase.GetListCSheet($"SELECT * FROM [CalculationSheet] WHERE Id = '{listBox6.SelectedValue}'"))
            {
                DataIdCustomer = int.Parse(item[1]);

                item[1] = GetNameСustomer(int.Parse(item[1]));
                item[2] = GetEmployees(int.Parse(item[2]));
                item[3] = GetServices(int.Parse(item[3]));

                textBox1.Text = item[1];
                dataGridView2.Rows.Add(item);
            }
        }
    }
}
