using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialParkHans
{
    public static class Extensions
    {
        public static IEnumerable<byte> Reverse(this int value) => BitConverter.GetBytes(value).Reverse();
        public static IEnumerable<byte> Reverse(this float value) => BitConverter.GetBytes(value).Reverse();
        public static int Switch(this int value) => BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        public static float Switch(this float value) => BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
    }
}
