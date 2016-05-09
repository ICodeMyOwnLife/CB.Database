using System.Reflection;


namespace CB.Database.EntityFramework
{
    public class ModelInfo
    {
        #region  Properties & Indexers
        public PropertyInfo Id { get; set; }
        public PropertyInfo Property { get; set; }
        public PropertyInfo PropertyId { get; set; }
        #endregion


        #region Methods
        public object GetProperty(object source)
            => Property.GetValue(source);
        #endregion
    }
}