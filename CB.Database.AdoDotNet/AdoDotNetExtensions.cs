using System.Data;


namespace CB.Database.AdoDotNet
{
    public static class AdoDotNetExtensions
    {
        #region Methods
        public static DataRow GetFirstRow(this DataTable dataTable)
        {
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
        }

        public static DataRow GetFirstRow(this DataSet dataSet)
        {
            return dataSet.GetFirstTable()?.GetFirstRow();
        }

        public static DataTable GetFirstTable(this DataSet dataSet)
        {
            return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
        }

        public static object GetFirstValue(this DataRow dataRow)
        {
            return dataRow.ItemArray.Length > 0 ? dataRow[0] : null;
        }

        public static object GetFirstValue(this DataTable dataTable)
        {
            return dataTable.GetFirstRow()?.GetFirstValue();
        }

        public static object GetFirstValue(this DataSet dataSet)
        {
            return dataSet.GetFirstTable()?.GetFirstValue();
        }
        #endregion
    }
}