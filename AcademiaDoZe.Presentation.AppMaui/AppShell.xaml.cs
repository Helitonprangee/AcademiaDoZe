using AcademiaDoZe.Presentation.AppMaui.Views;
namespace AcademiaDoZe.Presentation.AppMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        // O Routing.RegisterRoute é necessário para que o Shell do MAUI reconheça e permita a navegação
        // para páginas que não estão diretamente no Flyout, como páginas de detalhe, edição ou cadastro.
        private static void RegisterRoutes()
        {
            // highlight-start
            // ROTA PARA A PÁGINA DE DETALHES/CADASTRO DE ALUNO QUE ESTAVA FALTANDO
            Routing.RegisterRoute("aluno", typeof(AlunoPage));
            // highlight-end

            Routing.RegisterRoute("logradouro", typeof(LogradouroPage));
            Routing.RegisterRoute("colaborador", typeof(ColaboradorPage));
        }
    }
}