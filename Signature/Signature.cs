using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Signature
{
    #region Криптопровайдер для создания цифровой подписи производителя и ключей
    /// <summary>
    /// Криптопровайдер для создания цифровой подписи производителя и ключей
    /// </summary>
    public class RSAProvider
    {
        /// <summary>
        /// Открытый ключ
        /// </summary>
        private RSAParameters _publicKey;

        /// <summary>
        /// Закрытый ключ
        /// </summary>
        private RSAParameters _privateKey;

        #region Сформировать открытый и закрытый ключи
        /// <summary>
        /// Сформировать открытый и закрытый ключи
        /// </summary>
        public void AssignNewKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                _publicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);
            }
        }
        #endregion

        #region Подписание хеша
        /// <summary>
        /// Подписание хеша
        /// </summary>
        public byte[] SignData(byte[] hashOfDataToSign)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_privateKey);

                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("SHA256");
                return rsaFormatter.CreateSignature(hashOfDataToSign);
            }
        }
        #endregion

        #region Получение открытого ключа
        /// <summary>
        /// Получение открытого ключа
        /// </summary>
        public RSAParameters GetPublicKey()
        {
            return this._publicKey;
        }
        #endregion

        #region Создание хеша SHA256
        /// <summary>
        /// Создание хеша SHA256
        /// </summary>
        public static byte[] GetHash(byte[] data)
        {
            SHA256 sha256 = SHA256.Create();
            sha256.ComputeHash(data);
            return sha256.Hash;
        }
        #endregion

        #region Верификация подписи
        /// <summary>
        /// Верификация подписи
        /// </summary>
        public static bool VerifySignature(byte[] hashOfDataToSign, byte[] signature, RSAParameters publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(publicKey);
                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");
                return rsaDeformatter.VerifySignature(hashOfDataToSign, signature);
            }
        }
        #endregion

        #region Получение открытого ключа
        /// <summary>
        /// Получение открытого ключа
        /// </summary>
        public static RSAParameters GetPublicKey(RSAParametersSerializable rsa)
        {
            return rsa.GetParametersRSA();
        }
        #endregion
    }
    #endregion

    #region Сериализируемый криптопровайдер для создания цифровой подписи производителя и ключей
    /// <summary>
    /// Сериализируемый криптопровайдер для создания цифровой подписи производителя и ключей
    /// </summary>
    [Serializable]
    public class RSAParametersSerializable : ISerializable
    {
        private RSAParameters _rsaParameters;

        public RSAParametersSerializable(RSAParameters rsaParameters)
        {
            _rsaParameters = rsaParameters;
        }

        public byte[] D { get { return _rsaParameters.D; } set { _rsaParameters.D = value; } }

        public byte[] DP { get { return _rsaParameters.DP; } set { _rsaParameters.DP = value; } }

        public byte[] DQ { get { return _rsaParameters.DQ; } set { _rsaParameters.DQ = value; } }

        public byte[] Exponent { get { return _rsaParameters.Exponent; } set { _rsaParameters.Exponent = value; } }

        public byte[] InverseQ { get { return _rsaParameters.InverseQ; } set { _rsaParameters.InverseQ = value; } }

        public byte[] Modulus { get { return _rsaParameters.Modulus; } set { _rsaParameters.Modulus = value; } }

        public byte[] P { get { return _rsaParameters.P; } set { _rsaParameters.P = value; } }

        public byte[] Q { get { return _rsaParameters.Q; } set { _rsaParameters.Q = value; } }

        public RSAParametersSerializable(SerializationInfo information, StreamingContext context)
        {
            _rsaParameters = new RSAParameters()
            {
                D = (byte[])information.GetValue("D", typeof(byte[])),
                DP = (byte[])information.GetValue("DP", typeof(byte[])),
                DQ = (byte[])information.GetValue("DQ", typeof(byte[])),
                Exponent = (byte[])information.GetValue("Exponent", typeof(byte[])),
                InverseQ = (byte[])information.GetValue("InverseQ", typeof(byte[])),
                Modulus = (byte[])information.GetValue("Modulus", typeof(byte[])),
                P = (byte[])information.GetValue("P", typeof(byte[])),
                Q = (byte[])information.GetValue("Q", typeof(byte[]))
            };
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("D", _rsaParameters.D);
            info.AddValue("DP", _rsaParameters.DP);
            info.AddValue("DQ", _rsaParameters.DQ);
            info.AddValue("Exponent", _rsaParameters.Exponent);
            info.AddValue("InverseQ", _rsaParameters.InverseQ);
            info.AddValue("Modulus", _rsaParameters.Modulus);
            info.AddValue("P", _rsaParameters.P);
            info.AddValue("Q", _rsaParameters.Q);
        }

        public RSAParameters GetParametersRSA()
        {
            return this._rsaParameters;
        }
    }
    #endregion
}
