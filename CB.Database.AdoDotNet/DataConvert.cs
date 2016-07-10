using System;


namespace CB.Database.AdoDotNet
{
    public static class DataConvert
    {
        #region Methods
        public static bool? ToBoolean(object value)
            => value == DBNull.Value ? (bool?)null : Convert.ToBoolean(value);

        public static byte? ToByte(object value)
            => value == DBNull.Value ? (byte?)null : Convert.ToByte(value);

        public static char? ToChar(object value)
            => value == DBNull.Value ? (char?)null : Convert.ToChar(value);

        public static DateTime? ToDateTime(object value)
            => value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(value);

        public static short? ToInt16(object value)
            => value == DBNull.Value ? (short?)null : Convert.ToInt16(value);

        public static int? ToInt32(object value)
            => value == DBNull.Value ? (int?)null : Convert.ToInt32(value);

        public static long? ToInt64(object value)
            => value == DBNull.Value ? (long?)null : Convert.ToInt64(value);

        public static sbyte? ToSByte(object value)
            => value == DBNull.Value ? (sbyte?)null : Convert.ToSByte(value);

        public static string ToString(object value)
            => value == DBNull.Value ? null : Convert.ToString(value);

        public static ushort? ToUInt16(object value)
            => value == DBNull.Value ? (ushort?)null : Convert.ToUInt16(value);

        public static uint? ToUInt32(object value)
            => value == DBNull.Value ? (uint?)null : Convert.ToUInt32(value);

        public static ulong? ToUInt64(object value)
            => value == DBNull.Value ? (ulong?)null : Convert.ToUInt64(value);
        #endregion
    }
}