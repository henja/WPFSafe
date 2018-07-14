using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data;
using System.Data.SQLite;

namespace WPFSafe
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DataEditWindow : Window
    {
        private string myString;

        public DataEditWindow(string customerName)
        {
            InitializeComponent();
            this.myString = customerName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Database sqliteCon = new Database();
            string dataQuery = "Select * FROM customers where name='" + myString + "' ";
            string customerID = "SELECT cust_id from customers where name='" + myString + "' ";
            int custID;

            try
            {
                sqliteCon.OpenConnection();

                SQLiteCommand listcustCommand = new SQLiteCommand(dataQuery, sqliteCon.myConnection);
                SQLiteDataReader dr = listcustCommand.ExecuteReader();

                //----Clear data text box before reading in new data
                contentTextBox.SelectAll();
                contentTextBox.Selection.Text = "";
                string data = "";
                //----Load customer data into Data tab's textbox based on listbox selection----
                while (dr.Read())
                {
                    data = dr.GetString(2);
                    contentTextBox.AppendText(data);
                }

                Console.WriteLine(data);
                SQLiteCommand custIDCommand = new SQLiteCommand(customerID, sqliteCon.myConnection);
                SQLiteDataReader custDR = custIDCommand.ExecuteReader();

                //----Load customer's misc data into Misc tab's datagrid based on listbox selection----
                while (custDR.Read())
                {
                    custID = custDR.GetInt32(0);
                    SQLiteCommand cmdMiscData = new SQLiteCommand(" SELECT misc_name,misc_value FROM misc WHERE customer_id='" + custID + "' ", sqliteCon.myConnection);
                    SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(cmdMiscData);
                    DataTable dt = new DataTable("misc");
                    dataAdp.Fill(dt);
                    MiscDataGrid.ItemsSource = dt.DefaultView;
                    dataAdp.Update(dt);
                }

                sqliteCon.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
