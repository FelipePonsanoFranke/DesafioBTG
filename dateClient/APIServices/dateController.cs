using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatTimeIsItRightNow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class dateController : ControllerBase     
    {
        private IModel requestChannel = GlobalVariables.requestChannel;
        private string requestExchangeName = GlobalVariables.requestExchangeName;
        private string requestRoutingKey = GlobalVariables.requestRoutingKey;

        private IModel answerChannel = GlobalVariables.answerChannel;
        private string answerQueueName = GlobalVariables.answerQueueName;
        private string answerRoutingKey = GlobalVariables.answerRoutingKey;


        [HttpPost("AskTheDate")]
        public async Task<string> AskTheDate()
        {
            //crio a mensagem e a envio
            string message = "What day is today?";
            string response = String.Empty;

            //crio o listener de mensagens
            var consumer = new EventingBasicConsumer(answerChannel);
            //declaro de onde consumir mensagens, e pego o identificador
            string consumerTag = answerChannel.BasicConsume(answerQueueName, false, consumer);

            //declaro a função que vai rodar toda vez que uma mensagem chegar pelo canal de requisições
            consumer.Received += (sender, args) =>
            {
                //recebemos a resposta do servidor
                response = Encoding.UTF8.GetString(args.Body.ToArray());
                answerChannel.BasicAck(args.DeliveryTag, false);
                answerChannel.BasicCancel(consumerTag);
            };

            requestChannel.BasicPublish(requestExchangeName, requestRoutingKey, null, Encoding.UTF8.GetBytes(message));

            while (String.IsNullOrEmpty(response))
                Task.Delay(1000);//espero a mensagem do servidor chegar

            return response;
        }

    }
}
