using System.Threading.Tasks;

namespace StockTwits
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new StockTwitsClient();
            //await client.Authorize();

            await client.CreateMessage(Tokens.TOKEN, "New one");
           // await client.CreateMessageWithChart(Tokens.TOKEN, "bodybodybody2 $SPY", 
            //    @"C:\Users\ADMIN\Desktop\StockTwits\StockTwits\YELP Short A CT 1 min out-all 0.87r 11.56_09.11.2018 1026.png");
        }
    }
}