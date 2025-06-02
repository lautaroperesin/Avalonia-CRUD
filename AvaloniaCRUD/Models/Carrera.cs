using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaCRUD.Models;

public partial class Carrera : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string nombre;

    [ObservableProperty]
    private string sigla;

    [ObservableProperty]
    private bool eliminado;
}