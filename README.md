# Demosntration

Solução de demonstração em NET 6 com a infra da AWS.
<hr />

Esta Solução possui:

1. Contexto para cadastro de pessoas
2. Contexto de trilha de auditoria
3. Contexto para manutenção dos contatos

A execução destes flui da seguinte forma:

1. O contexto de cadastro de pessoas é uma API REST responsável pelo cadastro de pessoas utilizando informações básicas.
Qualquer pessoa pode efetuar um cadastro nesta API.
A pessoa pode efetuar o seu login (gerando um token JWT) e alterar o próprio cadastro, desde que esteja passando o JTW no header da requisição.
Após a criação/alteração, uma mensagem é enviada para uma fila SQS para a mannutenção dos dados de contato da pessoa criada.

2. O contexto de trilha de auditoria é uma função Lambda que recebe uma mensagem de uma fla SQS e insere essas informações no DynamoDB.
Todos os contextos da solução mandam mensagens para uma fila SQS, podendo ser esta uma informação, aviso ou o log de um erro, visando auditoria posterior.

3. O contexto para manutenção de contatos é um Worker Services que fica fazendo a leitura da fila SQS informada no contexto de cadastro de pessoas.
Ao receber esta mensagem, o Qorker Services obtém o cadastro da pessoa informada na mensagem e remove os dados duplicados de telefone e endereço desta.

<hr />

As pastas de cada um dos contextos contém instruções para execução da solução.