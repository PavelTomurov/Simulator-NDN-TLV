using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PacketCreator
{
    /// <summary>
    /// Именной компонент
    /// </summary>
    public static class Name
    {
        /// <summary>
        /// Конструктор подэлементов именного компонента пакета интереса
        /// </summary>
        public static List<TLV> GetNameElements(string full_name)
        {
            List<string> path_elements = full_name.Split('\\').ToList();
            List<TLV> name_tlvs = new List<TLV>();
            foreach (var p in path_elements)
                name_tlvs.Add(CreatorHelper.Create_TLV(8, p));
            return name_tlvs;
        }
    }
}
