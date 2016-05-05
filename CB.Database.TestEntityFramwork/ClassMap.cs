using System.Data.Entity.ModelConfiguration;


namespace CB.Database.TestEntityFramwork
{
    public class ClassMap: EntityTypeConfiguration<Class>
    {
        #region  Constructors & Destructor
        public ClassMap()
        {
            HasMany(c => c.Subjects).WithRequired(s => s.Class);
        }
        #endregion
    }
}