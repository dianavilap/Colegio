using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ColegioArceProject.Classes
{
    public class EncryptionService
    {
        public static String Encrypt(String text)
        {
            try
            {
                var rijndaelManaged = new RijndaelManaged();
                EncryptorTransform = rijndaelManaged.CreateEncryptor(KeyVal, Vector);
                DecryptorTransform = rijndaelManaged.CreateDecryptor(KeyVal, Vector);
                UTFEncoder = new UTF8Encoding();

                return EncryptToString(text);
            }
            catch
            {
                //LOG?? COULD NOT ENCRYPT SOMETHING!!
                return null;
            }
        }
        public static String Decrypt(String text)
        {
            try
            {
                var rijndaelManaged = new RijndaelManaged();

                EncryptorTransform = rijndaelManaged.CreateEncryptor(KeyVal, Vector);
                DecryptorTransform = rijndaelManaged.CreateDecryptor(KeyVal, Vector);

                UTFEncoder = new UTF8Encoding();

                return DecryptString(text);
            }
            catch
            {
                //LOG?? COULD NOT DECRYPT SOMETHING!!
                return null;
            }
        }

        /*RIJNDAEL*/
        private static Byte[] KeyVal = { 109, 182, 173, 114, 125, 136, 019, 158, 149, 140, 151, 162, 188, 199, 068, 045, 012, 144, 050, 054, 183, 122, 231, 150, 017, 012, 035, 074, 095, 110, 142, 195 };
        private static Byte[] Vector = { 246, 131, 074, 065, 105, 033, 074, 031, 022, 199, 111, 166, 102, 205, 023, 029 };

        private static ICryptoTransform EncryptorTransform, DecryptorTransform;
        private static UTF8Encoding UTFEncoder;
        private static String EncryptToString(String TextValue)
        {
            return ByteArrToString(EncryptToByteArray(TextValue));
        }
        private static Byte[] EncryptToByteArray(String TextValue)
        {
            var bytes = UTFEncoder.GetBytes(TextValue);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            memoryStream.Position = 0;
            var encrypted = new Byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            cryptoStream.Close();
            memoryStream.Close();

            return encrypted;
        }
        private static String DecryptString(String EncryptedString)
        {
            return DecryptToByteArray(StrToByteArray(EncryptedString));
        }
        private static String DecryptToByteArray(Byte[] EncryptedValue)
        {
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();

            return UTFEncoder.GetString(decryptedBytes);
        }
        private static Byte[] StrToByteArray(String text)
        {
            Byte vaule;
            var ByteArray = new Byte[text.Length / 3];
            var i = 0;
            var j = 0;
            do
            {
                vaule = Byte.Parse(text.Substring(i, 3));
                ByteArray[j++] = vaule;
                i += 3;
            }
            while (i < text.Length);
            return ByteArray;
        }
        private static String ByteArrToString(Byte[] ByteArray)
        {
            Byte value;
            var text = String.Empty;
            for (int i = 0; i <= ByteArray.GetUpperBound(0); i++)
            {
                value = ByteArray[i];
                if (value < (Byte)10) text += "00" + value.ToString();
                else if (value < (Byte)100) text += "0" + value.ToString();
                else text += value.ToString();
            }
            return text;
        }

        private static Byte[] GenerateEncryptionKey()
        {
            var rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.GenerateKey();
            return rijndaelManaged.Key;
        }
        private static Byte[] GenerateEncryptionVector()
        {
            var rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.GenerateIV();
            return rijndaelManaged.IV;
        }
        /*RIJNDAEL*/
    }
}