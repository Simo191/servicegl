using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
public partial class CatalogViewModel : BaseViewModel {
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<DepartmentDto> _departments = new();
    [ObservableProperty] private ObservableCollection<ProductDto> _products = new();
    [ObservableProperty] private DepartmentDto? _selectedDepartment; [ObservableProperty] private string _searchQuery = string.Empty;
    public CatalogViewModel(ApiService api) { _api = api; Title = "Catalogue"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<List<DepartmentDto>>("/grocery/store/departments");
        if (r.Success && r.Data != null) Departments = new(r.Data);
    }); }
    [RelayCommand] private async Task SelectDepartmentAsync(DepartmentDto d) { SelectedDepartment = d;
        var r = await _api.GetAsync<PaginatedResult<ProductDto>>("/grocery/store/products", new() { ["departmentId"] = d.Id.ToString(), ["pageSize"] = "100" });
        if (r.Success && r.Data != null) Products = new(r.Data.Items);
    }
    [RelayCommand] private async Task SearchAsync() { if (string.IsNullOrWhiteSpace(SearchQuery)) return;
        var r = await _api.GetAsync<PaginatedResult<ProductDto>>("/grocery/store/products", new() { ["search"] = SearchQuery, ["pageSize"] = "50" });
        if (r.Success && r.Data != null) Products = new(r.Data.Items);
    }
    [RelayCommand] private async Task AddProductAsync() => await Shell.Current.GoToAsync($"productform?departmentId={SelectedDepartment?.Id}");
    [RelayCommand] private async Task EditProductAsync(ProductDto p) => await Shell.Current.GoToAsync($"productform?id={p.Id}");
    [RelayCommand] private async Task ToggleProductAsync(ProductDto p) { await _api.PostAsync<object>($"/grocery/store/products/{p.Id}/toggle-availability"); p.IsAvailable = !p.IsAvailable; }
    [RelayCommand] private async Task DeleteProductAsync(ProductDto p) { if (await Shell.Current.DisplayAlert("Confirmer", $"Supprimer '{p.Name}'?", "Oui", "Non")) { await _api.DeleteAsync<object>($"/grocery/store/products/{p.Id}"); if (SelectedDepartment != null) await SelectDepartmentAsync(SelectedDepartment); } }
    [RelayCommand] private async Task BulkImportAsync() => await Shell.Current.GoToAsync("bulkimport");
    [RelayCommand] private async Task UpdatePricesAsync() { /* Mass price update */ await Shell.Current.DisplayAlert("Prix", "Mise à jour en masse des prix...", "OK"); }
    [RelayCommand] private async Task UploadPhotosAsync() {
        // Upload photos en masse
        var results = await FilePicker.Default.PickMultipleAsync(new PickOptions { FileTypes = FilePickerFileType.Images });
        if (results != null && results.Any()) {
            var files = new List<(Stream, string)>();
            foreach (var f in results) { files.Add((await f.OpenReadAsync(), f.FileName)); }
            await _api.UploadMultipleAsync<object>("/grocery/store/products/bulk-photos", files);
            await Shell.Current.DisplayAlert("OK", $"{files.Count} photos uploadées", "OK");
        }
    }
}