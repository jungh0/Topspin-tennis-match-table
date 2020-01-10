using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GTA.Main
{
    class AES
    {
        private string key = "";
        private static AES mBuilder = null;

        private AES()
        {

        }

        public static AES Builder()
        {
            if (mBuilder == null)
            {
                mBuilder = new AES();
            }
            return mBuilder;
        }

        public AES SetKey(string mKey)
        {
            key = mKey;

            return this;
        }

        public string GetKey()
        {
            return key;
        }

        //AES_128 암호화
        public string AESEncrypt128(string Input)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(Input);
            byte[] Salt = Encoding.ASCII.GetBytes(key.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(PlainText, 0, PlainText.Length);
            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string EncryptedData = Convert.ToBase64String(CipherBytes);

            mBuilder = null;

            SecretKey.Dispose();
            RijndaelCipher.Dispose();

            return EncryptedData;
        }

        //AE_S128 복호화
        public string AESDecrypt128(string Input)
        {
            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();

                byte[] EncryptedData = Convert.FromBase64String(Input);
                byte[] Salt = Encoding.ASCII.GetBytes(key.Length.ToString());

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);
                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(EncryptedData);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

                byte[] PlainText = new byte[EncryptedData.Length];

                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

                memoryStream.Close();
                cryptoStream.Close();

                string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

                mBuilder = null;

                SecretKey.Dispose();
                RijndaelCipher.Dispose();

                return DecryptedData;
            }
            catch
            {
                return "";
            }
        }

    }
}
