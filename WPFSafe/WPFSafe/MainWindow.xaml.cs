using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            try
            {
                Database sqliteCon = new Database();
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
                MessageBox.Show(ex.Message);
            }
            
        }

        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Database sqliteCon = new Database();
                sqliteCon.OpenConnection();
                string listcustQuery = "SELECT * FROM customers where name='" + CustomerListBox.SelectedItem + "' ";
                SQLiteCommand listcustCommand = new SQLiteCommand(listcustQuery, sqliteCon.myConnection);
                SQLiteDataReader dr = listcustCommand.ExecuteReader();
                while (dr.Read())
                {
                    string data = dr.GetString(2);
                    DataTextBox.Text = data;
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
