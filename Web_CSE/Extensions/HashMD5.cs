using System;
using System.Text;
using System.Security.Cryptography;

namespace Web_CSE.Extensions
{
    public static class HashMD5
    {
        public static string ToMD5(this string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash)
                sbHash.Append(String.Format("{0:2}", b));          
            return sbHash.ToString();
        }
    }
}
