using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public static class StringExtension
    {
        public static string GetTableName(this string className)
        {
            int underscoreIndex = className.IndexOf('_');
            Debug.Assert(underscoreIndex != -1, "[ERROR] Class 이름에 _가 존재하지 않습니다.\nmySQL 클래스명 규칙: [DB명]_[Table명]");

            return className.Substring(underscoreIndex + 1);
        }
        public static byte[] ChangeToByte(this string str)
        {
            // 변환할때 Encoding을 맞춰주기 위함.
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
