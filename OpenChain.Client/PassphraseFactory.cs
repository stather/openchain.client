using System;
using Google.Protobuf;
using NBitcoin;
using Openchain;
using Openchain.Infrastructure;
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
            Console.WriteLine("############################################################");
            var bs = new ByteString(data);
            var t = MessageSerializer.DeserializeTransaction(bs);
            var date = t.Timestamp;
            var m = t.Mutation;
            var dm = MessageSerializer.DeserializeMutation(m);
            var pm = ParsedMutation.Parse(dm);
            if (pm.AccountMutations != null)
            {
                foreach (var accountMutation in pm.AccountMutations)
                {
                    var ledgerPath = accountMutation.AccountKey.Account.FullPath;
                    var assetPath = accountMutation.AccountKey.Asset.FullPath;
                    var accountKeyName = accountMutation.AccountKey.Key.Name;
                    var accountKeyPath = accountMutation.AccountKey.Key.Path.FullPath;
                    var recordType = accountMutation.AccountKey.Key.RecordType;
                    var balance = accountMutation.Balance;
                    Console.WriteLine($"{ledgerPath}, {assetPath}, {accountKeyName}, {accountKeyPath}, {recordType}, {balance}, {date}");
                }
            }
        }
    }
}