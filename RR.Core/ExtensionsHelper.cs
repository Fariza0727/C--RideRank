using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RR.Core
{
    public static class ExtensionsHelper
    {
        public static string GetActualError(this Exception exception)
        {
            string message_ = string.Empty;
            if (exception != null)
            {
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                if (exception.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    string patern_ = "(\"dbo.)([A-Za-z0-9_.])+(\")";

                    Regex re = new Regex(patern_, RegexOptions.IgnoreCase);
                    Match m = re.Match(exception.Message);
                    if (re.IsMatch(exception.Message))
                    {
                        message_ = $@"This record is associate with {m.Value.Replace("dbo.", string.Empty)}";
                    }

                }
                else
                {
                    message_ = exception.Message;
                }
            }

            return message_;
        }

        /// <summary>
        /// Creates a pseudo-random password containing the number of character classes
        /// defined by complexity, where 2 = alpha, 3 = alpha+num, 4 = alpha+num+special.
        /// </summary>
        public static string GeneratePassword(int length, int complexity)
        {
            System.Security.Cryptography.RNGCryptoServiceProvider csp =
            new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Define the possible character classes where complexity defines the number
            // of classes to include in the final output.
            char[][] classes =
            {
                @"abcdefghijklmnopqrstuvwxyz".ToCharArray(),
                @"ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                @"0123456789".ToCharArray(),
                @" !""#$%&'()*+,./:;<>?@[\]^_{|}~".ToCharArray(),
                };

            complexity = Math.Max(1, Math.Min(classes.Length, complexity));
            if (length < complexity)
                throw new ArgumentOutOfRangeException("length");

            // Since we are taking a random number 0-255 and modulo that by the number of
            // characters, characters that appear earilier in this array will recieve a
            // heavier weight. To counter this we will then reorder the array randomly.
            // This should prevent any specific character class from recieving a priority
            // based on it's order.
            char[] allchars = classes.Take(complexity).SelectMany(c => c).ToArray();
            byte[] bytes = new byte[allchars.Length];
            csp.GetBytes(bytes);
            for (int i = 0; i < allchars.Length; i++)
            {
                char tmp = allchars[i];
                allchars[i] = allchars[bytes[i] % allchars.Length];
                allchars[bytes[i] % allchars.Length] = tmp;
            }

            // Create the random values to select the characters
            Array.Resize(ref bytes, length);
            char[] result = new char[length];

            while (true)
            {
                csp.GetBytes(bytes);
                // Obtain the character of the class for each random byte
                for (int i = 0; i < length; i++)
                    result[i] = allchars[bytes[i] % allchars.Length];

                // Verify that it does not start or end with whitespace
                if (Char.IsWhiteSpace(result[0]) || Char.IsWhiteSpace(result[(length - 1) % length]))
                    continue;

                string testResult = new string(result);
                // Verify that all character classes are represented
                if (0 != classes.Take(complexity).Count(c => testResult.IndexOfAny(c) < 0))
                    continue;

                return testResult;
            }
        }

        public static decimal ToRound(this decimal input)
        {
            return Math.Round(input, 2);
        }

        public static IQueryable<T> GetPage<T>(this IQueryable<T> input, int page, int pagesize, out int count)
        {
            count = input.Count();
            return input.Skip(page * pagesize).Take(pagesize);
        }

        
    }
}
