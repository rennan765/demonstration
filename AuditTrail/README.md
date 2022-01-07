# Demonstration - AuditTrail

Solução para logs de erros e informações - Trilha de Auditoria
<hr />

O processamento da trilha de auditoria trata-se de uma função Lambda que é disparada através de uma mensagem em uma fila SQS.
Ao receber esta mensagem, o Lambda valida os campos recebidos e insere esta mensagem validada em uma tabela no DynamoDb.
<hr />

Esta solução conta com:

1. Um CrossCutting para incluir a mensagem SQS na fila; 
2. Uma API REST com um POST para inserir mensagens na fila (utilizando o CrossCutting)
3. Função Lambda que faz a leitura da fila e a inserção no DynamoDb.
<hr />

A API REST utiliza a interface do Swagger para facilitar a execução.
Ao iniciar o projeto, acesse a página http://localhost:{porta}/swagger para abrir a interface do swagger.
Caso queira abrir a interface do swagger minimizada, acessar http://localhost:{porta}/swagger/index.html?docExpansion=none.
<hr />

Para debugar o Lambda, é necessário a instalação do pacote Lambda Test Tool (vide https://github.com/aws/aws-lambda-dotnet/tree/master/Tools/LambdaTestTool).
Também é necessária a instalação do .NET Core SDK 3.1 (vide https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-3.1.416-windows-x64-installer).
<hr />

Os projetos utilizas variáveis de ambiente para o funcionamento. 
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
</table>
