//Heliton

using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.Logging;
using AcademiaDoZe.Presentation.AppMaui.Configuration;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

            // Configurar serviços da aplicação e repositórios
            // -> IMPORTANTE: Verifique se dentro do método ConfigureServices os seus serviços
            // (IAlunoService, IColaboradorService, etc.) estão sendo registrados como Singleton.
            // Ex: services.AddSingleton<IAlunoService, AlunoService>();
            // Isso garante que o mesmo serviço que funciona no Dashboard seja usado na Lista de Alunos.
            ConfigurationHelper.ConfigureServices(builder.Services);

            // --- REGISTRO DE VIEWS E VIEWMODELS ---

            // Dashboard
            builder.Services.AddTransient<DashboardListViewModel>();
            builder.Services.AddTransient<DashboardListPage>();

            // highlight-start
            // Alunos (ESTAVAM FALTANDO)
            builder.Services.AddTransient<AlunoListViewModel>();
            builder.Services.AddTransient<AlunoViewModel>();
            builder.Services.AddTransient<AlunoListPage>();
            builder.Services.AddTransient<AlunoPage>();
            // highlight-end

            // Colaboradores
            builder.Services.AddTransient<ColaboradorListViewModel>();
            builder.Services.AddTransient<ColaboradorViewModel>();
            builder.Services.AddTransient<ColaboradorListPage>();
            builder.Services.AddTransient<ColaboradorPage>();

            // Logradouros
            builder.Services.AddTransient<LogradouroListViewModel>();
            builder.Services.AddTransient<LogradouroViewModel>();
            builder.Services.AddTransient<LogradouroListPage>();
            builder.Services.AddTransient<LogradouroPage>();

            // Outras Páginas
            builder.Services.AddTransient<ConfigPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}