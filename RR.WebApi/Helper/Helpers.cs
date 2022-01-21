using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace RR.WebApi
{
     public class Helpers
     {
          public static string GetRandomString(int length, IEnumerable<char> characterSet)
          {
               if (length < 0)
                    throw new ArgumentException("length must not be negative", "length");
               if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                    throw new ArgumentException("length is too big", "length");
               if (characterSet == null)
                    throw new ArgumentNullException("characterSet");
               var characterArray = characterSet.Distinct().ToArray();
               if (characterArray.Length == 0)
                    throw new ArgumentException("characterSet must not be empty", "characterSet");

               var bytes = new byte[length * 8];
               var result = new char[length];
               using (var cryptoProvider = new RNGCryptoServiceProvider())
               {
                    cryptoProvider.GetBytes(bytes);
               }
               for (int i = 0; i < length; i++)
               {
                    ulong value = BitConverter.ToUInt64(bytes, i * 8);
                    result[i] = characterArray[value % (uint)characterArray.Length];
               }
               return new string(result);
          }

          public static string GetIpAddress()
          {
               var host = Dns.GetHostEntry(Dns.GetHostName());
               foreach (var ip in host.AddressList)
               {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                         return ip.ToString();
                    }
               }
               return "";
          }
     }
}
