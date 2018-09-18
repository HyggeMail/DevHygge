using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;


namespace HyggeMail.BLL.Common
{
    public static class EncryptionHelper
    {
        public static string Encrypt(this string plainText, string encryptionPrivateKey = Config.EncryptionKey)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            cryptoServiceProvider.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));
            return Convert.ToBase64String(EncryptionHelper.EncryptTextToMemory(plainText, cryptoServiceProvider.Key, cryptoServiceProvider.IV));
        }

        public static byte[] EncryptToByte(this string plainText, string encryptionPrivateKey = Config.EncryptionKey)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            cryptoServiceProvider.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));
            return EncryptionHelper.EncryptTextToMemory(plainText, cryptoServiceProvider.Key, cryptoServiceProvider.IV);
        }

        public static string Decrypt(this string cipherText, string encryptionPrivateKey = Config.EncryptionKey)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            cryptoServiceProvider.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));
            return EncryptionHelper.DecryptTextFromMemory(Convert.FromBase64String(cipherText), cryptoServiceProvider.Key, cryptoServiceProvider.IV);
        }

        public static string DecryptFromByte(this byte[] cipherText, string encryptionPrivateKey = Config.EncryptionKey)
        {
            if (cipherText == null || cipherText.Length == 0)
                return String.Empty;
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            cryptoServiceProvider.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));
            return EncryptionHelper.DecryptTextFromMemory(cipherText, cryptoServiceProvider.Key, cryptoServiceProvider.IV);
        }

        private static byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    byte[] bytes = new UnicodeEncoding().GetBytes(data);
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return memoryStream.ToArray();
            }
        }

        private static string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    return new StreamReader((Stream)cryptoStream, (Encoding)new UnicodeEncoding()).ReadLine();
            }
        }

        public static string HashCode(string str)
        {
            SHA1 hash = SHA1CryptoServiceProvider.Create();
            byte[] plainTextBytes = Encoding.ASCII.GetBytes(str);
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);
            string localChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return localChecksum;
        }

        public static string CreateToken(string string_to_sign, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(string_to_sign);
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return string.Join("", Array.ConvertAll(hashmessage, b => b.ToString("x2")));
            }
        }

        public static string GetMD5HashFromFile(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
    }
}
