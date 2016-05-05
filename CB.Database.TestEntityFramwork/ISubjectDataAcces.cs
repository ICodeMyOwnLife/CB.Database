namespace CB.Database.TestEntityFramwork
{
    public interface ISubjectDataAcces
    {
        #region Abstract
        void DeleteSubject(int id);
        Subject GetSubject(int id);
        Subject[] GetSubjects();
        Subject SaveSubject(Subject subject);
        #endregion
    }
}