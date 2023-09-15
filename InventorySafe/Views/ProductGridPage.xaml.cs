using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using InventorySafe.Contracts.Views;
using InventorySafe.Helpers;

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
        _sampleDataService.InsertData(cell, ((System.Windows.Controls.TextBox)e.EditingElement).Text, StockName.Text);
        RefreshList();
    }
}
