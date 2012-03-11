using System;
using System.Text;

namespace PatientPending.Core {
    public class PatientAdded : Event {
        public Guid PatientId;
        public string FirstName;
        public string Surname;
        public Title Title;
        public NHSNumber NhsNumber;

        public PatientAdded(int userId, Guid patientId, string firstname, string surname,
            Title title, NHSNumber nhsNumber) 
            : base(userId) {
            PatientId = patientId;
            FirstName = firstname;
            Surname = surname;
            Title = title;
            NhsNumber = nhsNumber;
        }
    }

    public class PatientAdded_v2 : Event
    {
        public Guid PatientId;
        public string FirstName;
        public string Surname;
        public string MiddleName;
        public Title Title;
        public Gender Gender;
        public DateTime DateOfBirth;
        public NHSNumber NhsNumber;

        public PatientAdded_v2(int userId, Guid patientId, string firstname, string surname, string middlename,
            Title title, Gender gender, DateTime dateOfBirth, NHSNumber nhsNumber)
            : base(userId)
        {
            PatientId = patientId;
            FirstName = firstname;
            Surname = surname;
            MiddleName = middlename;
            Title = title;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            NhsNumber = nhsNumber;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("A Project named '{0} {1} {2}' with Id {3} was added.", Title, FirstName, Surname, PatientId);
            sb.AppendWithIndent("With NHS Number :{0}", NhsNumber);
            sb.AppendWithIndent("With a middle name of '{0}'", MiddleName);
            sb.AppendWithIndent("With a gender of '{0}'", Gender);
            sb.AppendWithIndent("With a date of birth of {0}", DateOfBirth.ToLongDateString());
            return sb.ToString().TrimStart('\n');
        }
    }
}