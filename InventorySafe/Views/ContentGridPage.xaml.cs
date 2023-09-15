using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using InventorySafe.Contracts.Services;
using InventorySafe.Contracts.Views;
using InventorySafe.Helpers;
using InventorySafe.Models;
using Microsoft.Win32;

namespace InventorySafe.Views;

public partial class ContentGridPage : Page, INotifyPropertyChanged, INavigationAware
{
    private readonly INavigationService _navigationService;
    private static SqliteHandler _sampleDataService;
    private static SQLiteConnection sqliteConnection;

    public ObservableCollection<Product> Source { get; set; }

    public ContentGridPage(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _sampleDataService = new SqliteHandler();
        sqliteConnection = _sampleDataService.CreateConnection();
        InitializeComponent();
        DataContext = this;
        Source = Helpers.InnerConstants.Source;
    }

    public async void OnNavigatedTo(object parameter)
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

    private void SelectImage(object sender, RoutedEventArgs e)
    {
        try
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            op.ShowDialog();
            string selectedFileName = op.FileName;
            imageLocation = selectedFileName;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(selectedFileName);
            bitmap.EndInit();
            ProductImage.Source = bitmap;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
    private static string imageLocation = "";
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var newproduct = new Product
            {
                Name = NewNanme.Text,                   // Product Name
                StockCount = StockCount.Text,           // Amount of Stock In
                CostPrice = CostPrice.Text,             // Cost Price per G
                InitialCost = Convert.ToString(int.Parse(CostPrice.Text) * int.Parse(StockCount.Text)),      // Total InitalCost of Product
                SalesPrice = SalesPrice.Text,           // Sale Price per G
                RestockPrice = RestockPrice.Text,       // Restock price of the product
                Profit = "TBD",                          // Determined overtime
                ImageLocation = Convert.ToBase64String(System.IO.File.ReadAllBytes(imageLocation))
            };
            _sampleDataService.InsertData(newproduct);
            //Helpers.InnerConstants.Source.Add(newproduct);
            MessageBox.Show($"{newproduct.Name} has been added to the\ninternal inventory list");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}
