//Heliton
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    [Fact]
    public async Task Aluno_LogradouroPorId_CpfJaExiste_Adicionar()
    {
        var logradouroId = 4;
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(logradouroId);

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // CPF válido de teste (11 dígitos)
        var _cpf = "12345678909";

        var repoAlunoCpf = new AlunoRepository(ConnectionString, DatabaseType);

        // Remove aluno com esse CPF caso exista
        var alunoExistenteBanco = await repoAlunoCpf.ObterPorCpf(_cpf);
        if (alunoExistenteBanco != null)
            await repoAlunoCpf.Remover(alunoExistenteBanco.Id);

        var cpfExistente = await repoAlunoCpf.CpfJaExiste(_cpf);
        Assert.False(cpfExistente, "CPF já existe no banco de dados.");

        var aluno = Aluno.Criar(
            1,
            "Aluno Teste",
            _cpf,
            new DateOnly(1990, 10, 10),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "complemento casa",
            "Senha@123",
            arquivo
        );

        var repoAlunoAdicionar = new AlunoRepository(ConnectionString, DatabaseType);
        var alunoInserido = await repoAlunoAdicionar.Adicionar(aluno);
        Assert.NotNull(alunoInserido);
        Assert.True(alunoInserido.Id > 0);
    }



    [Fact]
    public async Task Aluno_ObterPorCpf_Atualizar()
    {
        var cpfTeste = "13948678901";
        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(1); // ID válido

        // Tenta obter aluno; se não existir, cria
        var alunoExistente = await repoAluno.ObterPorCpf(cpfTeste);
        if (alunoExistente == null)
        {
            var novoAluno = Aluno.Criar(
                1,
                "Aluno Atualizar",
                cpfTeste,
                DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), // idade válida
                "49999999999",
                "teste_atualizar@email.com",
                logradouro!,
                "123",
                "",
                "Senha@123",
                arquivo
            );

            // Adiciona ao banco e garante que retornou um objeto válido
            alunoExistente = await repoAluno.Adicionar(novoAluno)
                              ?? throw new Exception("Falha ao inserir aluno para teste.");
        }

        // Act: cria o objeto atualizado
        var alunoAtualizado = Aluno.Criar(
            alunoExistente.Id,
            "Aluno Atualizado",
            alunoExistente.Cpf,
            alunoExistente.DataNascimento,
            alunoExistente.Telefone,
            alunoExistente.Email,
            alunoExistente.Endereco,
            alunoExistente.Numero,
            alunoExistente.Complemento,
            alunoExistente.Senha,
            arquivo
        );

        var idProperty = typeof(Entity).GetProperty("Id");
        idProperty?.SetValue(alunoAtualizado, alunoExistente.Id);

        var resultadoAtualizacao = await repoAluno.Atualizar(alunoAtualizado);

        // Assert
        Assert.NotNull(resultadoAtualizacao);
        Assert.Equal("Aluno Atualizado", resultadoAtualizacao.Nome);
    }


    [Fact]
    public async Task Aluno_TrocarSenha()
    {
        var cpfTeste = "12345088991";
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(1); // ID válido

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // Verifica se o aluno já existe; se não, cria
        var alunoExistente = await repoAluno.ObterPorCpf(cpfTeste);
        if (alunoExistente == null)
        {
            var novoAluno = Aluno.Criar(
                1,
                "Aluno Troca Senha",
                cpfTeste,
                DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), // idade válida
                "49999999999",
                "teste_trocasenha@email.com",
                logradouro!,
                "123",
                "",
                "Senha@123",
                arquivo
            );

            alunoExistente = await repoAluno.Adicionar(novoAluno);
        }

        // Act: troca a senha
        var novaSenha = "NovaSenha123";
        var resultadoTroca = await repoAluno.TrocarSenha(alunoExistente.Id, novaSenha);

        // Assert
        Assert.True(resultadoTroca);

        var alunoAtualizado = await repoAluno.ObterPorId(alunoExistente.Id);
        Assert.NotNull(alunoAtualizado);
        Assert.Equal(novaSenha, alunoAtualizado.Senha);
    }


    [Fact]
    public async Task Aluno_Remover_ObterPorId()
    {
        var cpfTeste = "33346678992";
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(1); // ID válido

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // Verifica se o aluno já existe; se não, cria
        var alunoExistente = await repoAluno.ObterPorCpf(cpfTeste);
        if (alunoExistente == null)
        {
            var novoAluno = Aluno.Criar(
                1,
                "Aluno Remover",
                cpfTeste,
                DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), // idade válida
                "49999999999",
                "teste_remover@email.com",
                logradouro!,
                "123",
                "",
                "Senha@123",
                arquivo
            );

            alunoExistente = await repoAluno.Adicionar(novoAluno);
        }

        // Act: remover
        var resultadoRemover = await repoAluno.Remover(alunoExistente.Id);

        // Assert
        Assert.True(resultadoRemover);

        var alunoRemovido = await repoAluno.ObterPorId(alunoExistente.Id);
        Assert.Null(alunoRemovido);
    }



    [Fact]
    public async Task Aluno_ObterTodos()
    {
        // Arrange
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(1); // id válido do logradouro
        Assert.NotNull(logradouro);

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // Gera CPF válido
        var cpfUnico = GerarCpfValido();

        // Remove aluno com mesmo CPF se existir
        var alunoExistenteBanco = await repoAluno.ObterPorCpf(cpfUnico);
        if (alunoExistenteBanco != null)
            await repoAluno.Remover(alunoExistenteBanco.Id);

        // Cria aluno
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste ObterTodos",
            cpfUnico,
            DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), // idade válida
            "49999999999",
            "teste_obtertodos@email.com",
            logradouro!,
            "123",
            "",
            "Senha@123",
            arquivo
        );

        await repoAluno.Adicionar(aluno);

        // Act
        var resultado = await repoAluno.ObterTodos();

        // Assert
        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado);
    }

    // Função auxiliar para gerar CPF válido
    private string GerarCpfValido()
    {
        var random = new Random();
        int[] numeros = new int[9];
        for (int i = 0; i < 9; i++)
            numeros[i] = random.Next(0, 10);

        // Calcula primeiro dígito verificador
        int soma = 0;
        for (int i = 0, peso = 10; i < 9; i++, peso--)
            soma += numeros[i] * peso;
        int resto = soma % 11;
        int digito1 = (resto < 2) ? 0 : 11 - resto;

        // Calcula segundo dígito verificador
        soma = 0;
        for (int i = 0, peso = 11; i < 9; i++, peso--)
            soma += numeros[i] * peso;
        soma += digito1 * 2;
        resto = soma % 11;
        int digito2 = (resto < 2) ? 0 : 11 - resto;

        return string.Concat(string.Join("", numeros), digito1, digito2);
    }





}