using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Malam.Digital.Base.Bll.Helpers
{
    public class EncryptBll
    {
        //Save the keystring some place like your database and use it to decrypt and encrypt
        //any text string or text file etc. Make sure you dont lose it though.
        private static readonly string KeyString = "E27BB2B0-5729-4AAE-8E09-B6D501C7";

        #region Public static Function
        public static string Encrypt(string plainStr)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.ISO10126
            };
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText);
        }

        public static string Decrypt(string encryptedText)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.ISO10126
            };
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedText.ToCharArray(), 0, encryptedText.Length);
            return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }
        #endregion
    }
}
