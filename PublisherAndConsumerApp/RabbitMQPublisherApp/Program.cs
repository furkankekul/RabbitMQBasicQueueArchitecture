using RabbitMQ.Client;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        //Rabbitmq sunucusuna erişmek için deaktif bağlantı nesnesini oluşturuyoruz 
        ConnectionFactory factory = new ConnectionFactory();
        factory.HostName = "localhost"; 

        //deaktif bağlantı nesnesini aktif hale getiriyoruz.

        IConnection connection = factory.CreateConnection();

        //aktif olan nesne üzerinde bir kanal oluşturuyoruz.
        IModel chanel = connection.CreateModel();


        //oluşturulan kanal üzerinden kuyruk oluşturuyoruz.Bu işlemi QueueDeclare metotu ile tanımlıyoruz
        //queue -> Kuyruğumuzun adı
        //durable(dayanıklı)->Eğer true yaparsak kuyruklarımız fiziksel,false yaparsak hafızada saklanır.
        //exclusive(özel) ->Oluşturulacak bu kuyruğa birden fazla kanalın bağlanıp, bağlanmayacağını belirtir
        //autoDelete:Kuyrukta yer alan veri consumer uygulamasına ulaştıktan sonra silinip silinmemesini belirtir.
        chanel.QueueDeclare(queue: "example", durable: true, exclusive: false, autoDelete: false);


        //Oluşturmuş olduğumuz kuyruğa mesaj yollama işlemini gerçekleştiriyoruz.
        //oluşturacağımız mesajın türü byte olackatır kesinlikle
        
        string message = "examplequeue";
        byte[] byteconvertmessage=Encoding.UTF8.GetBytes(message);
        chanel.BasicPublish(exchange: "", routingKey: "example", body:byteconvertmessage);
        Console.WriteLine(message+",Kuyruğa eklendi");

        Console.Read();


        //Yukarıdaki işlemler neticesinde oluşturmuş olduğumuz example kuyruğuna message gönderme işlemini tamamlamış oluyoruz .



    }
}