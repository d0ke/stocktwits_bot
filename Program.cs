using System.Linq;
using System.Threading.Tasks;

namespace StockTwits
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new StockTwitsClient();
            //await client.Authorize();

            //await client.CreateMessage("bodybodybody1 $SPY");
            await client.CreateMessageWithChartMultipart("bodybodybody321 $SPY", @"C:\Users\ADMIN\Desktop\png.png");
        }
    }
}





//POST https://api.stocktwits.com/api/2/messages/create.json HTTP/1.1
//Content-Type: multipart/form-data; boundary="970681cf-2341-4a2c-812c-628ecc1ca513"
//Host: api.stocktwits.com
//Content-Length: 366
//Expect: 100-continue
//Connection: Keep-Alive

//--970681cf-2341-4a2c-812c-628ecc1ca513
//Content-Type: text/plain; charset=utf-8
//Content-Disposition: form-data; name=access_token

//2e6e5bfbdacaec99f7222d9c657eeaea3e06d867
//--970681cf-2341-4a2c-812c-628ecc1ca513
//Content-Type: text/plain; charset=utf-8
//Content-Disposition: form-data; name=body

//bodybodybody321 $SPY
//--970681cf-2341-4a2c-812c-628ecc1ca513--
