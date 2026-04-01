using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketCreator
{
    public static class CreatorHelper
    {
        #region Создание пакета интереса
        /// <summary>
        /// Создание пакета интереса
        /// </summary>
        public static InterestPacket GetInterestPacket(
            byte type, 
            string full_name, 
            ushort interstLifeTime, 
            bool canBePrefix)
        {
            InterestPacket interest_packet = new InterestPacket(type, full_name, interstLifeTime, canBePrefix);
            return interest_packet;
        }
        #endregion

        #region Создание пакета данных
        /// <summary>
        /// Создание пакета данных
        /// </summary>
        public static DataPacket GetDataPacket(
            byte type,
            string full_name,
            byte content_type,
            ushort freshness_period,
            byte[] content)
        {
            MetaInfo metadata = GetMetaInfo(content_type, freshness_period, full_name);
            DataPacket data_packet = new DataPacket(type, full_name, metadata, content);
            return data_packet;
        }
        #endregion

        #region Создание объекта метаданных
        /// <summary>
        /// Создание объекта метаданных
        /// </summary>
        public static MetaInfo GetMetaInfo(byte content_type, ushort freshness_period, string path)
        {
            path = path.Split('\\').Last();
            MetaInfo metadata = new MetaInfo(content_type, freshness_period, path);
            return metadata;
        }
        #endregion

        #region Создание блока TLV
        /// <summary>
        /// Создание блока TLV
        /// </summary>
        public static TLV Create_TLV(byte type, object input)
        {
            _ = new List<byte>().ToArray();
            byte[] bytes;
            if (input.GetType().Name == "String")
            {
                string s = input.ToString();
                bytes = Encoding.Unicode.GetBytes(s);
            }
            else
            {
                bytes = BitConverter.GetBytes(Convert.ToUInt16(input));
            }
            uint length = Convert.ToUInt16(bytes.Length);
            TLV tlv = new TLV(type, length, bytes);
            return tlv;
        }
        #endregion

        #region Поиск длины блоков TLV
        /// <summary>
        /// Поиск длины блоков TLV
        /// </summary>
        public static uint Find_TLVs_Length(List<TLV> tlvs)
        {
            uint length = 0;
            foreach (var tlv in tlvs)
            {
                length += Convert.ToUInt32(BitConverter.GetBytes(Convert.ToUInt32(tlv.Type)).Length);
                length += Convert.ToUInt32(BitConverter.GetBytes(Convert.ToUInt32(tlv.Length)).Length);
                length += Convert.ToUInt32(tlv.Length);
            }
            return length;
        }
        #endregion
    }
}
