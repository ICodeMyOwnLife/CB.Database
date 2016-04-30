using System;
using System.Collections.Generic;
using System.Linq;


namespace CB.Database.TestEntityFramwork
{
    class Program
    {
        #region Fields
        private static readonly SchoolDataAccess _dataAccess = new SchoolDataAccess();
        #endregion


        #region Implementation
        private static void ListClasses(IEnumerable<Class> classes)
        {
            Console.WriteLine($"Classes:\r\n{string.Join("\r\n", classes.Select(c => c.ToString()))}");
        }

        private static void Main()
        {
            var classes = _dataAccess.GetClasses();

            SetClassForSubjects(classes);
            SetClassesForStudents(classes);
        }

        private static void SetClassesForStudent(Student student, Class[] classes)
        {
            Console.WriteLine($"Select classes for student {student}:");
            var classIds = Console.ReadLine().Split(',').Select(int.Parse);
            student.Classes = classIds.Select(classId => classes.FirstOrDefault(c => c.Id == classId)).ToList();
            _dataAccess.SaveStudent(student);
        }

        private static void SetClassesForStudents(Class[] classes)
        {
            var students = _dataAccess.GetStudents();
            ListClasses(classes);

            foreach (var student in students)
            {
                SetClassesForStudent(student, classes);
            }
        }

        private static void SetClassForSubject(Subject subject, IEnumerable<Class> classes)
        {
            Console.WriteLine($"Select class for subject {subject}:");
            var classId = int.Parse(Console.ReadLine());
            subject.Class = classes.FirstOrDefault(c => c.Id == classId);
            _dataAccess.SaveSubject(subject);
        }

        private static void SetClassForSubjects(Class[] classes)
        {
            ListClasses(classes);

            var subjects = new[]
            {
                new Subject { Name = "Data Structure & Algorithm" },
                new Subject { Name = "Graph Theory" },
                new Subject { Name = "Object-Oriented Programming" },
                new Subject { Name = "Xml Technology" },
                new Subject { Name = "Basic Database" },
                new Subject { Name = "Database Management Systems" },
                new Subject { Name = "Advance Database" },
                new Subject { Name = "Cryptography" },
                new Subject { Name = "Design Pattern" },
                new Subject { Name = "Web Programming" }
            };

            foreach (var subject in subjects)
            {
                SetClassForSubject(subject, classes);
            }
        }
        #endregion
    }
}