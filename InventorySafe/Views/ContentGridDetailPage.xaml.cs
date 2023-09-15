using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

using InventorySafe.Contracts.Views;
using InventorySafe.Core.Contracts.Services;
using InventorySafe.Core.Models;

namespace InventorySafe.Views;

public partial class ContentGridDetailPage : Page, INotifyPropertyChanged, INavigationAware
{
    private readonly ISqliteHandler _sampleDataService;
    private Product _item;

    public Product Item
    {
        get { return _item; }
        set { Set(ref _item, value); }
    }

    public ContentGridDetailPage(ISqliteHandler sampleDataService)
    {
        _sampleDataService = sampleDataService;
        InitializeComponent();
        DataContext = this;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is long orderID)
        {
            var data = await _sampleDataService.GetProducts();
            Item = data.First(i => i.Id == orderID);
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
}
