using System.Collections.Generic;

namespace PacketCreator
{
    /// <summary>
    /// Пакет данных
    /// </summary>
    public class DataPacket
    {
        /// <summary>
        /// Тип
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Имя пакета данных
        /// </summary>
        public string NameString { get; set; }

        /// <summary>
        /// Метаданные
        /// </summary>
        public MetaInfo MetaInfo { get; set; }

        /// <summary>
        /// Контент
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Конструктор пакета данных
        /// </summary>
        public DataPacket(
            byte type,
            string full_name,
            MetaInfo metadata,
            byte[] content)
        {
            this.Type = type;
            this.NameString = full_name;
            this.MetaInfo = metadata;
            this.Content = content;
        }

        #region Создание TLV-блока для пакета данных
        /// <summary>
        /// Создание TLV-блока для пакета данных
        /// </summary>
        public TLV GetDataPacketTLV(DataPacket data_packet)
        {
            //Блоки TLV пакета данных
            List<TLV> data_packet_tlvs = new List<TLV>();

            //***Именной компонент***//

            List<TLV> generic_name_elements = Name.GetNameElements(data_packet.NameString);
            //Длина
            uint name_field_length = CreatorHelper.Find_TLVs_Length(generic_name_elements);
            //Блок TLV для имени - запроса
            TLV tlv_name = new TLV(7, name_field_length, generic_name_elements);
            data_packet_tlvs.Add(tlv_name);

            //***Метаданные***//

            //Блок TLV для типа контента
            TLV content_type = CreatorHelper.Create_TLV(0, data_packet.MetaInfo.ContentType);

            //Блок TLV для периода актуальности данных
            TLV freshness_period = CreatorHelper.Create_TLV(25, data_packet.MetaInfo.FreshnessPeriod);

            //Блок TLV для последнего элемента в абсолютном пути к файлу
            TLV final_block_id = CreatorHelper.Create_TLV(26, data_packet.MetaInfo.FinalBlockId);

            //Блоки TLV для метаданных
            List<TLV> metadata_tlvs = new List<TLV>();
            metadata_tlvs.Add(content_type);
            metadata_tlvs.Add(freshness_period);
            metadata_tlvs.Add(final_block_id);

            //Длина
            uint metadata_length = CreatorHelper.Find_TLVs_Length(metadata_tlvs);
            //Блок TLV
            TLV tlv_metadata = new TLV(20, metadata_length, metadata_tlvs);
            data_packet_tlvs.Add(tlv_metadata);

            //***Поле контента***//

            //Блок TLV
            TLV tlv_content = new TLV(21, data_packet.Content.Length, data_packet.Content);
            data_packet_tlvs.Add(tlv_content);

            //Длина
            uint datapacket_length = CreatorHelper.Find_TLVs_Length(data_packet_tlvs);

            //*** Блок TLV для пакета данных ***
            TLV datapacket_tlv = new TLV(data_packet.Type, datapacket_length, data_packet_tlvs);

            return datapacket_tlv;
        }
        #endregion
    }

    /// <summary>
    /// Метаданные
    /// </summary>
    public class MetaInfo
    {
        /// <summary>
        /// Тип контента
        /// </summary>
        public byte ContentType { get; set; }

        /// <summary>
        /// Период актуальности данных
        /// </summary>
        public ushort FreshnessPeriod { get; set; }

        /// <summary>
        /// Последний элемент в абсолютном пути к файлу
        /// </summary>
        public string FinalBlockId { get; set; }

        /// <summary>
        /// Конструктор метаданных
        /// </summary>
        public MetaInfo(byte content_type, ushort freshness_period, string final_block_id)
        {
            this.ContentType = content_type;
            this.FreshnessPeriod = freshness_period;
            this.FinalBlockId = final_block_id;
        }
    }

    
}
