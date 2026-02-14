using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class TrainingQuizViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<QuizQuestion> _questions = new();
    [ObservableProperty] private int _currentQuestionIndex;
    [ObservableProperty] private QuizQuestion? _currentQuestion;
    [ObservableProperty] private bool _quizCompleted;
    [ObservableProperty] private bool _quizPassed;
    [ObservableProperty] private int _correctAnswers;
    [ObservableProperty] private int _totalQuestions;
    [ObservableProperty] private double _scorePercent;

    public TrainingQuizViewModel(ApiService api)
    {
        _api = api;
        Title = "Quiz de certification";
    }

    [RelayCommand]
    private async Task LoadQuizAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<List<QuizQuestion>>>(AppConstants.DelivererTrainingQuizEndpoint);
            if (result?.Data != null)
            {
                Questions = new ObservableCollection<QuizQuestion>(result.Data);
                TotalQuestions = Questions.Count;
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions.FirstOrDefault();
            }
            else
            {
                // Fallback quiz local
                Questions = new ObservableCollection<QuizQuestion>(GetLocalQuiz());
                TotalQuestions = Questions.Count;
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions.FirstOrDefault();
            }
        });
    }

    [RelayCommand]
    private void SelectAnswer(int index)
    {
        if (CurrentQuestion != null)
        {
            CurrentQuestion.SelectedIndex = index;
            OnPropertyChanged(nameof(CurrentQuestion));
        }
    }

    [RelayCommand]
    private void NextQuestion()
    {
        if (CurrentQuestion?.SelectedIndex == null)
        {
            ErrorMessage = "Sélectionnez une réponse";
            HasError = true;
            return;
        }

        HasError = false;
        if (CurrentQuestionIndex < Questions.Count - 1)
        {
            CurrentQuestionIndex++;
            CurrentQuestion = Questions[CurrentQuestionIndex];
        }
    }

    [RelayCommand]
    private void PreviousQuestion()
    {
        if (CurrentQuestionIndex > 0)
        {
            CurrentQuestionIndex--;
            CurrentQuestion = Questions[CurrentQuestionIndex];
        }
    }

    [RelayCommand]
    private async Task SubmitQuizAsync()
    {
        if (Questions.Any(q => q.SelectedIndex == null))
        {
            ErrorMessage = "Répondez à toutes les questions";
            HasError = true;
            return;
        }

        await ExecuteAsync(async () =>
        {
            CorrectAnswers = Questions.Count(q => q.IsCorrect);
            ScorePercent = (double)CorrectAnswers / TotalQuestions * 100;
            QuizPassed = ScorePercent >= 70;
            QuizCompleted = true;

            var answers = Questions.Select(q => new { q.Id, Answer = q.SelectedIndex }).ToList();
            await _api.PostAsync<ApiResponse<QuizResult>>(
                AppConstants.DelivererTrainingQuizEndpoint, new { Answers = answers });

            if (QuizPassed)
            {
                await Task.Delay(2000);
                await Shell.Current.GoToAsync("//pendingVerification");
            }
        });
    }

    [RelayCommand]
    private async Task RetryQuizAsync()
    {
        QuizCompleted = false;
        foreach (var q in Questions) q.SelectedIndex = null;
        CurrentQuestionIndex = 0;
        CurrentQuestion = Questions.FirstOrDefault();
    }

    private static List<QuizQuestion> GetLocalQuiz() => new()
    {
        new() { Id = 1, Question = "Que faire en arrivant au restaurant ?", Options = new() { "Klaxonner", "Appeler le restaurant", "Confirmer dans l'app 'Arrivé au retrait'", "Attendre dehors" }, CorrectIndex = 2 },
        new() { Id = 2, Question = "Comment vérifier une commande ?", Options = new() { "Ouvrir le sac", "Compter les articles dans l'app", "Demander au client", "Rien, le restaurant gère" }, CorrectIndex = 1 },
        new() { Id = 3, Question = "Client absent, que faire ?", Options = new() { "Partir avec la commande", "Appeler le client 2 fois puis signaler", "Laisser devant la porte", "Annuler la commande" }, CorrectIndex = 1 },
        new() { Id = 4, Question = "En cas d'accident, première action ?", Options = new() { "Continuer la livraison", "Appuyer sur le bouton SOS", "Appeler un ami", "Rien signaler" }, CorrectIndex = 1 },
        new() { Id = 5, Question = "Pourquoi la photo de livraison ?", Options = new() { "Pour les réseaux sociaux", "Comme preuve de livraison", "Pour le fun", "C'est optionnel" }, CorrectIndex = 1 },
        new() { Id = 6, Question = "À quelle fréquence mettre à jour sa position ?", Options = new() { "Jamais", "Toutes les heures", "L'app le fait automatiquement", "Une fois par jour" }, CorrectIndex = 2 },
        new() { Id = 7, Question = "Comment gagner des bonus ?", Options = new() { "Livrer aux heures de pointe", "Refuser les commandes loin", "Travailler une fois par semaine", "Aucune idée" }, CorrectIndex = 0 },
        new() { Id = 8, Question = "Temps max pour accepter une commande ?", Options = new() { "1 heure", "30 secondes", "60 secondes", "5 minutes" }, CorrectIndex = 2 },
        new() { Id = 9, Question = "Produits endommagés durant le transport ?", Options = new() { "Les cacher", "Signaler dans l'app avec photo", "Les remplacer", "Ignorer" }, CorrectIndex = 1 },
        new() { Id = 10, Question = "Le port du casque est ?", Options = new() { "Optionnel", "Obligatoire pour motos/scooters", "Que pour les vélos", "Jamais requis" }, CorrectIndex = 1 }
    };
}