using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
[QueryProperty(nameof(ProductId), "id")] [QueryProperty(nameof(DepartmentIdParam), "departmentId")]
public partial class ProductFormViewModel : BaseViewModel {
    private readonly ApiService _api; [ObservableProperty] private string? _productId; [ObservableProperty] private string? _departmentIdParam;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private string _name = string.Empty; [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _brand = string.Empty; [ObservableProperty] private string _barcode = string.Empty;
    [ObservableProperty] private string _price = string.Empty; [ObservableProperty] private string _pricePerUnit = string.Empty;
    [ObservableProperty] private string _unitMeasure = string.Empty; [ObservableProperty] private string _stock = string.Empty;
    [ObservableProperty] private string _nutritionalInfo = string.Empty; [ObservableProperty] private string _allergens = string.Empty;
    [ObservableProperty] private string _origin = string.Empty; [ObservableProperty] private bool _isBio; [ObservableProperty] private bool _isHalal = true;
    public ProductFormViewModel(ApiService api) { _api = api; Title = "Nouveau produit"; }
    partial void OnProductIdChanged(string? v) { if (!string.IsNullOrEmpty(v)) { IsEditing = true; Title = "Modifier produit"; LoadCommand.ExecuteAsync(null); } }
    [RelayCommand] private async Task LoadAsync() { if (ProductId == null) return; await ExecuteAsync(async () => {
        var r = await _api.GetAsync<ProductDto>($"/grocery/store/products/{ProductId}");
        if (r.Success && r.Data != null) { var d = r.Data; Name = d.Name; Description = d.Description ?? ""; Brand = d.Brand ?? ""; Barcode = d.Barcode ?? "";
            Price = d.Price.ToString(); PricePerUnit = d.PricePerKg?.ToString() ?? ""; Stock = d.StockQuantity.ToString();
            NutritionalInfo = d.NutritionalInfo ?? ""; Allergens = string.Join(", ", d.Allergens); Origin = d.Origin ?? ""; IsBio = d.IsBio; IsHalal = d.IsHalal; }
    }); }
    [RelayCommand] private async Task ScanBarcodeAsync() { /* ZXing.Net.Maui */ await Shell.Current.DisplayAlert("Scanner", "Scannez le code-barre du produit...", "OK"); }
    [RelayCommand] private async Task UploadPhotoAsync() { var r = await MediaPicker.Default.PickPhotoAsync(); if (r != null && IsEditing) { using var s = await r.OpenReadAsync(); await _api.UploadAsync<object>($"/grocery/store/products/{ProductId}/image", s, r.FileName); } }
    [RelayCommand] private async Task SaveAsync() { await ExecuteAsync(async () => {
        var req = new CreateProductRequest { DepartmentId = Guid.TryParse(DepartmentIdParam, out var did) ? did : Guid.Empty,
            Name = Name, Description = Description, Brand = Brand, Barcode = Barcode,
            Price = decimal.TryParse(Price, out var p) ? p : 0, PricePerUnit = decimal.TryParse(PricePerUnit, out var pu) ? pu : null,
            UnitMeasure = UnitMeasure, StockQuantity = int.TryParse(Stock, out var s) ? s : 0,
            NutritionalInfo = NutritionalInfo, Allergens = Allergens, Origin = Origin, IsBio = IsBio, IsHalal = IsHalal };
        if (IsEditing) await _api.PutAsync<object>($"/grocery/store/products/{ProductId}", req);
        else await _api.PostAsync<ProductDto>("/grocery/store/products", req);
        await Shell.Current.GoToAsync("..");
    }); }
}