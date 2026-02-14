using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
public partial class ReviewsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<ReviewDto> _reviews = new();
    [ObservableProperty] private double _avgRating; [ObservableProperty] private int _totalReviews;
    public ReviewsViewModel(ApiService api) { _api = api; Title = "Avis clients"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<PaginatedResult<ReviewDto>>("/restaurant/reviews", new() { ["pageSize"] = "50" });
        if (r.Success && r.Data != null) { Reviews = new(r.Data.Items); TotalReviews = r.Data.TotalCount; }
    }); }
    [RelayCommand] private async Task ReplyAsync(ReviewDto review) {
        string reply = await Shell.Current.DisplayPromptAsync("Répondre", "Votre réponse:");
        if (reply != null) { await _api.PostAsync<object>($"/restaurant/reviews/{review.Id}/reply", new { reply }); await LoadAsync(); }
    }
}