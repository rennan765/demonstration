# demonstration

Solução de demonstração em NET 6 com a infra da AWS para o cadastro de pessoas.
<hr />

Para debugar esta solução, restaure os pacotes nuget e execute-o.
Acesse a página http://localhost:{porta}/swagger para abrir a interface do swagger.
Caso queira abrir a interface do swagger minimizada, acessar http://localhost:{porta}/swagger/index.html?docExpansion=none.
<hr />

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
    <td>Issuer</td>
    <td>Issuer do JWT</td>
    <td>30bce1bb</td>
  </tr>
  <tr>
    <td>Audience</td>
    <td>Audience do JWT</td>
    <td>2603aff38a29</td>
  </tr>
  <tr>
    <td>SecretKey</td>
    <td>Secret key do JWT (normalmente um Guid)</td>
    <td>c517a6fc-cefd-43b9-a1f5-40a551b7aecc</td>
  </tr>
  <tr>
    <td>Time</td>
    <td>Tempo de vida do JWT (em segundos)</td>
    <td>900</td>
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
    <td>sa-east-1</td>
    <td>fdjsa8e3/8HcsUVxaQB8QUFnRPfa5PhEVK2U+qIf</td>
  </tr>
</table>

<hr/>

Os dados de connection string e de refresh token ficam salvos em secret managers do AWS. Desta forma, caso a aplicação esteja sendo executada no ambiente, será resgatado o valor do secret.
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
  <tr>
    <td>TokenConfig</td>
    <td>{ "Issuer": "30bce1bb", "Audience": "2603aff38a29", "SecretKey": "c517a6fc-cefd-43b9-a1f5-40a551b7aecc", "TokenTime": "900" }</td>
  </tr>
  <tr>
  </tr>
</table>