# Desafio de programação

Esta aplicação foi desenvolvida como um desafio de programação para o processo seletivo do banco BTG
Um vídeo mostrando o código e o uso da aplicação está disponível em https://youtu.be/WAmU8vjPtEg

*Desafio*: Elaborar uma solução simples onde duas aplicações se comuniquem por meio de filas (Kafka, Rabbit, mqqt, …).
A aplicação 1 é um servidor http que recebe requests rest e encaminha eles para a aplicação 2.
A aplicação 2 é um console application que recebe os requests da aplicação 1 e devolve a resposta para ela.

*Premissas*:
A aplicação 1 deve segurar o request http até ter a resposta da aplicação 2 para devolver a resposta para o cliente http.


# Instalação e preparo do ambiente

## Rabbit MQ

Para utilização dessa aplicação, é necessário possuir um servidor do RabbitMQ. Recomendo a criação de um container com o aplicativo por simplicidade.
A forma mais fácil de descobri de baixar e gerenciar containeres é por meio do *Docker Desktop*, que pode ser adiquirido aqui: https://www.docker.com/products/docker-desktop/
Com seu docker desktop instalado, reinicie seu computador (para atualizar variáveis de path) e execute
  `docker run -d --hostname rmq --name rabbit-server -p 8081:15672 -p 5672:5672 rabbitmq:4-management`
em um terminal no seu computador. Isso vai automaticamente baixar a imagem correta e mapear as portas do servidor de mensageria.
Também é possível inicializar um container com rabbitMQ diretamente pelo Docker Desktop. Recomendo que seja utilizadoa uma imagem de "management" do rabbitMQ para podermos ver o que está acontecendo no servidor.

## Visual Studio

O uso do Visual Studio é recomendado para compular e executar essa aplicação. Basta abrir o arquivo *SimpleApplication.sln* para visualizar todos os arquivos e executar a aplicação.

## Compilador do C# para linux

Para utilização da aplicação em Ubuntu, serão necessárioas alguns passos extras. Baixe esse repositório manualmente, utilizando as funcionalidades do próprio github (baixar em zip), ou baixar utilizando o **git**.

Para baixar o repositório utilizando o **git**, deve-se instalar o programa em sua máquina ubuntu. Para isso, use `sudo apt install git`. Caso não consiga, busque auxílio na internet.

Com o **git** instalado, deve-se instalar o compilador de C#. Em um terminal, digite:

`sudo apt-get update`

`sudo apt-get install -y dotnet-sdk-8.0`

Note que este projeto utiliza .NET 8.0! Esta forma de instalação é recomendada para a versão mais nova LTS do ubuntu no momento (22.04), e é possível que não funcione em versões mais antigas.

Para compilar o servidor, entre na pasta do servidor por um terminal com cd dateServer e utiize o comando dotnet build para compilar. Com a compilação feita, execute o programa com dotnet run --project dateServer.csproj.

Compilar e executar o cliente é um processo identico, mas realizado na pasta do cliente. entre na pasta do cliente com cd dateClient e utiize o comando dotnet build para compilar. Com a compilação feita, execute o programa com dotnet run --project dateClient.csproj.


# Uso da aplicação

Com a ambas as aplicações rodando (cliente e servidor), deve-se acessar a pagina inicial do swagger. Faça isso por um navegador através do link http://localhost:8080/swagger/index.html
Lá, basta executar a função de perguntar a data.

Você pode também verificar suas filas de mensagens acessando http://localhost:8081. Isso irá te levar para a página de gerenciamento do rabbit MQ, onde o usuário é *guest* e a senha também.

Note que isso sósfuncionará se você utilizar a imagem de "management" do rabbitMQ, e tiver mapeado corretamente as portas, como instruido nos passos acima.
