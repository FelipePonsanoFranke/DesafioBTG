using System.Globalization;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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


var consumer = new EventingBasicConsumer(requestChannel);

//pego as informações necessárias para fechar a fila de mansagens de forma correta
//também informo ao consumidor que quero consumir mensagens da fila de requisições
string consumerTag = requestChannel.BasicConsume(requestQueueName, false, consumer);


//declaro a função que vai rodar toda vez que uma mensagem chegar pelo canal de requisições
consumer.Received += (sender, args) =>
{
    //a mensagem vem como um vetor de bytes. Eu poderia transitar um json com os dados de uma entidade C#, e deserializa-los
    //como é uma aplicação simples, verificarei apenas se o cliente está perguntando que dia é hoje
    string message = Encoding.UTF8.GetString(args.Body.ToArray());
    if(message == "What day is today?")
    {
        
        //criamos a resposta
        DateTime date = DateTime.Now;
        string response = "It is " + date.Day.ToString() + " of " + date.ToString("MMMM", new CultureInfo("en-US")) + " of " + date.Year.ToString();

        //respondo a mensagem
        answerChannel.BasicPublish(answerExchangeName, answerRoutingKey, null, Encoding.UTF8.GetBytes(response));

        //reconhecemos a leitura da mensagem
        requestChannel.BasicAck(args.DeliveryTag, false);
    }


};



//mantenho o servidor aberto até que seja pressionado enter
Console.ReadLine();

requestChannel.BasicCancel(consumerTag);
requestChannel.Close();
connection.Close();