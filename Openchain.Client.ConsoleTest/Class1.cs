using System;
using System.Threading.Tasks;

namespace OpenChain.Client.ConsoleTest
{
    public class Class1
    {
        public Class1()
        {
        }

        // these are test-only secrets !
        const string admin = "remind pause film conduct protect grab young myth refuse depth liberty bean";
        // corresponding address is "XiqvPB63hh8TML2iWYGDvF7i3HXRxqv3nN" : add this address to admin list in openchain server config.json

        const string alice = "hidden position purse loop neutral miss know deliver sorry wife general able";
        const string bob = "moon later shift frame vendor aisle brush enrich guitar mix shock picture";
        const string fred = "cover pumpkin child stool vehicle alone rescue behind mom wrong mistake help";
        public async Task Run()
        {
            var ocs = new OpenChainServer("http://localhost:5000/");


            //try
            //{
            //    var m = MnemonicFactor.Create();
            //    var ss = ocs.Login(fred);
            //    var adacc = ocs.Login(admin);

            //    var bert = await ocs.GetData<string>("/aka/fred", "goto");
            //    if (bert.Value == null)
            //    {
            //        bert.Value = ss.Account;
            //        await adacc.SetData(bert);
            //    }

            //    var gbp = "/asset/GBP";
            //    var ti = await adacc.Transfert(gbp, ss.Account, 120, gbp);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            string assetPath;

            using (var a = ocs.Login(alice))
            using (var ad = ocs.Login(admin))
            using (var b = ocs.Login(bob))
            using (var d = ocs.Login(fred))
            {
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

                gt = await ad.GetData<string>("/aka/fred", "goto");
                if (gt.Value == null)
                {
                    gt.Value = d.Account;
                    await ad.SetData(gt);
                }

                assetPath = "/asset/gold/"; //ad.GetAssetPath(0);
                var gbpPath = "/asset/GBP";

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, ad.Account, 300, assetPath)}");

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