using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaCRUD.Interfaces;
using AvaloniaCRUD.Models;
using AvaloniaCRUD.Services;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaCRUD.ViewModels;

public class CarrerasViewModel : ViewModelBase
{
    public ObservableCollection<Carrera> Carreras { get; set; } = new ObservableCollection<Carrera>();
    
    private GenericService<Carrera> _carreraService = new GenericService<Carrera>();
    
    public Carrera CarreraSeleccionada { get; set; }
    
    public string Nombre { get; set; }
    
    public string Sigla { get; set; }
    
    public bool Eliminado { get; set; }
    public ICommand AgregarCommand { get; }
    public ICommand ModificarCommand { get; }
    public ICommand EliminarCommand { get; }


    public CarrerasViewModel()
    {
        AgregarCommand = new RelayCommand(async () => await AgregarCarrera());
        EliminarCommand = new RelayCommand(async () => await EliminarCarrera());
        ObtenerCarreras();
    }

    private async Task EliminarCarrera()
    {
        try
        {
            if (CarreraSeleccionada == null)
            {
                Debug.Print("No se ha seleccionado ninguna carrera para eliminar.");
                return;
            }

            await _carreraService.DeleteAsync(CarreraSeleccionada.Id);
            Carreras.Remove(CarreraSeleccionada);
            OnPropertyChanged(nameof(Carreras));
            Debug.Print($"Carrera eliminada: {CarreraSeleccionada.Nombre}");
            CarreraSeleccionada = null; // Limpiar la selección
        }
        catch (Exception ex)
        {
            Debug.Print($"Error al eliminar la carrera: {ex.Message}");
        }
    }

    private async Task AgregarCarrera()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Sigla))
            {
                Debug.Print("Nombre y Sigla son obligatorios.");
                return;
            }

            var nuevaCarrera = new Carrera
            {
                Nombre = Nombre,
                Sigla = Sigla,
                Eliminado = Eliminado
            };

            var resultado = await _carreraService.AddAsync(nuevaCarrera);
            if (resultado != null)
            {
                Carreras.Add(resultado);
                OnPropertyChanged(nameof(Carreras));
                Debug.Print($"Carrera agregada: {resultado.Nombre}");
                Nombre = string.Empty;
                Sigla = string.Empty;
            }
        }
        catch (Exception ex)
        {
            Debug.Print($"Error al agregar la carrera: {ex.Message}");
        }
    }

    private async void ObtenerCarreras()
    {
        try
        {
            var carreras = await _carreraService.GetAllAsync();
            if (carreras != null)
            {
                foreach (var carrera in carreras)
                {
                    Carreras.Add(carrera);
                    Debug.Print($"Carrera obtenida: {carrera.Nombre}");
                }
                Debug.Print($"Carreras obtenida: {Carreras.Count.ToString()}");
            }
        }
        catch (Exception ex)
        {
            Debug.Print($"Error al obtener las carreras: {ex.Message}");
        }
    }
}