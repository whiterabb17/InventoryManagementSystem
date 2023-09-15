using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using InventorySafe.Contracts.Services;
using InventorySafe.Contracts.Views;
using InventorySafe.Helpers;
using InventorySafe.Models;

namespace InventorySafe.Views;

public partial class ShopItemGridPage : Page, INotifyPropertyChanged, INavigationAware
{
    private readonly INavigationService _navigationService;
    private static SqliteHandler _sampleDataService;
    private static SQLiteConnection sqliteConnection;

    public ObservableCollection<ShopItem> Source { get; set; }

    public ShopItemGridPage(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _sampleDataService = new SqliteHandler();
        sqliteConnection = _sampleDataService.CreateConnection();
        InitializeComponent();
        DataContext = this;
        Source = InnerConstants.ShopSource;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // Replace this with your actual data
        var data = await _sampleDataService.GetShopProducts();
        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        => SelectItem(e);

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SelectItem(e);
            e.Handled = true;
        }
    }

    private static int SelectedProductId { get; set; }

    private void SelectItem(RoutedEventArgs args)
    {
        if (args.OriginalSource is FrameworkElement selectedItem
            && selectedItem.DataContext is Product order)
        {
            SelectedProductId = order.Id;
            //_navigationService.NavigateTo(typeof(ContentGridDetailPage), order.Id);
        }
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
        try
        {
            var newproduct = new ShopItem
            {
                Name = NewNanme.Text,                   // Product Name
                StorePrice = StockCount.Text,           // Amount of Stock In
                CostPrice = CostPrice.Text,             // Cost Price per G
                Sales = null                     // Determined overtime
            };
            _sampleDataService.InsertShopData(newproduct);
            //Helpers.InnerConstants.Source.Add(newproduct);
            MessageBox.Show($"{newproduct.Name} has been added to the\ninternal inventory list");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}
