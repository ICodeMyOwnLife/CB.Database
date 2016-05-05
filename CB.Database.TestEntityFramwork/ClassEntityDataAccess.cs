using CB.Database.EntityFramework;


namespace CB.Database.TestEntityFramwork
{
    public class ClassEntityDataAccess: NoTrackingEntityDataAccess<SchoolContext, Class>
    {
        #region  Constructors & Destructor
        public ClassEntityDataAccess()
        {
            RegisterDependent(c => c.Students);
        }
        #endregion
    }
}