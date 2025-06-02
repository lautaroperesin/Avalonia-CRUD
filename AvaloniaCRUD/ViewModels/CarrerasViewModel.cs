using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using AvaloniaCRUD.Models;
using AvaloniaCRUD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace AvaloniaCRUD.ViewModels;

public partial class CarrerasViewModel : ViewModelBase
{
    public ObservableCollection<Carrera> Carreras { get; set; } = new();

    [ObservableProperty]
    private string nombre;

    [ObservableProperty]
    private string sigla;

    [ObservableProperty]
    private bool eliminado;

    [ObservableProperty]
    private Carrera carreraSeleccionada;

    private readonly GenericService<Carrera> carreraService = new();

    public CarrerasViewModel()
    {
        ObtenerCarreras();
    }

    partial void OnNombreChanged(string value)
    {
        AgregarCommand.NotifyCanExecuteChanged();
        ModificarCommand.NotifyCanExecuteChanged();
    }

    partial void OnSiglaChanged(string value)
    {
        AgregarCommand.NotifyCanExecuteChanged();
        ModificarCommand.NotifyCanExecuteChanged();
    }

    partial void OnCarreraSeleccionadaChanged(Carrera value)
    {
        if (value != null)
        {
            Nombre = value.Nombre;
            Sigla = value.Sigla;
            Eliminado = value.Eliminado;
        }

        AgregarCommand.NotifyCanExecuteChanged(); // ACTUALIZADO
        ModificarCommand.NotifyCanExecuteChanged();
        EliminarCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PermitirAgregar))]
    private async Task Agregar()
    {
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Sigla))
            return;

        var nuevaCarrera = new Carrera
        {
            Nombre = Nombre,
            Sigla = Sigla,
            Eliminado = Eliminado
        };

        try
        {
            var carreraAgregada = await carreraService.AddAsync(nuevaCarrera);
            if (carreraAgregada != null)
            {
                Carreras.Add(carreraAgregada);
                // Limpiar campos luego de agregar
                Nombre = string.Empty;
                Sigla = string.Empty;
                Eliminado = false;
            }
        }
        catch (Exception ex)
        {
            Debug.Print("Error al agregar carrera: {0}", ex.Message);
        }
    }

    private bool PermitirAgregar() =>
        !string.IsNullOrWhiteSpace(Nombre)
        && !string.IsNullOrWhiteSpace(Sigla)
        && CarreraSeleccionada == null; // <- Esto es lo nuevo

    [RelayCommand(CanExecute = nameof(PermitirModificar))]
    private async Task Modificar()
    {
        if (CarreraSeleccionada == null)
            return;

        CarreraSeleccionada.Nombre = Nombre;
        CarreraSeleccionada.Sigla = Sigla;
        CarreraSeleccionada.Eliminado = Eliminado;

        try
        {
            var result = await carreraService.UpdateAsync(CarreraSeleccionada);
            if (!result)
                Debug.Print("Error al modificar carrera");
        }
        catch (Exception ex)
        {
            Debug.Print("Error al modificar carrera: {0}", ex.Message);
        }
    }

    private bool PermitirModificar() =>
        CarreraSeleccionada != null
        && !string.IsNullOrWhiteSpace(Nombre)
        && !string.IsNullOrWhiteSpace(Sigla);

    [RelayCommand(CanExecute = nameof(PermitirEliminar))]
    private async Task<object> Eliminar()
    {
        if (CarreraSeleccionada == null)
            return null;

        var messageBox = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.YesNo,
            ContentTitle = "Confirmar Eliminación",
            ContentMessage = $"¿Está seguro que desea eliminar la carrera '{CarreraSeleccionada.Nombre}'?",
            Icon = Icon.Question
        });

        var result = await messageBox.ShowAsync();
        if (result != ButtonResult.Yes)
            return null;

        try
        {
            await carreraService.DeleteAsync(CarreraSeleccionada.Id);
            Carreras.Remove(CarreraSeleccionada);
            CarreraSeleccionada = null; // Limpiar selección
        }
        catch (Exception ex)
        {
            Debug.Print("Error al eliminar carrera: {0}", ex.Message);
        }

        return null;
    }

    private bool PermitirEliminar() => CarreraSeleccionada != null;

    private async void ObtenerCarreras()
    {
        var carrerasList = await carreraService.GetAllAsync();
        Carreras = new ObservableCollection<Carrera>(carrerasList);
        OnPropertyChanged(nameof(Carreras));
    }
}