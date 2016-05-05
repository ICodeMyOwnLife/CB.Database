using System;
using System.Xml.Serialization;
using CB.Model.Common;


namespace CB.Database.TestEntityFramwork
{
    [Serializable]
    public class Subject: IdNameModelBase
    {
        #region Fields
        [NonSerialized]
        private Class _class;

        private int _classId;
        #endregion


        #region  Properties & Indexers
        [XmlIgnore]
        [ToString(OrderIndex = 30)]
        public Class Class
        {
            get { return _class; }
            set { SetProperty(ref _class, value); }
        }

        [ToString(OrderIndex = 20)]
        public int ClassId
        {
            get { return _classId; }
            set { SetProperty(ref _classId, value); }
        }
        #endregion
    }
}