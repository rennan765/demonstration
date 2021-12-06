# Demonstration - Person

Worker Services para manutenção dos dados de contato da pessoa
<hr />

Para debugar esta solução, restaure os pacotes nuget e execute-o.
<hr />

O Worker Service roda em um intervalo pequeno, efetuando a leitura de uma fila SQS. Caso esta esteja vazia, la loga no console uma mensagem de fila vazia de hora em hora, o qual pode ser utilizada como um keep alive.
<hr />

Este projeto utiliza variáveis de ambiente para o funcionamento. 
Para que o mesmo execute normalmente, será necessário inserir as variáveis abaixo: 

<table>
  <theader>
    <td>Nome</td>
    <td>Descrição</td>
    <td>Conteúdo de exemplo</td>
  </theader>
  <tr>
    <td>ASPNETCORE_ENVIRONMENT</td>
    <td>Ambiente em que a aplicação está sendo executada</td>
    <td>Local</td>
  </tr>
  <tr>
    <td>DOTNET_ENVIRONMENT</td>
    <td>Ambiente em que a aplicação está sendo executada</td>
    <td>Local</td>
  </tr>
  <tr>
    <td>RelationalDatabaseConnectionString</td>
    <td>Connection String do banco de dados relacional (PostgreSQL)</td>
    <td>User ID=user;Password=password;Host=host;Port=5432;Database=database;</td>
  </tr>
  <tr>
    <td>AwsAccessKeyId</td>
    <td>Chave de acesso da conta da AWS</td>
    <td>OQNRLFWMFLCCGFJMGETU</td>
  </tr>
  <tr>
    <td>AwsSecretKey</td>
    <td>Segredo da chave de acesso da conta da AWS</td>
    <td>fdjsa8e3/8HcsUVxaQB8QUFnRPfa5PhEVK2U+qIf</td>
  </tr>
  <tr>
    <td>AwsRegion</td>
    <td>Região dos serviços AWS</td>
    <td>sa-east-1</td>
  </tr>
  <tr>
    <td>ContactQueueUrl</td>
    <td>Fila para leitura da pessoa adicionada/alterada</td>
    <td>https://sqs.sa-east-1.amazonaws.com/257893541578/PERSON</td>
  </tr>
</table>

<hr/>

Os dados de connection string ficam salvos em secret managers do AWS. Desta forma, caso a aplicação esteja sendo executada no ambiente, será resgatado o valor do secret.
A configuração do secret é desta forma:

<table>
  <theader>
    <td>Nome do secret</td>
    <td>Conteúdo do secret</td>
  </theader>
  <tr>
    <td>RelationalDatabaseConnectionString</td>
    <td>User ID=user;Password=password;Host=host;Port=5432;Database=database;</td>
  </tr>
</table>