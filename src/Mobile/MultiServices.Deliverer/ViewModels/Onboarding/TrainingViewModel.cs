using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class TrainingViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private int _currentVideoIndex;
    [ObservableProperty] private string _currentVideoUrl = string.Empty;
    [ObservableProperty] private string _currentVideoTitle = string.Empty;
    [ObservableProperty] private int _totalVideos;
    [ObservableProperty] private int _watchedVideos;
    [ObservableProperty] private double _progressPercent;
    [ObservableProperty] private bool _currentVideoWatched;
    [ObservableProperty] private bool _allVideosWatched;

    private readonly HashSet<int> _watchedSet = new();

    public TrainingViewModel(ApiService api)
    {
        _api = api;
        Title = "Formation obligatoire";
        TotalVideos = AppConstants.TrainingVideoUrls.Length;
        LoadVideo(0);
    }

    private void LoadVideo(int index)
    {
        if (index < 0 || index >= TotalVideos) return;
        CurrentVideoIndex = index;
        CurrentVideoUrl = AppConstants.TrainingVideoUrls[index];
        CurrentVideoTitle = AppConstants.TrainingVideoTitles[index];
        CurrentVideoWatched = _watchedSet.Contains(index);
    }

    [RelayCommand]
    private void MarkVideoWatched()
    {
        _watchedSet.Add(CurrentVideoIndex);
        WatchedVideos = _watchedSet.Count;
        ProgressPercent = (double)WatchedVideos / TotalVideos * 100;
        CurrentVideoWatched = true;
        AllVideosWatched = WatchedVideos >= TotalVideos;
    }

    [RelayCommand]
    private void NextVideo()
    {
        if (CurrentVideoIndex < TotalVideos - 1)
            LoadVideo(CurrentVideoIndex + 1);
    }

    [RelayCommand]
    private void PreviousVideo()
    {
        if (CurrentVideoIndex > 0)
            LoadVideo(CurrentVideoIndex - 1);
    }

    [RelayCommand]
    private async Task MarkVideoCompletedOnServerAsync()
    {
        try
        {
            await _api.PostAsync<ApiResponse<object>>(
                $"{AppConstants.DelivererTrainingEndpoint}/video/{CurrentVideoIndex}/complete");
        }
        catch { }
    }

    [RelayCommand]
    private async Task GoToQuizAsync()
    {
        if (!AllVideosWatched)
        {
            ErrorMessage = "Regardez toutes les vidéos avant de passer au quiz";
            HasError = true;
            return;
        }
        await Shell.Current.GoToAsync("trainingQuiz");
    }
}