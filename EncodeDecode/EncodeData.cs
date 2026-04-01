using Signature;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace EncodeDecode
{
    /// <summary>
    /// Вспомогательный класс для закодированной информации
    /// </summary>
    [Serializable]
    public class EncodeData
    {
        /// <summary>
        /// Закодированная информация - последовательность байтов
        /// </summary>
        public byte[] Encode { get; set; }

        /// <summary>
        /// Вспомогательные параметры для декодирования
        /// </summary>
        public List<Inside> InsideInfo { get; set; }

        /// <summary>
        /// Подпись
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// Открытый ключ для проверки подписи (подпись - массив байтов) входящего пакета данных (десериализованный объект Encode) 
        /// </summary>
        public RSAParametersSerializable PublicKey { get; set; }

        /// <summary>
        /// Конструктор EncodeData
        /// </summary>
        public EncodeData(byte[] encode, List<Inside> insides, byte[] signature, RSAParametersSerializable public_key)
        {
            this.Encode = encode;
            this.InsideInfo = insides;
            this.Signature = signature;
            this.PublicKey = public_key;
        }
    }

    /// <summary>
    /// Вспомогательный класс для декодирования TLV-объектов
    /// </summary>
    [Serializable]
    public class Inside
    {
        /// <summary>
        /// Тип контента
        /// </summary>
        public object Type { get; set; }

        /// <summary>
        /// Длина типа контента
        /// </summary>
        public object LengthType { get; set; }

        /// <summary>
        /// Длина длины контента
        /// </summary>
        public object LengthLength { get; set; }

        /// <summary>
        /// Длина значения контента
        /// </summary>
        public object LengthValue { get; set; }

        /// <summary>
        /// Вложения
        /// </summary>
        public int Insides { get; set; }

        /// <summary>
        /// Строка байтов TLV-объекта
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Конструктор Inside
        /// </summary>
        public Inside()
        {
            this.LengthType = Convert.ToUInt32(0);
            this.LengthLength = Convert.ToUInt32(0);
            this.LengthValue = Convert.ToUInt32(0);
            this.Value = new List<byte>().ToArray();
            this.Insides = 0;
        }
    }
}
