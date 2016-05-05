using System.Data.Entity;


namespace CB.Database.TestEntityFramwork
{
    public class SchoolDatabaseInitializer: DropCreateDatabaseAlways<SchoolContext>
    {
        #region Override
        protected override void Seed(SchoolContext context)
        {
            base.Seed(context);

            context.Students.Add(new Student { Name = "David" });
            context.Students.Add(new Student { Name = "Henry" });
            context.Students.Add(new Student { Name = "Paul" });
            context.Students.Add(new Student { Name = "Mary" });
            context.Students.Add(new Student { Name = "George" });
            context.Students.Add(new Student { Name = "Josh" });
            context.Students.Add(new Student { Name = "Sarah" });

            context.Classes.Add(new Class { Name = "Computer Science" });
            context.Classes.Add(new Class { Name = "Database Scientist" });
            context.Classes.Add(new Class { Name = "Networking & Security" });
            context.Classes.Add(new Class { Name = "Information Technology" });
        }
        #endregion
    }
}