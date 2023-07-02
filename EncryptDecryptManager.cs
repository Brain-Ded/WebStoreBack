using System;
using System.Security.Cryptography;
using System.Text;

namespace Web_StoreAPI.DataModels
{
    public class EncryptDecryptManager
    {

        public static string Encrypt(string text, out byte[] key, out byte[] iv)
        {
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                key = aes.Key;
                iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                        array = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string text, byte[] key, byte[] iv)
        {
            byte[] buffer = Convert.FromBase64String(text);
            using(Aes aes = Aes.Create())
            {

                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream((Stream)ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
