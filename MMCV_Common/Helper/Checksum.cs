using System;
using System.IO;
using System.Security.Cryptography;

namespace MMCV_Common.Helper
{
    public static class Checksum
    {
        public static string CreateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string CreateMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                byte[] byteArray = md5.ComputeHash(stream);

                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(byteArray.Length * 2);
                foreach (byte byteMember in byteArray)
                {
                    stringBuilder.AppendFormat("{0:x2}", byteMember);
                }

                var check = stringBuilder.ToString();

                return check;
            }
        }

        public static string GenerateReferenceCode(int key = 0)
        {
            string[] str = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M" };
            DateTime now = DateTime.Now;
            string dtString = $"{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
            int countNumber = key.ToString().Length;
            dtString = key.ToString().Length < 17 ? $"{key}{dtString.Remove(0, countNumber)}" : key.ToString();
            long num = Convert.ToInt64(dtString);
            long d = 0;
            string decode = "";
            do
            {
                d = num % str.Length;
                num = (num / str.Length);
                decode += str[d];
            } while (num > 0);

            return decode;
        }
    }
}
