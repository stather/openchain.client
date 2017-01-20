using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenChain.Client.ConsoleTest
{
    public class Class1
    {
        public Class1()
        {
        }

        // these are test-only secrets !
        const string admin = "combat pélican gagner bateau caporal infini charbon neutron détester menhir causer espoir carbone saugrenu obscur inexact torrent rayonner laisser relief féroce honteux cirer époque";
        // corresponding address is "XiqvPB63hh8TML2iWYGDvF7i3HXRxqv3nN" : add this address to admin list in openchain server config.json

        const string alice = "pélican combat gagner bateau caporal infini charbon neutron détester menhir causer espoir carbone saugrenu obscur inexact torrent rayonner laisser relief féroce honteux cirer époque";
        const string bob = "pélican gagner combat bateau caporal infini charbon neutron détester menhir causer espoir carbone saugrenu obscur inexact torrent rayonner laisser relief féroce honteux cirer époque";

        private const string fred = "bomb clock can ripple sister energy motion produce envelope photo skirt duck";

        public async Task Run()
        {
            var ocs = new OpenChainServer("http://openchain20170120104825.azurewebsites.net/");


            try
            {
                var cts = new CancellationTokenSource();
                var socket = new ClientWebSocket();
                //var wsUri = $"ws://openchain20170120104825.azurewebsites.net/stream";
                var wsUri = $"ws://localhost:5000/stream";
                await socket.ConnectAsync(new Uri(wsUri), cts.Token);
                await Task.Factory.StartNew(
      async () =>
      {
          var rcvBytes = new byte[1024];
          var rcvBuffer = new ArraySegment<byte>(rcvBytes);
          while (true)
          {
              WebSocketReceiveResult rcvResult =
                  await socket.ReceiveAsync(rcvBuffer, cts.Token);
              byte[] msgBytes = rcvBuffer
                  .Skip(rcvBuffer.Offset)
                  .Take(rcvResult.Count).ToArray();
              try
              {
                  PassphraseFactory.DecodeMessage(msgBytes);
              }
              catch (Exception e)
              {
                  Console.WriteLine(e);
                  throw;
              }
              string rcvMsg = Encoding.UTF8.GetString(msgBytes);
              Console.WriteLine("Received: {0}", rcvMsg);
          }
      }, cts.Token, TaskCreationOptions.LongRunning,
         TaskScheduler.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(TimeSpan.FromDays(1));
            string assetPath;
            var gbpPath = "/asset/GBP/";

            var george = PassphraseFactory.GetNewPassphrase();

            using (var gg = ocs.Login(george))
            using (var f = ocs.Login(fred))
            using (var a = ocs.Login(alice))
            using (var ad = ocs.Login(admin))
            using (var b = ocs.Login(bob))
            {
                await ocs.GetStream();
                var res2 = await ad.Transfert(ad.Account, gg.Account, 77, gbpPath);

                var res = await f.Transfert(f.Account, a.Account, 44, gbpPath);
                var ir = await ocs.GetData<LedgerInfo>("/", "info");
                if (ir.Value == null || ir.Value.Name != "My Ledger")
                {
                    ir.Value = new LedgerInfo { Name = "My Ledger" };
                    await ad.SetData(ir);
                }

                var s = await ocs.GetData<string>("/", "info");

                var gt = await ad.GetData<string>("/aka/alice/", "goto");
                if (gt.Value == null)
                {
                    gt.Value = a.Account;
                    await ad.SetData(gt);
                }

                gt = await ad.GetData<string>("/aka/bob/", "goto");
                if (gt.Value == null)
                {
                    gt.Value = b.Account;
                    await ad.SetData(gt);
                }

                gt = await ad.GetData<string>("/aka/fred/", "goto");
                if (gt.Value == null)
                {
                    gt.Value = f.Account;
                    await ad.SetData(gt);
                }

                assetPath = "/asset/gold/"; //ad.GetAssetPath(0);

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, ad.Account, 300, assetPath)}");
                await ad.Transfert(gbpPath, ad.Account, 400, gbpPath);
                await ad.Transfert(ad.Account, f.Account, 123, gbpPath);

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");

                var re = await b.GetData<UserInfo>(b.Account, "info");
                if (re.Value == null)
                {
                    re.Value = new UserInfo { DisplayName = "Bob" };
                    Console.WriteLine($"SetData : {await b.SetData(re)}");
                }

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");
                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(ad.Account, b.Account, 12, assetPath)}");

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");
                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");
                foreach (var r in await a.GetAccountRecords())
                    Console.WriteLine($"a  : {r}");

                Console.WriteLine($"Transfert : {await b.Transfert(b.Account, a.Account, 2, assetPath)}");

                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");
                foreach (var r in await a.GetAccountRecords())
                    Console.WriteLine($"a  : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, "@alice", 100, assetPath)}");
                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, "@bob", 33, assetPath)}");

                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");
                foreach (var r in await a.GetAccountRecords())
                    Console.WriteLine($"a  : {r}");

            }
        }
    }
}