using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace CB.Database.TestEntityFramwork
{
    class Program
    {
        #region Fields
        private static readonly ClassEntityDataAccess _classDataAccess = new ClassEntityDataAccess();
        private static readonly StudentEntityDataAccess _studentDataAccess = new StudentEntityDataAccess();
        private static readonly SubjectEntityDataAccess _subjectDataAccess = new SubjectEntityDataAccess();
        #endregion


        #region Implementation
        /*private static void ListClasses(IEnumerable<Class> classes)
        {
            Console.WriteLine($"Classes:\r\n{string.Join("\r\n", classes.Select(c => c.ToString()))}");
        }*/

        private static void Main()
        {
            SetClassesForStudents();
            SetClassesForStudents();
            SetClassForSubjects();
            SetClassForSubjects();
        }

        private static void SetClassesForStudent(Student student)
        {
            var classes = _classDataAccess.GetEntities();
            ShowList(classes, "Classes");
            Console.WriteLine($"Select classes for student {student}:");
            var readLine = Console.ReadLine();
            if (readLine != null)
            {
                var classIds = readLine.Split(',').Select(int.Parse).ToArray();

                /*var s = _studentDataAccess.GetEntity(student.Id);
                s.Classes =
                    _classDataAccess.GetEntities().Where(c => c.Id != null && classIds.Contains(c.Id.Value)).ToList();
                student.Classes = classIds.Select(classId => classes.FirstOrDefault(c => c.Id == classId)).ToList();
                _studentDataAccess.SaveItem(student);
                ShowItem(_studentDataAccess.GetEntity(student.Id));*/

                using (var db = new SchoolContext())
                {
                    var std = db.Students.Include(s => s.Classes).FirstOrDefault(s => s.Id == student.Id);
                    if (std == null) return;

                    foreach (var @class in db.Classes)
                    {
                        if (@class.Id != null && !classIds.Contains(@class.Id.Value)) std.Classes.Remove(@class);
                        else if (!std.Classes.Contains(@class)) std.Classes.Add(@class);
                    }

                    db.SaveChanges();
                }
            }
        }

        private static void SetClassesForStudents()
        {
            var students = _studentDataAccess.GetEntities();

            foreach (var student in students)
            {
                SetClassesForStudent(student);
            }
        }

        private static void SetClassForSubject(Subject subject)
        {
            var classses = _classDataAccess.GetEntities();
            ShowList(classses, "Classes");
            Console.WriteLine($"Select class for subject {subject}:");
            var readLine = Console.ReadLine();
            if (readLine == null) return;

            var classId = int.Parse(readLine);

            //subject.Class = classes.FirstOrDefault(c => c.Id == classId);
            subject.ClassId = classId;
            _subjectDataAccess.SaveItem(subject);
            ShowItem(_subjectDataAccess.GetEntity(subject.Id));
        }

        private static void SetClassForSubjects()
        {
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
                SetClassForSubject(subject);
            }
        }

        private static void ShowItem<TItem>(TItem item)
        {
            Console.WriteLine(item);
        }

        private static void ShowList<TElement>(IEnumerable<TElement> list, string name = "List")
        {
            Console.WriteLine($"{name}:\r\n{string.Join("\r\n", list)}");
        }
        #endregion
    }
}