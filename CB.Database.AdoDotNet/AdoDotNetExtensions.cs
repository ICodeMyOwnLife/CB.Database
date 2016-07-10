using System;
using System.Data;


namespace CB.Database.AdoDotNet
{
    public static class AdoDotNetExtensions
    {
        #region Methods
        public static bool GetBoolean(this IDataRecord reader, string columnName)
            => Convert.ToBoolean(reader[columnName]);

        public static byte GetByte(this IDataRecord reader, string columnName)
            => Convert.ToByte(reader[columnName]);

        public static char GetChar(this IDataRecord reader, string columnName)
            => Convert.ToChar(reader[columnName]);

        public static DateTime GetDateTime(this IDataRecord reader, string columnName)
            => Convert.ToDateTime(reader[columnName]);

        public static DataRow GetFirstRow(this DataTable dataTable)
            => dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;

        public static DataRow GetFirstRow(this DataSet dataSet)
            => dataSet.GetFirstTable()?.GetFirstRow();

        public static DataTable GetFirstTable(this DataSet dataSet)
            => dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;

        public static object GetFirstValue(this DataRow dataRow)
            => dataRow.ItemArray.Length > 0 ? dataRow[0] : null;

        public static object GetFirstValue(this DataTable dataTable)
            => dataTable.GetFirstRow()?.GetFirstValue();

        public static object GetFirstValue(this DataSet dataSet)
            => dataSet.GetFirstTable()?.GetFirstValue();

        public static short GetInt16(this IDataRecord reader, string columnName)
            => Convert.ToInt16(reader[columnName]);

        public static int GetInt32(this IDataRecord reader, string columnName)
            => Convert.ToInt32(reader[columnName]);

        public static long GetInt64(this IDataRecord reader, string columnName)
            => Convert.ToInt64(reader[columnName]);

        public static sbyte GetSByte(this IDataRecord reader, string columnName)
            => Convert.ToSByte(reader[columnName]);

        public static string GetString(this IDataRecord reader, string columnName)
            => Convert.ToString(reader[columnName]);

        public static ushort GetUInt16(this IDataRecord reader, string columnName)
            => Convert.ToUInt16(reader[columnName]);

        public static uint GetUInt32(this IDataRecord reader, string columnName)
            => Convert.ToUInt32(reader[columnName]);

        public static ulong GetUInt64(this IDataRecord reader, string columnName)
            => Convert.ToUInt64(reader[columnName]);

        public static bool? ReadBoolean(this IDataRecord reader, string columnName)
            => DataConvert.ToBoolean(reader[columnName]);

        public static bool? ReadBoolean(this IDataRecord reader, int index)
            => DataConvert.ToBoolean(reader[index]);

        public static byte? ReadByte(this IDataRecord reader, string columnName)
            => DataConvert.ToByte(reader[columnName]);

        public static byte? ReadByte(this IDataRecord reader, int index)
            => DataConvert.ToByte(reader[index]);

        public static char? ReadChar(this IDataRecord reader, string columnName)
            => DataConvert.ToChar(reader[columnName]);

        public static char? ReadChar(this IDataRecord reader, int index)
            => DataConvert.ToChar(reader[index]);

        public static DateTime? ReadDateTime(this IDataRecord reader, string columnName)
            => DataConvert.ToDateTime(reader[columnName]);

        public static DateTime? ReadDateTime(this IDataRecord reader, int index)
            => DataConvert.ToDateTime(reader[index]);

        public static short? ReadInt16(this IDataRecord reader, string columnName)
            => DataConvert.ToInt16(reader[columnName]);

        public static short? ReadInt16(this IDataRecord reader, int index)
            => DataConvert.ToInt16(reader[index]);

        public static int? ReadInt32(this IDataRecord reader, string columnName)
            => DataConvert.ToInt32(reader[columnName]);

        public static int? ReadInt32(this IDataRecord reader, int index)
            => DataConvert.ToInt32(reader[index]);

        public static long? ReadInt64(this IDataRecord reader, string columnName)
            => DataConvert.ToInt64(reader[columnName]);

        public static long? ReadInt64(this IDataRecord reader, int index)
            => DataConvert.ToInt64(reader[index]);

        public static sbyte? ReadSByte(this IDataRecord reader, string columnName)
            => DataConvert.ToSByte(reader[columnName]);

        public static sbyte? ReadSByte(this IDataRecord reader, int index)
            => DataConvert.ToSByte(reader[index]);

        public static string ReadString(this IDataRecord reader, string columnName)
            => DataConvert.ToString(reader[columnName]);

        public static string ReadString(this IDataRecord reader, int index)
            => DataConvert.ToString(reader[index]);

        public static ushort? ReadUInt16(this IDataRecord reader, string columnName)
            => DataConvert.ToUInt16(reader[columnName]);

        public static ushort? ReadUInt16(this IDataRecord reader, int index)
            => DataConvert.ToUInt16(reader[index]);

        public static uint? ReadUInt32(this IDataRecord reader, string columnName)
            => DataConvert.ToUInt32(reader[columnName]);

        public static uint? ReadUInt32(this IDataRecord reader, int index)
            => DataConvert.ToUInt32(reader[index]);

        public static ulong? ReadUInt64(this IDataRecord reader, string columnName)
            => DataConvert.ToUInt64(reader[columnName]);

        public static ulong? ReadUInt64(this IDataRecord reader, int index)
            => DataConvert.ToUInt64(reader[index]);
        #endregion
    }
}