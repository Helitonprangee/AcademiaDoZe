using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class AlunoListViewModel : BaseViewModel
    {
        private readonly IAlunoService _alunoService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private ObservableCollection<AlunoDTO> _alunos = new();
        public ObservableCollection<AlunoDTO> Alunos
        {
            get => _alunos;
            set => SetProperty(ref _alunos, value);
        }

        public AlunoListViewModel(IAlunoService alunoService)
        {
            _alunoService = alunoService;
            Title = "Alunos";
        }

        [RelayCommand]
        private async Task AddAlunoAsync()
        {
            try
            {
                // Navega para a página de detalhes do aluno (rota "aluno")
                await Shell.Current.GoToAsync("aluno");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar para tela de cadastro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditAlunoAsync(AlunoDTO aluno)
        {
            try
            {
                if (aluno == null)
                    return;
                // Navega para a página de detalhes passando o Id do aluno
                await Shell.Current.GoToAsync($"aluno?Id={aluno.Id}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar para tela de edição: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadAlunosAsync();
        }

        [RelayCommand]
        private async Task SearchAlunosAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await MainThread.InvokeOnMainThreadAsync(() => Alunos.Clear());

                // CORREÇÃO: Chamando o método BuscarAsync com apenas um argumento
                var resultados = await _alunoService.BuscarAsync(SearchText);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var item in resultados)
                    {
                        Alunos.Add(item);
                    }
                    OnPropertyChanged(nameof(Alunos));
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao buscar alunos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task LoadAlunosAsync() 
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await MainThread.InvokeOnMainThreadAsync(() => Alunos.Clear());

                // Coloque o breakpoint VERMELHO aqui
                var alunosList = await _alunoService.ObterTodosAsync(); ;
                if (alunosList != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (var aluno in alunosList)
                        {
                            Alunos.Add(aluno);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar alunos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAlunoAsync(AlunoDTO aluno)
        {
            if (aluno == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar Exclusão", $"Deseja realmente excluir o aluno {aluno.Nome}?", "Sim", "Não");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                bool success = await _alunoService.RemoverAsync(aluno.Id);
                if (success)
                {
                    Alunos.Remove(aluno);
                    await Shell.Current.DisplayAlert("Sucesso", "Aluno excluído com sucesso!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível excluir o aluno.", "OK");

                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao excluir aluno: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}