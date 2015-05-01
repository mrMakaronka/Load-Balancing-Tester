using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceTest.Utils
{
    static class RandomUtils
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static string GetRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string[] GetArrayOfRandomStrings(int arraySize, int stringSize)
        {
            string[] randomStrings = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                randomStrings[i] = GetRandomString(stringSize);
            }
            return randomStrings;
        }
    }
}
