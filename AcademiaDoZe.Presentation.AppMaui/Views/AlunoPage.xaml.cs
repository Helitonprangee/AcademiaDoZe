using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoPage : ContentPage
{
    private readonly AlunoViewModel _viewModel;

    public AlunoPage(AlunoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        // Define o BindingContext para que o XAML saiba de onde pegar os dados e comandos
        BindingContext = _viewModel;
    }

    // Este método é executado toda vez que a página é exibida
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Manda a ViewModel se preparar, carregando um aluno existente ou limpando os campos para um novo
        await _viewModel.InitializeAsync();
    }
}