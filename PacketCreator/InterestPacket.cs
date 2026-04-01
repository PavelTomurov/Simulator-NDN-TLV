using System.Collections.Generic;

namespace PacketCreator
{
    /// <summary>
    /// Пакет интереса
    /// </summary>
    public class InterestPacket
    {
        /// <summary>
        /// Тип
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Сам запрос - имя пакета интереса
        /// </summary>
        public string NameString { get; set; }

        /// <summary>
        /// Приблизительное время, оставшееся до истечения срока действия интереса.
        /// </summary>
        public ushort InterstLifeTime { get; set; }

        /// <summary>
        /// Соответствуют ли данные указанному имени в запросе в точности
        /// </summary>
        public bool CanBePrefix { get; set; }

        /// <summary>
        /// Конструктор пакета интереса
        /// </summary>
        public InterestPacket(byte type, string full_name, ushort interstLifeTime, bool canBePrefix)
        {
            this.Type = type;
            this.NameString = full_name;
            this.InterstLifeTime = interstLifeTime;
            this.CanBePrefix = canBePrefix;
        }

        #region Создание TLV-блока для пакета интереса
        /// <summary>
        /// Создание TLV-блока для пакета интереса
        /// </summary>
        public TLV GetInterestPacketTLV(InterestPacket interest_packet)
        {
            //Блоки TLV пакета интереса
            List<TLV> interest_packet_tlvs = new List<TLV>();

            //Именной компонент
            List<TLV> generic_name_elements = Name.GetNameElements(interest_packet.NameString);
            //Длина
            uint name_field_length = CreatorHelper.Find_TLVs_Length(generic_name_elements);
            //Блок TLV для имени - запроса
            TLV tlv_name = new TLV(7, name_field_length, generic_name_elements);
            interest_packet_tlvs.Add(tlv_name);

            //Время актуальности запроса
            //Блок TLV для времени актуальности запроса
            TLV interest_life_time = CreatorHelper.Create_TLV(12, interest_packet.InterstLifeTime);
            interest_packet_tlvs.Add(interest_life_time);

            //Префикс
            //Блок TLV для префикса
            TLV can_be_prefix = CreatorHelper.Create_TLV(33, interest_packet.CanBePrefix);
            interest_packet_tlvs.Add(can_be_prefix);

            //Общая длина
            uint interest_packet_length = CreatorHelper.Find_TLVs_Length(interest_packet_tlvs);

            //*** Блок TLV для пакета интереса ***
            TLV interest_packet_tlv = new TLV(5, interest_packet_length, interest_packet_tlvs);

            return interest_packet_tlv;
        }
        #endregion
    }
}
