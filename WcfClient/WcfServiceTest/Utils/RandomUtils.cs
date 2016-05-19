using System;
using System.Text;

namespace WcfServiceTest.Utils
{
    static class RandomUtils
    {
        public static string GetRandomString(Random random, int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                char nextChar = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(nextChar);
            }
            return builder.ToString();
        }
    }
}
