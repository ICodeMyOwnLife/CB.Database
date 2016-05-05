using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CB.Model.Common;


namespace CB.Database.TestEntityFramwork
{
    public class Class: IdNameModelBase
    {
        #region Fields
        [NonSerialized]
        private ICollection<Student> _students;

        [NonSerialized]
        private ICollection<Subject> _subjects;
        #endregion


        #region  Properties & Indexers
        [XmlIgnore]
        [ToString(OrderIndex = 20)]
        public ICollection<Student> Students
        {
            get { return _students; }
            set { SetProperty(ref _students, value); }
        }

        [XmlIgnore]
        [ToString(OrderIndex = 30)]
        public ICollection<Subject> Subjects
        {
            get { return _subjects; }
            set { SetProperty(ref _subjects, value); }
        }
        #endregion
    }
}