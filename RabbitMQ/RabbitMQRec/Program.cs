using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQRec
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConnectionFactory factory = new ConnectionFactory
            //{
            //    UserName = "admin",//用户名
            //    Password = "admin",//密码
            //    HostName = "127.0.0.1"
            //};
            ////创建连接
            //var connection = factory.CreateConnection();
            ////创建通道
            //var channel = connection.CreateModel();
            ////事件基本消费者
            //EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            ////接收到消息事件
            //consumer.Received += (ch, ea) =>
            //{
            //    var message = Encoding.UTF8.GetString(ea.Body);
            //    Console.WriteLine($"收到消息： {message}");
            //    //确认该消息已被消费
            //    channel.BasicAck(ea.DeliveryTag, false);
            //};

            ////启动消费者 设置为手动应答消息
            //channel.BasicConsume("TestMessage", false, consumer);
            //Console.WriteLine("消费者已启动");
            //Console.ReadKey();
            //channel.Dispose();
            //connection.Close();
            DirectAcceptExchangeTask();
        }
        /// <summary>
        /// 连接配置
        /// </summary>
        private static readonly ConnectionFactory rabbitMqFactory = new ConnectionFactory()
        {
            HostName = "127.0.0.1",
            UserName = "admin",
            Password = "admin"
        };
        /// <summary>
        /// 路由名称
        /// </summary>
        const string ExchangeName = "justin.exchange";

        //队列名称
        const string QueueName = "justin.queue";
        public static void DirectAcceptExchangeTask()
        {
            using (IConnection conn = rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //channel.ExchangeDeclare(ExchangeName, "direct", durable: true, autoDelete: false, arguments: null);
                    channel.QueueDeclare(QueueName, durable: true, autoDelete: false, exclusive: false, arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);//告诉broker同一时间只处理一个消息
                                                                                       //channel.QueueBind(QueueName, ExchangeName, routingKey: QueueName);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var msgBody = Encoding.UTF8.GetString(ea.Body);
                        Console.WriteLine(string.Format("***接收时间:{0}，消息内容：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msgBody));
                        int dots = msgBody.Split('.').Length - 1;
                        System.Threading.Thread.Sleep(dots * 1000);
                        Console.WriteLine(" [x] Done");
                        //处理完成，告诉Broker可以服务端可以删除消息，分配新的消息过来
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    //noAck设置false,告诉broker，发送消息之后，消息暂时不要删除，等消费者处理完成再说
                    channel.BasicConsume(QueueName, noAck: false, consumer: consumer);

                    Console.WriteLine("按任意值，退出程序");
                    Console.ReadKey();
                }
            }
        }
    }
}
