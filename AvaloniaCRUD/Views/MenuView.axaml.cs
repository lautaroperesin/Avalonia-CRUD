using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaCRUD.Models;
using AvaloniaCRUD.ViewModels;

namespace AvaloniaCRUD.Views;

public partial class MenuView : Window
{
    public MenuView()
    {
        InitializeComponent();
    }
    
    private void OnSalirClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void OnCarrerasClick(object? sender, RoutedEventArgs e)
    {
        var carrerasView = new CarrerasView()
        {
            DataContext = new CarrerasViewModel()
        };
        carrerasView.ShowDialog(this);
    }
}