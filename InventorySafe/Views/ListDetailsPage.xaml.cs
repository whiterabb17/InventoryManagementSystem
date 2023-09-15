using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using InventorySafe.Contracts.Services;
using InventorySafe.Contracts.Views;
using InventorySafe.Core.Contracts.Services;
using InventorySafe.Helpers;

namespace InventorySafe.Views;

public partial class ListDetailsPage : Page, INotifyPropertyChanged, INavigationAware
{
    private readonly INavigationService _navigationService;
    private static SqliteHandler _sampleDataService;
    private static SQLiteConnection sqliteConnection;

    private Product _selected;

    public Product Selected
    {
        get { return _selected; }
        set { Set(ref _selected, value); }
    }

    public ObservableCollection<Product> StockItems { get; private set; } = new ObservableCollection<Product>();

    public ListDetailsPage(INavigationService navigationService)
    {
        _navigationService = navigationService;
        InitializeComponent();
        DataContext = this;
        _sampleDataService = new SqliteHandler();
        sqliteConnection = _sampleDataService.CreateConnection();
    }

    public async void OnNavigatedTo(object parameter)
    {
        StockItems.Clear();

        var data = await _sampleDataService.GetProducts();

        foreach (var item in data)
        {
            StockItems.Add(item);
        }

        Selected = StockItems.First();
    }

    public async void RefreshStockList()
    {
        StockItems.Clear();

        var data = await _sampleDataService.GetProducts();

        foreach (var item in data)
        {
            StockItems.Add(item);
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

    private void ListView_Selected(object sender, System.Windows.RoutedEventArgs e)
    {
        string imagestring = _sampleDataService.GetImageString(Selected.Name);
        string _ = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllBytes(_, Convert.FromBase64String(imagestring));
        BitmapImage bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(_);
        bitmap.EndInit();
        DisplayImage.Source = bitmap;
    }
}
