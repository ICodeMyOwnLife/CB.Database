using CB.Database.EntityFramework;


namespace CB.Database.TestEntityFramwork
{
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
}