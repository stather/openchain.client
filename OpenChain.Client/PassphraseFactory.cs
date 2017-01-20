using Google.Protobuf;
using NBitcoin;
using Openchain;
using ByteString = Openchain.ByteString;
using Transaction = Openchain.Transaction;

namespace OpenChain.Client
{
    public class PassphraseFactory
    {
        public static string GetNewPassphrase()
        {
            var m = new Mnemonic(Wordlist.English,WordCount.Twelve);
            return m.ToString();
        }

        public static void DecodeMessage(byte[] data)
        {
            var bs = new ByteString(data);
            var t = MessageSerializer.DeserializeTransaction(bs);
            var m = t.Mutation;
            var dm = MessageSerializer.DeserializeMutation(m);
            //var pm = ParsedMutation.Parse(dm);
        }
    }
}