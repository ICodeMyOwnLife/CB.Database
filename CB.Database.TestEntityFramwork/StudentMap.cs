using System.Data.Entity.ModelConfiguration;


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
}