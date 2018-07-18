using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

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
                MessageBox.Show(ex.Message);
            }

        }

        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Database sqliteCon = new Database();

            string listcustQuery = "SELECT * FROM customers where name='" + CustomerListBox.SelectedItem + "' ";
            string customerID = "SELECT cust_id from customers where name='" + CustomerListBox.SelectedItem + "' ";
            int custID;

            customerDisplayName.Text = (string)CustomerListBox.SelectedItem;

            try
            {
                //----Open db connection----
                sqliteCon.OpenConnection();
                SQLiteCommand listcustCommand = new SQLiteCommand(listcustQuery, sqliteCon.myConnection);
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

                //----Load customer's misc data into Misc tab's datagrid based on listbox selection----
                SQLiteCommand custIDCommand = new SQLiteCommand(customerID, sqliteCon.myConnection);
                SQLiteDataReader custDR = custIDCommand.ExecuteReader();
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
                //----Close db connection----
                sqliteCon.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----searches the Data text box for the string passed into the search box. highlights the matches.-----
        private void DataSearchButton_Click(object sender, RoutedEventArgs e)
        {

            contentTextBox.SelectAll();
            contentTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
            contentTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            contentTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            Regex reg = new Regex(searchBox.Text.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var start = contentTextBox.Document.ContentStart;
            var start2 = MiscDataGrid.Items;
            var matchCount = 0;
            while (start != null && start.CompareTo(contentTextBox.Document.ContentEnd) < 0)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var match = reg.Match(start.GetTextInRun(LogicalDirection.Forward));

                    var textrange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
                    textrange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                    textrange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
                    textrange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    start = textrange.End;
                    matchCount++;
                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }
            SearchCount.Text = "Matches found: " + matchCount;
        }

        //-----loads the editing window after double clicking on a listbox selection-----
        private void CustomerListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string customerData = StringFromRichTextBox(contentTextBox);
            string customerName = (string)CustomerListBox.SelectedItem;
            DataEditWindow nw = new DataEditWindow(customerData, customerName);
            nw.Show();
        }


        public void UpdateFrom_DataEditWindow(string data)
        {
            contentTextBox.SelectAll();
            contentTextBox.Selection.Text = "";
            contentTextBox.AppendText(data);
        }

        public string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
                );
            return textRange.Text;
        }


        //-----calls search button click event when pressing Enter in the search box-----
        /*
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DataSearchButton_Click(sender, e);
            }
        }
        */



        /*
        private void dataSearchButton_Click(object sender, RoutedEventArgs e)
        {
            contentTextBox.SelectAll();
            contentTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
            contentTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            Regex reg = new Regex(searchBox.Text.Trim(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            TextPointer position = contentTextBox.Document.ContentStart;
            List<TextRange> ranges = new List<TextRange>();
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string text = position.GetTextInRun(LogicalDirection.Forward);
                    var matchs = reg.Matches(text);
                    foreach (Match match in matchs)
                    {
                        TextPointer start = position.GetPositionAtOffset(match.Index);
                        TextPointer end = start.GetPositionAtOffset(searchBox.Text.Trim().Length);
                        TextRange textrange = new TextRange(start, end);
                        ranges.Add(textrange);
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
            foreach (TextRange range in ranges)
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }
        */


        /*
    private void dataSearchButton_Click(object sender, RoutedEventArgs e)
    {
        DoSearch(contentTextBox, searchBox.Text, true);
    }
        public bool DoSearch(System.Windows.Controls.RichTextBox richTextBox, string searchText, bool searchNext)
    {
        TextRange searchRange;
        // Get the range to search
        if (searchNext)
            searchRange = new TextRange(
                richTextBox.Selection.Start.GetPositionAtOffset(1),
                richTextBox.Document.ContentEnd);
        else
            searchRange = new TextRange(
                richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd);
        // Do the search
        TextRange foundRange = FindTextInRange(searchRange, searchText);
        if (foundRange == null)
            return false;
        // Select the found range
        contentTextBox.Selection.Select(foundRange.Start, foundRange.End);
        return true;
    }
    public TextRange FindTextInRange(TextRange searchRange, string searchText)
    {
        // Search the text with IndexOf
        int offset = searchRange.Text.IndexOf(searchText);
        if (offset < 0)
            return null;  // Not found
        // Try to select the text as a contiguous range
        for (TextPointer start = searchRange.Start.GetPositionAtOffset(offset); start != searchRange.End; start = start.GetPositionAtOffset(1))
        {
            TextRange result = new TextRange(start, start.GetPositionAtOffset(searchText.Length));
            if (result.Text == searchText)
                return result;
        }
        return null;
    }
    */
    }
}