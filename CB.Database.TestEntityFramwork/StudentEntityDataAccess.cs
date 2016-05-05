using CB.Database.EntityFramework;


namespace CB.Database.TestEntityFramwork
{
    public class StudentEntityDataAccess: NoTrackingEntityDataAccess<SchoolContext, Student>
    {
        #region  Constructors & Destructor
        public StudentEntityDataAccess()
        {
            RegisterDependent(s => s.Classes);
        }
        #endregion
    }
}