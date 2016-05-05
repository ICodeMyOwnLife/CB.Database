using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CB.Model.Common;


namespace CB.Database.TestEntityFramwork
{
    [Serializable]
    public class Student: IdNameModelBase
    {
        #region Fields
        [NonSerialized]
        private ICollection<Class> _classes;
        #endregion


        #region  Properties & Indexers
        [XmlIgnore]
        [ToString(OrderIndex = 20)]
        public ICollection<Class> Classes
        {
            get { return _classes; }
            set { SetProperty(ref _classes, value); }
        }
        #endregion
    }
}


// TODO: Test New SaveModel (One-to-many relationship and many-to-many relationship)
// TODO: Use context.Set<T>