using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConnectionFactory factory = new ConnectionFactory
            //{
            //    UserName = "admin",//用户名
            //    Password = "admin",//密码
            //    HostName = "127.0.0.1",//IP
            //};
            ////创建连接
            //var connection = factory.CreateConnection();
            ////创建通道
            //var channel = connection.CreateModel();
            ////声明交换机（名称和类型）
            //channel.ExchangeDeclare("directLogs", "DIRECT");
            ////声明一个队列
            //channel.QueueDeclare("TestMessage", false, false, false, null);

            //Console.WriteLine("\nRabbitMQ连接成功，请输入要发送的消息:");
            //string input = Console.ReadLine();
            //var sendBytes = Encoding.UTF8.GetBytes(input);
            //channel.BasicPublish("", "TestMessage", null, sendBytes);
            //channel.Close();
            //connection.Close();
            DirectExchangeSendMsg();

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
        public static void DirectExchangeSendMsg()
        {
            using (IConnection conn = rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {

                    //消息--routingkey -》交换机- bingdkey -》队列
                    //routingkey和bingdkey通过不同的类型做不同的映射

                    //这些参数作为硬编码在程序组写着，只一次就够了，也可以不写，提前通过界面建立队列和交换机的关系，之后通过直接通过key发送消息到交换机就可以了，交换机自动根据key查询
                    //
                    //channel.ExchangeDeclare(ExchangeName, "direct", durable: true, autoDelete: false, arguments: null);
                    //channel.QueueDeclare(QueueName, durable: true, autoDelete: false, exclusive: false, arguments: null);
                    //channel.QueueBind(QueueName, ExchangeName, routingKey: "2");

                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;
                    string vadata = Console.ReadLine();
                    while (vadata != "exit")
                    {
                        var msgBody = Encoding.UTF8.GetBytes(vadata);                      
                        channel.BasicPublish(exchange: ExchangeName, routingKey: "3", basicProperties: props, body: msgBody);
                        Console.WriteLine(string.Format("***发送时间:{0}，发送完成，输入exit退出消息发送", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                        vadata = Console.ReadLine();
                    }
                }
            }
        }
    }
}
