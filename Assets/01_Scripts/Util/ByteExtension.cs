using System.Text;

namespace Util
{
    public static class ByteExtension
    {
        public static string ChangeToString(this byte[] bytes)
        {
            // 변환할때 Encoding을 맞춰주기 위함.
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
