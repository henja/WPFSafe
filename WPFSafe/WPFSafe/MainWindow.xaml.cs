using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;


namespace WPFSafe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Database sqliteCon = new Database();

            try
            {
                //----Load customer names into listbox----
                sqliteCon.OpenConnection();
                string listcustQuery = "SELECT * FROM customers";
                SQLiteCommand listcustCommand = new SQLiteCommand(listcustQuery, sqliteCon.myConnection);
                SQLiteDataReader dr = listcustCommand.ExecuteReader();
                while (dr.Read())
                {
                    string name = dr.GetString(1);
                    CustomerListBox.Items.Add(name);
                }
                sqliteCon.CloseConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            
        }

        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Database sqliteCon = new Database();
            string customerID = "SELECT cust_id from customers where name='" + CustomerListBox.SelectedItem + "' ";
            int custID;
            try
            {
                
                //----Load customer data into Data tab's textbox based on listbox selection----
                sqliteCon.OpenConnection();
                string listcustQuery = "SELECT * FROM customers where name='" + CustomerListBox.SelectedItem + "' ";
                customerDisplayName.Text = (string)CustomerListBox.SelectedItem;
                SQLiteCommand listcustCommand = new SQLiteCommand(listcustQuery, sqliteCon.myConnection);
                SQLiteDataReader dr = listcustCommand.ExecuteReader();
                string data = "";

                contentTextBox.SelectAll();

                while (dr.Read())
                {
                    data = dr.GetString(2);
                    contentTextBox.AppendText(data);
                }

                //contentTextBox.Text = data;
                //contentTextBox.AppendText(data);
                SQLiteCommand custIDCommand = new SQLiteCommand(customerID, sqliteCon.myConnection);
                SQLiteDataReader custDR = custIDCommand.ExecuteReader();

                while (custDR.Read())
                {
                    //----Load customer's misc data into Misc tab's datagrid based on listbox selection----
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
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        
        private void dataSearchButton_Click(object sender, RoutedEventArgs e)
        {/*
            int index = 0;
            String temp = dataTextBox.Text;
            dataTextBox.Text = "";
            dataTextBox.Text = temp;

            while (index < dataTextBox.Text.LastIndexOf(dataSearchField.Text))
            {
                dataTextBox

            }*/
        }
    }
}
