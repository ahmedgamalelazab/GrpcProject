using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Models;
using Grpc.Net.Client;

namespace EcommerceApp.Controllers;

public class HomeController : Controller
{
    private readonly static Inventory inventory = new Inventory();
    private Grpc.Net.Client.GrpcChannel channel;
    private Grpc.Net.Client.GrpcChannel channel2;

    private Greeter.GreeterClient client;
    private Greeter.GreeterClient client2;
    public HomeController()
    {
        channel = GrpcChannel.ForAddress("https://localhost:7151");
        channel2 = GrpcChannel.ForAddress("https://localhost:7074");
        client = new Greeter.GreeterClient(channel);
        client2 = new Greeter.GreeterClient(channel2);
    }

    //we will be talking to Inventory that contains the same database 
    private readonly List<Product> products = new List<Product>(){
        new Product(){Id = 1 , Name = "PlayStation 5 Console" , Image = "https://m.media-amazon.com/images/I/619BkvKW35L._AC_UL320_.jpg" , Price=700},
        new Product(){Id = 2 , Name = "Meta Quest 2 Virtual Reality Headset" , Image = "https://m.media-amazon.com/images/I/61tE7IcuLmL._AC_UL320_.jpg" , Price=399},
        new Product(){Id = 3 , Name = "Xbox Series S" , Image = "https://m.media-amazon.com/images/I/71NBQ2a52CL._AC_UL320_.jpg" , Price=255},
        new Product(){Id = 4 , Name = "PlayStation DualSense Wireless" , Image = "https://m.media-amazon.com/images/I/61Uh8NFDzsL._AC_UL320_.jpg" , Price=22},
        new Product(){Id = 5 , Name = "DualShock 4 Wireless" , Image = "https://m.media-amazon.com/images/I/61IG46p-yHL._AC_UL320_.jpg" , Price=100},
        new Product(){Id = 6 , Name = "Logitech G502 HERO High Performance" , Image = "https://m.media-amazon.com/images/I/61mpMH5TzkL._AC_UY218_.jpg" , Price=10},
        new Product(){Id = 7 , Name = "BENGOO G9000 Stereo Gaming Headset" , Image = "https://m.media-amazon.com/images/I/61CGHv6kmWL._AC_UY218_.jpg" , Price=290},
    };


    public async Task<IActionResult> Index()
    {

        var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
        Console.WriteLine("InventoryServer : " + reply.Message);

        var reply2 = await client2.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
        Console.WriteLine("PaymentServer : " + reply.Message);

        return View(inventory);
    }

    [HttpGet]
    public async Task<IActionResult> Buy(int Id)
    {

        var reply = await client.BuyItemAsync(
                  new RequestLoad { ItemId = Id });

        inventory.Items.SingleOrDefault((item) => item.Product.Id == Id).Quantity = (int)reply.CurrentAmout;

        Console.WriteLine("Greeting: " + reply.CurrentAmout);

        var reply2 = await client2.PayItemAsync(
                  new RequestLoad { ItemId = Id });

        Console.WriteLine("Greeting: " + reply2.Message);

        return View("Index", inventory);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
