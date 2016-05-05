namespace CB.Database.TestEntityFramwork
{
    public interface IClassDataAccess
    {
        #region Abstract
        void DeleteClass(int id);
        Class GetClass(int id);
        Class[] GetClasses();
        Class SaveClass(Class @class);
        #endregion
    }
}