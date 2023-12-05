using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace RPFO.Utilities.Extensions
{
    public class HashCode
    {
        public string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return Convert.ToBase64String(Results);
        }

        public string DecryptString(string Message, string Passphrase)
        {
            byte[] Results;
            if (Message.Length == 0 || Passphrase.Length == 0)
                return "";

            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return UTF8.GetString(Results);
        }

        //public static string SHA512(string input)
        //{
        //    var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        //    using (var hash = System.Security.Cryptography.SHA512.Create())
        //    {
        //        var hashedInputBytes = hash.ComputeHash(bytes);

        //        // Convert to text
        //        // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
        //        var hashedInputStringBuilder = new System.Text.StringBuilder(128);
        //        foreach (var b in hashedInputBytes)
        //            hashedInputStringBuilder.Append(b.ToString("X2"));
        //        return hashedInputStringBuilder.ToString();
        //    }
        //}

        public static string GenSHA512(string input, bool lower = false)
        {
            string r = "";
            try
            {
                byte[] d = System.Text.Encoding.UTF8.GetBytes(input);
                using (SHA512 a = new SHA512Managed())
                {
                    byte[] h = a.ComputeHash(d);
                    r = BitConverter.ToString(h).Replace("-", "");
                }
                r = lower ? r.ToLowerInvariant() : r;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return r;
        }
    }
}