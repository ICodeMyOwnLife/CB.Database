using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Xml.Serialization;
using CB.Database.EntityFramework;
using CB.Model.Common;


namespace CB.Database.TestEntityFramwork

{
    public class StudentMap: EntityTypeConfiguration<Student>
    {
        #region  Constructors & Destructor
        public StudentMap()
        {
            HasMany(s => s.Classes).WithMany(c => c.Students);
        }
        #endregion
    }

    public interface IStudentDataAccess
    {
        #region Abstract
        void DeleteStudent(int id);
        Student GetStudent(int id);
        Student[] GetStudents();
        Student SaveStudent(Student student);
        #endregion
    }

    public interface IClassDataAccess
    {
        #region Abstract
        void DeleteClass(int id);
        Class GetClass(int id);
        Class[] GetClasses();
        Class SaveClass(Class @class);
        #endregion
    }

    public interface ISubjectDataAcces
    {
        #region Abstract
        void DeleteSubject(int id);
        Subject GetSubject(int id);
        Subject[] GetSubjects();
        Subject SaveSubject(Subject subject);
        #endregion
    }

    public interface ISchoolDataAccess: IStudentDataAccess, IClassDataAccess, ISubjectDataAcces { }

    public class SchoolDataAccess: ModelDbContextBase<SchoolContext>, ISchoolDataAccess
    {
        #region Methods
        public void DeleteClass(int id)
            => DeleteModel<Class>(id);

        public void DeleteStudent(int id)
            => DeleteModel<Student>(id);

        public void DeleteSubject(int id)
            => DeleteModel<Subject>(id);

        public Class GetClass(int id)
            => GetModel<Class>(id);

        public Class[] GetClasses()
            => GetModelsWithNoTracking<Class>();

        public Student GetStudent(int id)
            => GetModel<Student>(id);

        public Student[] GetStudents()
            => GetModelsWithNoTracking<Student>();

        public Subject GetSubject(int id)
            => GetModel<Subject>(id);

        public Subject[] GetSubjects()
            => GetModelsWithNoTracking<Subject>();

        public Class SaveClass(Class @class)
            => SaveModel(@class);

        public Student SaveStudent(Student student)
            => SaveModel(student);

        public Subject SaveSubject(Subject subject)
            => SaveModel(subject);
        #endregion
    }

    public class SubjectMap: EntityTypeConfiguration<Subject> { }

    public class ClassMap: EntityTypeConfiguration<Class>
    {
        #region  Constructors & Destructor
        public ClassMap()
        {
            HasMany(c => c.Subjects).WithRequired(s => s.Class);
        }
        #endregion
    }

    public class SchoolContext: DbContext
    {
        #region  Constructors & Destructor
        public SchoolContext(): base("SchoolContext")
        {
            System.Data.Entity.Database.SetInitializer(new SchoolDatabaseInitializer());
        }
        #endregion


        #region  Properties & Indexers
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        #endregion


        #region Override
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new StudentMap());
            modelBuilder.Configurations.Add(new ClassMap());
            modelBuilder.Configurations.Add(new SubjectMap());
        }
        #endregion
    }

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

    [Serializable]
    public class Subject: IdNameModelBase
    {
        #region Fields
        [NonSerialized]
        private Class _class;
        #endregion


        #region  Properties & Indexers
        [XmlIgnore]
        [ToString(OrderIndex = 20)]
        public Class Class
        {
            get { return _class; }
            set { SetProperty(ref _class, value); }
        }
        #endregion
    }

    public class SchoolDatabaseInitializer: DropCreateDatabaseAlways<SchoolContext>
    {
        #region Override
        protected override void Seed(SchoolContext context)
        {
            base.Seed(context);

            context.Students.Add(new Student { Name = "David" });
            context.Students.Add(new Student { Name = "Henry" });
            context.Students.Add(new Student { Name = "Paul" });
            context.Students.Add(new Student { Name = "Mary" });
            context.Students.Add(new Student { Name = "George" });
            context.Students.Add(new Student { Name = "Josh" });
            context.Students.Add(new Student { Name = "Sarah" });

            context.Classes.Add(new Class { Name = "Computer Science" });
            context.Classes.Add(new Class { Name = "Database Scientist" });
            context.Classes.Add(new Class { Name = "Networking & Security" });
            context.Classes.Add(new Class { Name = "Information Technology" });
        }
        #endregion
    }

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


// TODO: Test New SaveModel (One-to-many relationship and many-to-many relationship)
// TODO: Use context.Set<T>