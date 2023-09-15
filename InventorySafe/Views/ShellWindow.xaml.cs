using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using InventorySafe.Contracts.Services;
using InventorySafe.Contracts.Views;

using MahApps.Metro.Controls;

namespace InventorySafe.Views;

public partial class ShellWindow : MetroWindow, IShellWindow, INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;
    private bool _canGoBack;
    private HamburgerMenuItem _selectedMenuItem;

    public bool CanGoBack
    {
        get { return _canGoBack; }
        set { Set(ref _canGoBack, value); }
    }

    public HamburgerMenuItem SelectedMenuItem
    {
        get { return _selectedMenuItem; }
        set { Set(ref _selectedMenuItem, value); }
    }

    // TODO: Change the icons and titles for all HamburgerMenuItems here.
    public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuGlyphItem() { Label = Properties.Resources.ShellDataGridPage, Glyph = "\uE914", TargetPageType = typeof(ProductGridPage) },
        new HamburgerMenuGlyphItem() { Label = Properties.Resources.ShellContentGridPage, Glyph = "\uECCD", TargetPageType = typeof(ContentGridPage) },
        new HamburgerMenuGlyphItem() { Label = Properties.Resources.ShellListDetailsPage, Glyph = "\uE81E", TargetPageType = typeof(ListDetailsPage) },
        new HamburgerMenuGlyphItem() { Label = Properties.Resources.ShellContentGridPage, Glyph = "\uE7BF", TargetPageType = typeof(ShopItemGridPage) },
     //   new HamburgerMenuGlyphItem() { Label = Properties.Resources.ShellListDetailsPage, Glyph = "\uE8A5", TargetPageType = typeof(ListDetailsPage) },
    };

    public ShellWindow(INavigationService navigationService)
    {
        _navigationService = navigationService;
        InitializeComponent();
        DataContext = this;
    }

    public Frame GetNavigationFrame()
        => shellFrame;

    public void ShowWindow()
        => Show();

    public void CloseWindow()
        => Close();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigated += OnNavigated;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigated -= OnNavigated;
    }

    private void OnItemClick(object sender, ItemClickEventArgs args)
        => NavigateTo(SelectedMenuItem.TargetPageType);

    private void NavigateTo(Type targetPage)
    {
        if (targetPage != null)
        {
            _navigationService.NavigateTo(targetPage);
        }
    }

    private void OnNavigated(object sender, Type pageType)
    {
        var item = MenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => pageType == i.TargetPageType);
        if (item != null)
        {
            SelectedMenuItem = item;
        }

        CanGoBack = _navigationService.CanGoBack;
    }

    private void OnGoBack(object sender, RoutedEventArgs e)
    {
        _navigationService.GoBack();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
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
