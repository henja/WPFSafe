using System;
using System.Windows;
using System.Windows.Controls;
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

        public DataEditWindow(string customerData, string customerName)
        {
            InitializeComponent();
            contentTextBox.AppendText(customerData);
            this.myString = customerName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Database sqliteCon = new Database();
            //string dataQuery = "Select * FROM customers where name='" + myString + "' ";
            string customerID = "SELECT cust_id from customers where name='" + myString + "' ";
            int custID;

            try
            {
                sqliteCon.OpenConnection();

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringFromRichTextBox(contentTextBox);
            Database con = new Database();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "Update "

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
                );
            return textRange.Text;
        }
    }
}
