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
        public ICollection<Class> Classes
        {
            get { return _classes; }
            set { SetProperty(ref _classes, value); }
        }
        #endregion
    }

    public class Class: IdNameModelBase
    {
        #region Fields
        [NonSerialized]
        private ICollection<Student> _students;
        #endregion


        #region  Properties & Indexers
        [XmlIgnore]
        public ICollection<Student> Students
        {
            get { return _students; }
            set { SetProperty(ref _students, value); }
        }
        #endregion
    }
}


// TODO: Test New SaveModel (One-to-many relationship and many-to-many relationship)