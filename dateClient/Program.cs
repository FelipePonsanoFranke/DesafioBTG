/*(Desafio:
Elaborar uma solução simples onde duas aplicações se comuniquem por meio de filas (Kafka, Rabbit, mqqt, …).
A aplicação 1 é um servidor http que recebe requests rest e encaminha eles para a aplicação 2.
A aplicação 2 é um console application que recebe os requests da aplicação 1 e devolve a resposta para ela.

Premissas:
A aplicação 1 deve segurar o request http até ter a resposta da aplicação 2 para devolver a resposta para o cliente http.
Qualquer dúvida sobre o desafio, pode me passar aqui ou enviar para o número do José: (11) 3383 - 2361

(Entrega até 24/10)*/





using RabbitMQ.Client;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;



//inicializando a conexão com o servidor de mensageria
ConnectionFactory factory = new ConnectionFactory();

//a senha, usuário e outras informações como chaves de roteamento deveriam estar em um JSON
//estou fazendo uma senha hard coded por simplicidade da aplicação
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Date Server";

IConnection connection = factory.CreateConnection();

IModel requestChannel = connection.CreateModel();
IModel answerChannel = connection.CreateModel();

//criarei dois canais para troca de mensagens.
//um dos canais receberá os pedidos por informação e o outro irá responder ao pedido.
string requestExchangeName = "requestExchange";
string requestRoutingKey = "request-routing-key";
string requestQueueName = "request-queue";

string answerExchangeName = "answerExchange";
string answerRoutingKey = "answer-routing-key";
string answerQueueName = "answer-queue";

requestChannel.ExchangeDeclare(requestExchangeName, ExchangeType.Direct);
requestChannel.QueueDeclare(requestQueueName, false, false, false, null);
requestChannel.QueueBind(requestQueueName, requestExchangeName, requestRoutingKey, null);

answerChannel.ExchangeDeclare(answerExchangeName, ExchangeType.Direct);
answerChannel.QueueDeclare(answerQueueName, false, false, false, null);
answerChannel.QueueBind(answerQueueName, answerExchangeName, answerRoutingKey, null);

requestChannel.BasicQos(0, 1, false);
answerChannel.BasicQos(0, 1, false);

GlobalVariables.requestChannel = requestChannel;//coloco as informações importantes nas variaveis globais para minha API utilizar
GlobalVariables.requestExchangeName= requestExchangeName;
GlobalVariables.requestRoutingKey= requestRoutingKey;

GlobalVariables.answerChannel = answerChannel;
GlobalVariables.answerQueueName = answerQueueName;
GlobalVariables.answerRoutingKey = answerRoutingKey;

//agora crio a API
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(8080, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1); // Para REST API
});

var app = builder.Build();

// Configura o pipeline de requisição HTTP.
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();





