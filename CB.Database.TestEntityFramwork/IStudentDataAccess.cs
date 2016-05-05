namespace CB.Database.TestEntityFramwork
{
    public interface IStudentDataAccess
    {
        #region Abstract
        void DeleteStudent(int id);
        Student GetStudent(int id);
        Student[] GetStudents();
        Student SaveStudent(Student student);
        #endregion
    }
}