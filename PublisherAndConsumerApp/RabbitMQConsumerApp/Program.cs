using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

internal class Program
{
    private static void Main(string[] args)
    {
        //Öncelikle RabbitMQ'ya bağlanıyoruz
        ConnectionFactory factory = new ConnectionFactory();
        factory.HostName = "localhost";
        //Bağlantı açıyoruz
        IConnection connection = factory.CreateConnection();
        //Yukarıda oluşturduğumuz bağlantı üzerinden bir kanal oluşturuyoruz.
        IModel chanel = connection.CreateModel();

        // QueueDeclare() ve içerisindeki parametreler birebir publisher uygulamasında ki verilen değerler ile aynı olmalıdır.Aksi taktirde hata alırız.
        chanel.QueueDeclare(queue: "example", durable: true, exclusive: false, autoDelete: false);


        /*Yukarıdaki kodlar zaten publisher kısmında yazdığımız kodlarla birebir aynı.
         Standart bağlantı-kanal-kuyruk açma kodları. Şimdi consumer ile ilgili
         kodlarımızı yazıyoruz.*/

        //İlgili kanal üzerinden publisher uygulamasından gelen mesajları tüketecek consumer nesnesini oluşturuyoruz.
        EventingBasicConsumer consumer = new EventingBasicConsumer(chanel);


        //Kanal üzerinden consumer'ın hangi kuyruğu dinleyeceğini tanımlıyoruz
        //autoAck:Kuyruktan alınan mesajların siinip silinmeme durumunu ifade eder.Eğer mesaj kuyruktan alındıktan hemen sonra kuyruktan silinirse consumer uygulamasında karşımıza çıkan bir hatadan dolayı alınan mesaj 
        //işlenmezse burada veri kaybı meydana gelebilir o yüzden bir genel olarak autoAck'yı false olarak işaretliyelim :D
        chanel.BasicConsume(queue: "example", autoAck: false, consumer: consumer);


        //consumer nesnesinin Received olayı bizlere kuyruktaki mesajı getirecektir.
        consumer.Received += (object sender, BasicDeliverEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine("Gelen Mesaj: " + message);

            /*Mesajların doğru bir şekilde işlendiğini ve kuyruktan silineceğini 
            bildiriyoruz.multiple -> işlenmiş ama rabbitmq'dan silinmemiş 
            mesajları da silsin istersek true demeliyiz.Biz sadece kendi 
            mesajımızla ilgileneceğimiz için false diyoruz.*/
            chanel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

        };
        Console.Read();




    }
}