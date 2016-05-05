using System.Data.Entity;


namespace CB.Database.TestEntityFramwork
{
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
}