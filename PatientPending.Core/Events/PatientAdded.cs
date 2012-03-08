using System;

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
    }
}