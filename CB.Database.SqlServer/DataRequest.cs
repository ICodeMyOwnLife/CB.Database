using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;


namespace CB.Database.SqlServer
{
    public class DataRequest
    {
        #region  Constructors & Destructor
        public DataRequest(string databaseName, string procedureName, object parameters)
            : this(databaseName, procedureName, CreateParameters(parameters)) { }

        public DataRequest(string databaseName, string procedureName, Dictionary<string, object> parameters)
        {
            DatabaseName = databaseName;
            Parameters = parameters;
            ProcedureName = procedureName;
        }

        public DataRequest() { }
        #endregion


        #region  Properties & Indexers
        public string DatabaseName { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public string ProcedureName { get; set; }
        public string Query => string.IsNullOrEmpty(DatabaseName) ? ProcedureName : $"{DatabaseName}..{ProcedureName}";
        #endregion


        #region Implementation
        private static string CreateParameterName(string value)
        {
            if (!value.StartsWith("@")) value = "@" + value;
            return value;
        }

        private static Dictionary<string, object> CreateParameters(object parameters)
        {
            return parameters?.GetType().GetProperties().ToDictionary(p => CreateParameterName(p.Name),
                p => p.GetValue(parameters));
        }
        #endregion
    }
}