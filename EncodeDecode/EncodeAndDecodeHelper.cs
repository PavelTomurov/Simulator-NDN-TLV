using PacketCreator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace EncodeDecode
{
    public static class EncodeAndDecodeHelper
    {
        #region Декодирование

        #region Десериализация
        /// <summary>
        /// Десериализация EncodeData
        /// </summary>
        public static EncodeData Deserialization(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (EncodeData)obj;
            }
        }

        /// <summary>
        /// Десериализация TLV
        /// </summary>
        public static TLV DeserializationTLV(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (TLV)obj;
            }
        }

        #endregion

        #region Декодирование строки байтов в TLV-блок
        /// <summary>
        /// Декодирование строки байтов в TLV-блок
        /// </summary>
        public static TLV Decoding_To_TLV(byte[] encode, List<Inside> insides)
        {
            long count = 0;
            List<TLV> all_tlvs = new List<TLV>(); //Все TLV-блоки
            foreach (var ins in insides)
            {
                var type = BitConverter.ToUInt16(encode.Skip((int)count).Take(Convert.ToInt32(ins.LengthType)).ToArray());
                var length = BitConverter.ToUInt16(encode.Skip((int)(count + Convert.ToInt32(ins.LengthType))).Take(Convert.ToInt32(ins.LengthLength)).ToArray());
                var value_byte = new List<byte>().ToArray();

                count += Convert.ToInt16(ins.LengthType) + Convert.ToInt16(ins.LengthLength);
                if (Convert.ToInt16(ins.Insides) == 0)
                {
                    value_byte = encode.Skip((int)count).Take(Convert.ToInt32(ins.LengthValue)).ToArray();
                    count += Convert.ToInt64(ins.LengthValue);
                }
                TLV packet_tlv = new TLV(type, length, value_byte);
                all_tlvs.Add(packet_tlv);
            }
            List<TLV> data_name_packet_tlvs = Get_Name_Tlvs(all_tlvs); //Внутренние блоки TLV у пакета данных
            List<TLV> metainfo_packet_tlvs = Get_MetaInfo_Tlvs(all_tlvs); //Внутренние блоки TLV у метаданных
            TLV data_name_tlv = AddRange_To_TLV(all_tlvs, data_name_packet_tlvs, 7); //Блок TLV для имени
            TLV metadata_tlv = AddRange_To_TLV(all_tlvs, metainfo_packet_tlvs, 20); //Блок TLV для метаданных
            TLV content_tlv = all_tlvs.Where(t => Convert.ToUInt16(t.Type) == 21).FirstOrDefault(); //Блок TLV для контента
            if (Convert.ToUInt32(content_tlv.Length) != Convert.ToUInt32(content_tlv.Value.Length))
                content_tlv.Length = Convert.ToUInt32(content_tlv.Value.Length);
            TLV data_packet_tlv = AddRange_To_TLV(all_tlvs, new List<TLV>() { data_name_tlv, metadata_tlv, content_tlv }, 6);
            if (data_packet_tlv.Length != insides[0].LengthValue)
                data_packet_tlv.Length = insides[0].LengthValue;


            return data_packet_tlv;
        }
        #endregion

        #region Получение вложенных TLV-блоков для наименования
        /// <summary>
        /// Получение вложенных TLV-блоков для наименования
        /// </summary>
        static List<TLV> Get_Name_Tlvs(List<TLV> all_tlvs)
        {
            List<TLV> data_name_packet_tlvs = new List<TLV>();
            data_name_packet_tlvs.AddRange(all_tlvs.Where(t => Convert.ToUInt16(t.Type) == 8));
            return data_name_packet_tlvs;
        }
        #endregion

        #region Получение вложенных TLV-блоков для метаданных
        /// <summary>
        /// Получение вложенных TLV-блоков для метаданных
        /// </summary>
        static List<TLV> Get_MetaInfo_Tlvs(List<TLV> all_tlvs)
        {
            List<TLV> metainfo_packet_tlvs = new List<TLV>();
            metainfo_packet_tlvs.AddRange(all_tlvs.Where(t => Enumerable.Range(24, 26).Contains(Convert.ToUInt16(t.Type))));
            return metainfo_packet_tlvs;
        }
        #endregion

        #region Добавление декодированных вложенных TLV-блоков в "старший" блок
        /// <summary>
        /// Добавление декодированных вложенных TLV-блоков в "старший" блок
        /// </summary>
        static TLV AddRange_To_TLV(List<TLV> all_tlvs, List<TLV> tlvs, uint type)
        {
            TLV tlv = all_tlvs.Where(t => Convert.ToUInt16(t.Type) == type).FirstOrDefault();
            tlv.SubTLVs.AddRange(tlvs);
            return tlv;
        }
        #endregion

        #endregion

        #region Кодирование

        #region Сериализация
        /// <summary>
        /// Сериализация EncodeData
        /// </summary>
        public static byte[] Serialization(EncodeData data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Сериализация TLV
        /// </summary>
        public static byte[] SerializationTLV(TLV data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        #endregion

        #region Кодирование натурального числа
        /// <summary>
        /// Кодирование натурального числа
        /// </summary>
        static byte[] Encoding_Natural_Number(ulong v)
        {
            _ = new List<byte>().ToArray();
            byte[] result;
            /*
             * Если v <= 255, то v кодируется одним байтом (0-255). 2^8.
             * Иначе если v <= 65 535, то v кодируется двумя байтами (0-65535). 2^16.
             * Иначе если v <= 4 294 967 295, то v кодируется четырьмя байтами (0-4294967295). 2^32.
             * Иначе v кодируется восьмью байтами (0-18 446 744 073 709 551 615). 2^64.

            */
            if (v <= 0xff)
            {
                result = BitConverter.GetBytes(Convert.ToByte(v));
            }
            else if (v <= 0xffff)
            {
                result = BitConverter.GetBytes(Convert.ToUInt16(v));
            }
            else if (v <= 0xffffffff)
            {
                result = BitConverter.GetBytes(Convert.ToUInt32(v));
            }
            else
            {
                result = BitConverter.GetBytes(Convert.ToUInt64(v));
            }
            return result;
        }
        #endregion

        #region Кодирование блока TLV
        /// <summary>
        /// Кодирование блока TLV
        /// </summary>
        public static byte[] Encoding_TLV(TLV v, List<Inside> insides)
        {
            byte[] byte_type = Encoding_Natural_Number(Convert.ToUInt32(v.Type));
            byte[] byte_length = Encoding_Natural_Number(Convert.ToUInt32(v.Length));
            byte[] byte_value = new List<byte>().ToArray();

            Inside ins = new Inside();
            ins.Type = Convert.ToUInt32(v.Type);
            ins.LengthType = Convert.ToUInt32(byte_type.Length);
            ins.LengthLength = Convert.ToUInt32(byte_length.Length);
            ins.LengthValue = Convert.ToUInt32(v.Length);
            if (v.SubTLVs.Count > 0)
                ins.Insides = v.SubTLVs.Count;
            insides.Add(ins);

            if (v.SubTLVs.Count > 0)
            {
                foreach (var sTLVs in v.SubTLVs)
                {
                    byte[] temp = Encoding_TLV(sTLVs, insides);
                    byte_value = byte_value.Concat(temp).ToArray();
                    //ins.LengthValue = Convert.ToUInt32(byte_value.Length);
                }
            }
            else
            {
                byte_value = v.Value;
                //ins.LengthValue = Convert.ToUInt32(byte_value.Length);
            }

            //insides.Add(ins);
            byte[] result = byte_type.Concat(byte_length).Concat(byte_value).ToArray();
            return result;
        }
        #endregion

        #endregion
    }
}
