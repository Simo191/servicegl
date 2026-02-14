using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class DocumentUploadViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string? _idCardPath;
    [ObservableProperty] private string? _licensePath;
    [ObservableProperty] private string? _insurancePath;
    [ObservableProperty] private string? _photoPath;
    [ObservableProperty] private bool _idCardUploaded;
    [ObservableProperty] private bool _licenseUploaded;
    [ObservableProperty] private bool _insuranceUploaded;
    [ObservableProperty] private bool _photoUploaded;
    [ObservableProperty] private string? _rejectionReason;
    [ObservableProperty] private double _uploadProgress;

    public bool AllUploaded => IdCardUploaded && LicenseUploaded && InsuranceUploaded && PhotoUploaded;

    public DocumentUploadViewModel(ApiService api)
    {
        _api = api;
        Title = "Documents requis";
    }

    [RelayCommand]
    private async Task PickIdCardAsync() => await PickAndUpload("id_card", v => { IdCardPath = v; IdCardUploaded = true; });

    [RelayCommand]
    private async Task PickLicenseAsync() => await PickAndUpload("license", v => { LicensePath = v; LicenseUploaded = true; });

    [RelayCommand]
    private async Task PickInsuranceAsync() => await PickAndUpload("insurance", v => { InsurancePath = v; InsuranceUploaded = true; });

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo != null)
            {
                var localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using var stream = await photo.OpenReadAsync();
                using var localFile = File.OpenWrite(localPath);
                await stream.CopyToAsync(localFile);
                localFile.Close();

                await UploadDocument(localPath, "photo");
                PhotoPath = localPath;
                PhotoUploaded = true;
                OnPropertyChanged(nameof(AllUploaded));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur photo: {ex.Message}";
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync() => await PickAndUpload("photo", v => { PhotoPath = v; PhotoUploaded = true; });

    private async Task PickAndUpload(string docType, Action<string> onSuccess)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Sélectionner le document",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "image/*", "application/pdf" } },
                    { DevicePlatform.iOS, new[] { "public.image", "com.adobe.pdf" } }
                })
            });

            if (result != null)
            {
                await UploadDocument(result.FullPath, docType);
                onSuccess(result.FullPath);
                OnPropertyChanged(nameof(AllUploaded));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
            HasError = true;
        }
    }

    private async Task UploadDocument(string filePath, string docType)
    {
        await ExecuteAsync(async () =>
        {
            UploadProgress = 0.5;
            await _api.UploadAsync<ApiResponse<object>>(
                $"{AppConstants.DelivererDocumentsEndpoint}/{docType}", filePath, "document");
            UploadProgress = 1.0;
        }, $"Erreur lors de l'upload du document");
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (!AllUploaded)
        {
            ErrorMessage = "Veuillez uploader tous les documents requis";
            HasError = true;
            return;
        }
        await Shell.Current.GoToAsync("//training");
    }
}