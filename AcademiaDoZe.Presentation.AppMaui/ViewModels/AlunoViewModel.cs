using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(AlunoId), "Id")]
    public partial class AlunoViewModel : BaseViewModel
    {
        private readonly IAlunoService _alunoService;

        // AJUSTE 1: Definindo o País na inicialização do objeto
        // highlight-start
        private AlunoDTO _aluno = new()
        {
            Nome = string.Empty,
            Cpf = string.Empty,
            DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
            Telefone = string.Empty,
            Email = string.Empty,
            Endereco = new LogradouroDTO
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty,
                Pais = "Brasil" // <- VALOR PADRÃO ADICIONADO AQUI
            },
            Numero = string.Empty
        };
        // highlight-end

        public AlunoDTO Aluno
        {
            get => _aluno;
            set => SetProperty(ref _aluno, value);
        }

        private int _alunoId;
        public int AlunoId
        {
            get => _alunoId;
            set => SetProperty(ref _alunoId, value);
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public AlunoViewModel(IAlunoService alunoService)
        {
            _alunoService = alunoService;
            Title = "Detalhes do Aluno";
        }

        public async Task InitializeAsync()
        {
            if (AlunoId > 0)
            {
                IsEditMode = true;
                Title = "Editar Aluno";
                await LoadAlunoAsync();
            }
            else
            {
                IsEditMode = false;
                Title = "Novo Aluno";

                // AJUSTE 2: Garantindo que o País seja definido ao criar um novo aluno
                // highlight-start
                Aluno = new AlunoDTO()
                {
                    Nome = string.Empty,
                    Cpf = string.Empty,
                    DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
                    Telefone = string.Empty,
                    Email = string.Empty,
                    Endereco = new LogradouroDTO
                    {
                        Cep = string.Empty,
                        Nome = string.Empty,
                        Bairro = string.Empty,
                        Cidade = string.Empty,
                        Estado = string.Empty,
                        Pais = "Brasil" // <- VALOR PADRÃO ADICIONADO AQUI TAMBÉM
                    },
                    Numero = string.Empty
                };
                // highlight-end
            }
        }

        [RelayCommand]
        private async Task SaveAlunoAsync()
        {
            // Opcional: Uma última garantia antes de salvar. As correções acima já devem resolver.
            // if (string.IsNullOrWhiteSpace(Aluno.Endereco.Pais))
            // {
            //     Aluno.Endereco.Pais = "Brasil";
            // }

            if (IsBusy) return;
            if (!ValidateAluno(Aluno)) return;

            try
            {
                IsBusy = true;
                if (IsEditMode)
                {
                    await _alunoService.AtualizarAsync(Aluno);
                    await Shell.Current.DisplayAlert("Sucesso", "Aluno atualizado com sucesso!", "OK");
                }
                else
                {
                    await _alunoService.AdicionarAsync(Aluno);
                    await Shell.Current.DisplayAlert("Sucesso", "Aluno criado com sucesso!", "OK");
                }
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar aluno: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ... O RESTANTE DO SEU CÓDIGO (LoadAlunoAsync, SelecionarFotoAsync, etc.) CONTINUA IGUAL ...
        #region Outros Métodos
        [RelayCommand]
        private async Task LoadAlunoAsync()
        {
            if (AlunoId <= 0) return;
            try
            {
                IsBusy = true;
                var alunoData = await _alunoService.ObterPorIdAsync(AlunoId);
                if (alunoData != null)
                {
                    Aluno = alunoData;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar aluno: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SelecionarFotoAsync()
        {
            try
            {
                string escolha = await Shell.Current.DisplayActionSheet("Origem da Imagem", "Cancelar", null, "Galeria", "Câmera");
                FileResult? result = null;

                if (escolha == "Galeria")
                {
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selecione uma imagem",
                        FileTypes = FilePickerFileType.Images
                    });
                }
                else if (escolha == "Câmera")
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        result = await MediaPicker.Default.CapturePhotoAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erro", "Câmera não suportada neste dispositivo.", "OK");
                        return;
                    }
                }

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);

                    Aluno.Foto = new ArquivoDTO { Conteudo = ms.ToArray() };
                    OnPropertyChanged(nameof(Aluno));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao selecionar imagem: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private static bool ValidateAluno(AlunoDTO aluno)
        {
            const string validationTitle = "Validação";

            if (string.IsNullOrWhiteSpace(aluno.Nome))
            {
                Shell.Current.DisplayAlert(validationTitle, "Nome é obrigatório.", "OK");
                return false;
            }
            if (string.IsNullOrWhiteSpace(aluno.Cpf) || aluno.Cpf.Length != 11)
            {
                Shell.Current.DisplayAlert(validationTitle, "CPF deve ter 11 dígitos.", "OK");
                return false;
            }
            // Adicione outras validações aqui se precisar
            return true;
        }
        #endregion

    }
}