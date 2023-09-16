using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using InventorySafe.Contracts.Views;
using InventorySafe.Helpers;
using InventorySafe.Services;
using InventorySafe.Windows;

namespace InventorySafe.Views;

public partial class ProductGridPage : Page, INotifyPropertyChanged, INavigationAware
{
    private static SqliteHandler _sampleDataService;
    private static SQLiteConnection sqliteConnection;
    private static string SelectedProduct = null;

    public ObservableCollection<Product> Source { get; set; } 

    public ProductGridPage()
    {
        _sampleDataService = new SqliteHandler();
        sqliteConnection = _sampleDataService.CreateConnection();
        InitializeComponent();
        DataContext = this;
        Source = Helpers.InnerConstants.Source;
    }

    public async void WorkOutProfits()
    {
        _sampleDataService.GetValues(StockName.Text);
        RefreshList();
    }


    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        var data = _sampleDataService.ReadData();

        foreach (var item in data)
        {
            Source.Add(item);
        }
        productGridList.ItemsSource = Source;
        idColoum.Width = 10;
    }

    public async void RefreshList()
    {
        Source.Clear();

        // Replace this with your actual data
        var data = await _sampleDataService.GetProducts();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)Profitize.IsChecked)
        {
            WorkOutProfits();
        }
        else
        { 
            string outName = StockName.Text;
            string stockOut = StockOut.Text;
            foreach (Product listItem in productGridList.ItemsSource)
            {
                if (listItem.Name == outName)
                {
                    if ((bool)DeleteCheck.IsChecked)
                    {
                        _sampleDataService.Delete(listItem.Name);
                    }
                    else
                    {
                        int newcount = 0;
                        if ((bool)StockInCheck.IsChecked)
                            newcount = int.Parse(listItem.StockCount) + int.Parse(stockOut);
                        else
                            newcount = int.Parse(listItem.StockCount) - int.Parse(stockOut);
                        int price = (bool)EmployeeCheckBox.IsChecked ? int.Parse(listItem.RestockPrice) : int.Parse(listItem.SalesPrice);
                        //int total = (int.Parse(listItem.SalesPrice) * int.Parse(stockOut));
                        int total = (price * int.Parse(stockOut));
                        int tcost = (int.Parse(listItem.CostPrice) * int.Parse(stockOut));
                        string _profit = Convert.ToString(total - tcost);
                        if (listItem.Profit == "TBD")
                            listItem.Profit = Convert.ToString(0);
                        var _prof = (int.Parse(listItem.Profit) + int.Parse(_profit));
                        //MessageBox.Show($"Total Sale: {total}\nTotal Cost: {tcost}\nTotal Profit: {_profit}\nCurrent Profit: {_prof}");
                        string curProfit = Convert.ToString(_prof);
                        _sampleDataService.UpdateDB(outName, Convert.ToString(newcount), curProfit);
                    }
                }
            }
            RefreshList();
        }
    }
    private async Task OpenContentGridPageAsync(object sender, RoutedEventArgs e)
    {
        await new ApplicationHostService().HandleActivationAsync(typeof(ContentGridPage));
        return;
    }

    private void DeleteItem(object sender, RoutedEventArgs e)
    {
        _sampleDataService.Delete(StockName.Text);
    }

    private void productGridList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (Product listItem in productGridList.ItemsSource)
        { 
            if (listItem.Id == productGridList.SelectedIndex)
                SelectedProduct = listItem.Name;
        }
        StockName.Text = SelectedProduct;        
    }

    private void productGridList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        foreach (Product listItem in productGridList.ItemsSource)
        {
            if (listItem.Id == productGridList.SelectedIndex)
                _sampleDataService.FullUpdate(listItem);
        }
        RefreshList();
    }

    private void productGridList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {        
    }

    private void productGridList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        var cell = ((System.Windows.Data.Binding)((System.Windows.Controls.DataGridBoundColumn)e.Column).Binding).Path.Path;
        SelectedCell = cell.ToString();
        _sampleDataService.InsertData(cell, ((System.Windows.Controls.TextBox)e.EditingElement).Text, StockName.Text);
        RefreshList();
    }

    private static string SelectedCell = "";
    private void StockDialog(object sender, RoutedEventArgs e)
    {
        string inputRead = new InputBox("Please enter the Stock Replenishment Value", $"Additional Stock for {StockName.Text}", "Arial", 20).ShowDialog();
        _sampleDataService.InsertData("StockCount", inputRead, StockName.Text);
    }
    private void ShowTODODialog(object sender, RoutedEventArgs e)
    {
        string inputRead = new InputBox("Please enter the Replacement Value", $"Editting {SelectedCell}", "Arial", 20).ShowDialog();
        _sampleDataService.InsertData(SelectedCell, inputRead, StockName.Text);
    }
}
public class InputBox
{

    Window Box = new Window();// window for the inputbox
    FontFamily font = new FontFamily("Tahoma");// font for the whole inputbox
    int FontSize = 30;// fontsize for the input
    StackPanel sp1 = new StackPanel();// items container
    string title = "InputBox";// title as heading
    string boxcontent;// title
    string defaulttext = "Write here your name...";// default textbox content
    string errormessage = "Invalid answer";// error messagebox content
    string errortitle = "Error";// error messagebox heading title
    string okbuttontext = "OK";// Ok button content
    Brush BoxBackgroundColor = Brushes.GreenYellow;// Window Background
    Brush InputBackgroundColor = Brushes.Ivory;// Textbox Background
    bool clicked = false;
    TextBox input = new TextBox();
    Button ok = new Button();
    bool inputreset = false;

    public InputBox(string content)
    {
        try
        {
            boxcontent = content;
        }
        catch { boxcontent = "Error!"; }
        windowdef();
    }

    public InputBox(string content, string Htitle, string DefaultText)
    {
        try
        {
            boxcontent = content;
        }
        catch { boxcontent = "Error!"; }
        try
        {
            title = Htitle;
        }
        catch
        {
            title = "Error!";
        }
        try
        {
            defaulttext = DefaultText;
        }
        catch
        {
            DefaultText = "Error!";
        }
        windowdef();
    }

    public InputBox(string content, string Htitle, string Font, int Fontsize)
    {
        try
        {
            boxcontent = content;
        }
        catch { boxcontent = "Error!"; }
        try
        {
            font = new FontFamily(Font);
        }
        catch { font = new FontFamily("Tahoma"); }
        try
        {
            title = Htitle;
        }
        catch
        {
            title = "Error!";
        }
        if (Fontsize >= 1)
            FontSize = Fontsize;
        windowdef();
    }

    private void windowdef()// window building - check only for window size
    {
        Box.Height = 500;// Box Height
        Box.Width = 300;// Box Width
        Box.Background = BoxBackgroundColor;
        Box.Title = title;
        Box.Content = sp1;
        Box.Closing += Box_Closing;
        TextBlock content = new TextBlock();
        content.TextWrapping = TextWrapping.Wrap;
        content.Background = null;
        content.HorizontalAlignment = HorizontalAlignment.Center;
        content.Text = boxcontent;
        content.FontFamily = font;
        content.FontSize = FontSize;
        sp1.Children.Add(content);

        input.Background = InputBackgroundColor;
        input.FontFamily = font;
        input.FontSize = FontSize;
        input.HorizontalAlignment = HorizontalAlignment.Center;
        input.Text = defaulttext;
        input.MinWidth = 200;
        input.MouseEnter += input_MouseDown;
        sp1.Children.Add(input);
        ok.Width = 70;
        ok.Height = 30;
        ok.Click += ok_Click;
        ok.Content = okbuttontext;
        ok.HorizontalAlignment = HorizontalAlignment.Center;
        sp1.Children.Add(ok);

    }

    void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!clicked)
            e.Cancel = true;
    }

    private void input_MouseDown(object sender, MouseEventArgs e)
    {
        if ((sender as TextBox).Text == defaulttext && inputreset == false)
        {
            (sender as TextBox).Text = null;
            inputreset = true;
        }
    }

    void ok_Click(object sender, RoutedEventArgs e)
    {
        clicked = true;
        if (input.Text == defaulttext || input.Text == "")
            MessageBox.Show(errormessage, errortitle);
        else
        {
            Box.Close();
        }
        clicked = false;
    }

    public string ShowDialog()
    {
        Box.ShowDialog();
        return input.Text;
    }
}
