using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GlobalVariables
{
    public static IModel requestChannel { get; set; }

    public static string requestExchangeName { get; set; }

    public static string requestRoutingKey { get; set; }

    public static IModel answerChannel { get; set; }

    public static string answerQueueName { get; set; }

    public static string answerRoutingKey { get; set; }
}
