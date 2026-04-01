using PacketCreator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Search
{
    public static class Search
    {
        #region Поиск контента в базе данных по запросу
        /// <summary>
        /// Поиск контента в базе данных по запросу
        /// </summary>
        public static TLV Search_By_Request(TLV tlv_interest, List<TLV> tlvs)
        {
            for (int i = 0; i < tlvs.Count; i++)
            {
                //Именной блок пакета данных (внутри 4 TLV с типом 8 "D:\\files\\pictures\\a.JPG")
                var name = tlvs[i].SubTLVs[0];

                //Именной блок пакета интереса (запроса)
                var interest_search = tlv_interest.SubTLVs[0];

                //Количество элементов имени (GenericNameComponent) должно быть одинаковым
                //Пакет интереса:   C:\\files\\pictures
                //Пакет данных:     C:\\files\\pictures
                if (name.SubTLVs.Count != interest_search.SubTLVs.Count)
                    continue;
                for (int j = 0; j < interest_search.SubTLVs.Count; j++)
                {
                    //Сравнение D-D, files-files, pictures-pictures, a.JPG-Кот.JPG
                    var res = MemcmpCompare(interest_search.SubTLVs[j].Value, name.SubTLVs[j].Value);
                    //Если массивы байтов полностью одинаковые (не только по длине, но и по значению),
                    //То возвращается [0],
                    //Иначе,
                    //Если левый больше, то возвращается [1], если правый - то [-1]
                    if (res != 0)
                        break;
                    //Если цикл завершается НЕ ПРЕРЫВАНИЕМ, значит найден нужный контент
                    if (j == tlv_interest.SubTLVs[0].SubTLVs.Count - 1)
                    {
                        return tlvs[i];
                    }
                }
            }
            //В этой точке поиск заканчивается.
            //Если ранее не было ничего найдено, то функция возвращает пустоту - null.
            return null;
        }
        #endregion

        #region Поиск контента по байтам №1
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);
        /// <summary>
        /// Если массивы байтов полностью одинаковые (не только по длине, но и по значению),
        /// То возвращается [0],
        /// Иначе,
        /// Если левый больше, то возвращается [1], если правый - то [-1]
        /// </summary>
        public static int MemcmpCompare(byte[] b1, byte[] b2)
        {
            var retval = memcmp(b1, b2, new UIntPtr((uint)b1.Length));
            return retval;
        }
        #endregion

        #region Поиск контента по байтам №2
        /// <summary>
        /// Если массивы байтов полностью одинаковые (не только по длине, но и по значению),
        /// то возвращается true,
        /// иначе, при любом отличии возвращается false
        /// </summary>
        public static class ByteArrayExtensions
        {
            [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);

            /// <summary>
            /// Если массивы байтов полностью одинаковые (не только по длине, но и по значению),
            /// То возвращается true,
            /// Иначе, при любом отличии возвращается false
            /// </summary>
            public static bool SequenceEqual(byte[] b1, byte[] b2)
            {
                if (b1 == b2) return true; //reference equality check

                if (b1 == null || b2 == null || b1.Length != b2.Length) return false;

                return memcmp(b1, b2, new UIntPtr((uint)b1.Length)) == 0;
            }
        }
        #endregion
    }
}


