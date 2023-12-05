using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RPFO.Application.Helpers
{
    public class RSACrypto
    {
        private static RSACrypto instance;
        private readonly static object lockInstance = new object();

        private RSACryptoServiceProvider privatekey = null;
        private RSACryptoServiceProvider publicKey = null;

        /// <summary>
        /// Create
        /// </summary>
        private RSACrypto()
        {
            try
            {
                string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                #region Privatekey

                var privateKeyPath = @$"{baseDirectory}\RSAKey\PrivateKey_Partner.xml";
                string xmlPrivatekey = File.ReadAllText(privateKeyPath);
                privatekey = new RSACryptoServiceProvider();
                privatekey.FromXmlString(xmlPrivatekey);

                #endregion

                #region PublicKey

                var publicKeyPath = @$"{baseDirectory}\RSAKey\PublicKey_Payoo.xml";
                string xmlPublickey = File.ReadAllText(publicKeyPath);
                publicKey = new RSACryptoServiceProvider();
                publicKey.FromXmlString(xmlPublickey);

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static RSACrypto GetInstance()
        {
            if (instance == null)
            {
                lock (lockInstance)
                {
                    if (instance == null)
                    {
                        instance = new RSACrypto();
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Sign
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Sign(string value)
        {
            try
            {
                SHA256 sha256 = new SHA256Managed();
                byte[] data = Encoding.UTF8.GetBytes(value);
                byte[] hash = sha256.ComputeHash(data);
                var bytesSign = privatekey.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
                return Convert.ToBase64String(bytesSign);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// VerifySign
        /// </summary>
        /// <param name="value">string</param>
        /// <param name="sign">string</param>
        /// <returns>bool</returns>
        public bool VerifySign(string value, string sign)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(value);            //chuyển thành byte array
                SHA256 sha256 = new SHA256Managed();
                var hash = sha256.ComputeHash(data);                    // hash theo chuẩn sha256 
                var isVerify = publicKey.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), // verify
                    Convert.FromBase64String(sign));
                return isVerify;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
