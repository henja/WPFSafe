using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace WPFSafe
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DataEditWindow : Window
    {
        private string custName;

        public DataEditWindow(string customerData, string customerName)
        {
            InitializeComponent();
            contentTextBox.AppendText(customerData);
            custName = customerName;

            this.Closed += new EventHandler(DataEditWindow_Closed);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ReadMiscData();
            
        }

        void ReadMiscData()
        {
            Database sqliteCon = new Database();
            DataSet ds;
            //string dataQuery = "Select * FROM customers where name='" + myString + "' ";
            string customerID = "SELECT cust_id from customers where name='" + custName + "' ";
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
                    //dataAdp.Update(dt);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string data = StringFromRichTextBox(contentTextBox);
            Database con = new Database();
            {
                try
                {
                    
                    SQLiteCommand cmd = new SQLiteCommand(con.myConnection);
                    cmd.CommandText = "UPDATE customers SET data = @data WHERE name='" + custName + "' ";
                    cmd.Parameters.AddWithValue("@data", data);
                    con.OpenConnection();
                    cmd.ExecuteNonQuery();
                    con.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //----Closes DataEditWindow and will call DataEditWindow_Closed method---
            Close();
        }

        //----This will update the MainWindow data text box with the newly saved data (without querying the db)----
        private void DataEditWindow_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if(mainWindow != null)
            {
                string data = StringFromRichTextBox(contentTextBox);
                mainWindow.UpdateFrom_DataEditWindow(data);
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
