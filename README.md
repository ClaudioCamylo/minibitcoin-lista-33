# MiniBitcoin · Lista 33

Simulador de carteira Bitcoin para a atividade de Git, Conventional Commits e TDD. O aplicativo de console usa .NET 10 e a suíte usa xUnit.

## Como executar

Com o SDK do .NET 10 instalado, execute os comandos na raiz da solução.

```bash
dotnet restore MiniBitcoin.sln
dotnet test MiniBitcoin.sln
dotnet run --project MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

A carteira começa com R$ 1.000,00 e 0 BTC. A cotação inicial simulada é R$ 500.000,00 por BTC. A aplicação guarda os dados em memória durante a sessão.

| Opção | Operação |
| --- | --- |
| 1 | Consultar a cotação atual em BRL por BTC |
| 2 | Ver os saldos em BRL e BTC |
| 3 | Comprar uma quantidade de BTC |
| 4 | Vender uma quantidade de BTC |
| 5 | Exibir o extrato e o patrimônio consolidado |
| 6 | Atualizar a cotação simulada |
| 0 | Sair |

Digite valores com vírgula ou ponto decimal, sem separador de milhar. Para experimentar, compre `0,001` BTC, atualize a cotação para `600000` e venda `0,0005` BTC. O extrato mostrará as duas ordens. O saldo ficará em R$ 800,00 e 0,0005 BTC, com patrimônio de R$ 1.100,00 pela nova cotação.

## Regras

- A carteira usa `decimal` com até duas casas em BRL e oito em BTC. Um satoshi equivale a 0,00000001 BTC. Valores iniciais podem ser zero. Ordens, débitos e cotação precisam ser positivos.
- O total de uma ordem é a quantidade de BTC multiplicada pela cotação do momento. A compra arredonda o custo para cima e a venda arredonda o crédito para baixo, sempre ao centavo. A venda exige crédito de pelo menos R$ 0,01.
- Uma ordem recusada por saldo insuficiente, quantidade inválida ou limite numérico preserva os dois saldos e o extrato. Cálculos em BRL acima de 7 quintilhões excedem a faixa definida para preservar a precisão do produto.
- O extrato lista as ordens executadas em sequência, com horário UTC, tipo, quantidade, cotação da ordem e total liquidado em BRL.
- O patrimônio é o saldo em reais mais o saldo em BTC multiplicado pela cotação atual. O total é arredondado ao centavo mais próximo e, em empate, ao centavo par.

## Organização

- `MiniBitcoin.Console/Domain/` contém a carteira, a negociação, as regras de precisão e o modelo de transação.
- `MiniBitcoin.Console/Program.cs` configura a sessão e inicia o menu. `ConsoleMenu.cs` recebe os fluxos de entrada e saída para permitir testes automatizados.
- `MiniBitcoin.Tests/` cobre carteira, negociação, extrato, patrimônio e interação com o menu.

## Contribuição

Veja o [guia de contribuição](CONTRIBUTING.md) para corrigir erros, validar mudanças e enviar um pull request.

## Histórico de desenvolvimento

Os commits `test(...)` registram a fase em que a suíte da etapa falhava, e os commits `feat(...)` seguintes registram a implementação validada. A ponta de `main` contém a suíte completa. Confira a sequência com `git log --oneline --reverse`.

A entrega compactada também inclui `MiniBitcoin.bundle`. Após extrair o ZIP, você pode recuperar o repositório Git completo a partir da pasta que contém o bundle.

```bash
git clone MiniBitcoin.bundle MiniBitcoin-com-historico
```
