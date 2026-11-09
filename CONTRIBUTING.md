# Como contribuir com o MiniBitcoin

Este guia orienta a manutenção do projeto. O [README](README.md) descreve a configuração com .NET 10, o menu e as regras de negócio.

## Comece a mudança

1. Consulte as issues e os pull requests abertos para verificar se alguém já está trabalhando no assunto. Ao registrar um erro, inclua como reproduzi-lo, o resultado obtido e o esperado.
2. Crie uma branch a partir da `main` atualizada. Quem contribui de fora pode criar um fork, sincronizar a `main` dele com o projeto original e trabalhar em uma branch do fork. [Como abrir um pull request a partir de um fork](https://docs.github.com/pt/pull-requests/how-tos/create-pull-requests/creating-a-pull-request-from-a-fork)
3. Concentre a branch em uma correção ou melhoria. Se outra pessoa assumir a tarefa, registre na issue ou no pull request o que já foi feito, o que falta e como reproduzir o estado atual.

```bash
git switch main
git pull --ff-only
git switch -c fix/extrato
```

## Implemente e valide

- Procure as regras de negócio em `MiniBitcoin.Console/Domain/`, a interação com o usuário em `MiniBitcoin.Console/ConsoleMenu.cs` e os testes em `MiniBitcoin.Tests/`.
- Para corrigir um erro, comece com um teste xUnit que falhe pelo motivo esperado. Aplique a correção, confirme que o teste passou e execute a suíte completa. Para uma funcionalidade nova, use o mesmo ciclo com o comportamento esperado.
- Em mudanças de negociação, confira a precisão de BRL e BTC, o arredondamento de compra e venda e se uma ordem recusada preserva os dois saldos e o extrato. O extrato guarda a cotação da ordem. O patrimônio usa a cotação atual.
- Em mudanças no menu, teste o caminho afetado, uma entrada inválida e o encerramento da entrada quando forem relevantes.

Na raiz do projeto, execute a suíte em Release. Se a mudança afetar o menu, execute também o aplicativo e reproduza o fluxo alterado.

```bash
dotnet test MiniBitcoin.sln -c Release
dotnet run --project MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

## Envie para revisão

Use commits pequenos com mensagens no padrão Conventional Commits. Exemplos para este projeto incluem `fix(extrato): preserve the executed quote`, `test(carteira): cover an insufficient balance` e `docs: clarify how to run tests`. [Especificação do Conventional Commits](https://www.conventionalcommits.org/pt-br/v1.0.0/)

Envie a branch com `git push -u origin fix/extrato` e abra um pull request para a `main` do repositório original. Na descrição, informe o problema ou objetivo, o que mudou e como você validou a alteração. Vincule a issue correspondente, quando existir.

Antes de integrar a mudança, confira se o pull request atende ao que foi proposto, se os testes passam e se o README reflete qualquer alteração nos comandos ou nas regras. Quando houver outro colaborador disponível, peça que ele revise a mudança.
