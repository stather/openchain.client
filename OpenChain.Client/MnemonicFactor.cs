using System;
using NBitcoin;

namespace OpenChain.Client
{
    public class MnemonicFactor
    {
        public static string Create()
        {
            try
            {
                Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
                var hk = mnemo.DeriveExtKey();
                return mnemo.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}