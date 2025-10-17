using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoListPage : ContentPage
{
    private readonly AlunoListViewModel _viewModel;

    public AlunoListPage(AlunoListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Carrega os alunos sempre que a página aparecer
        await _viewModel.LoadAlunosAsync();
    }

    // CORRIGIDO: O nome do método foi atualizado para corresponder ao XAML
    private void OnAlunoEditButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is AlunoDTO aluno)
        {
            _viewModel.EditAlunoCommand.Execute(aluno);
        }
    }

    // CORRIGIDO: O nome do método foi atualizado para corresponder ao XAML
    private void OnAlunoDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is AlunoDTO aluno)
        {
            _viewModel.DeleteAlunoCommand.Execute(aluno);
        }
    }
}