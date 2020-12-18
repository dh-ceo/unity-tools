//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Email   : developershub.organization@gmail.com
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using System;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace DevelopersHub.Unity.Tools
{
    public static class Encryption
    {

        public static string Serialize<T>(this T target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            xml.Serialize(writer, target);
            return writer.ToString();
        }

        public static T Deserialize<T>(this string target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(target);
            return (T)xml.Deserialize(reader);
        }

        private const int keysize = 256;
        private const int derivationIterations = 1000;

        private static string EncryptData(string data, string key)
        {
            var saltStringBytes = RandomGenerate256Bits();
            var ivStringBytes = RandomGenerate256Bits();
            var plainTextBytes = Encoding.UTF8.GetBytes(data);
            using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, derivationIterations))
            {
                var keyBytes = password.GetBytes(keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        private static string DecryptData(string data, string key)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(data);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(keysize / 8).Take(keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((keysize / 8) * 2)).ToArray();
            using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, derivationIterations))
            {
                var keyBytes = password.GetBytes(keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] RandomGenerate256Bits()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static string Encrypt(string data, string key)
        {
            return EncryptData(data, key);
        }

        public static string Decrypt(string data, string key)
        {
            return DecryptData(data, key);
        }

        public static string EncrypteToMD5(string data)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(data);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);
            string hashString = "";
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString = hashString + Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
            return hashString.PadLeft(32, '0');
        }

    }
}