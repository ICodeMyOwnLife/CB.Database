using CB.Database.EntityFramework;


namespace CB.Database.TestEntityFramwork
{
    public class SubjectEntityDataAccess: NoTrackingEntityDataAccess<SchoolContext, Subject>
    {
        #region  Constructors & Destructor
        public SubjectEntityDataAccess()
        {
            RegisterDependent(s => s.Class);
        }
        #endregion
    }
}