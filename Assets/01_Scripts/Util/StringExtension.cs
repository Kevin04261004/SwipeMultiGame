using System.Text;

namespace Util
{
    public static class StringExtension
    {
        public static byte[] ChangeToByte(this string str)
        {
            // 변환할때 Encoding을 맞춰주기 위함.
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
