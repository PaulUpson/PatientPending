using System;

namespace PatientPending.Core {
    public class AddPatient : Command {
        public Guid PatientId;
        public string FirstName;
        public string Surname;
        public string MiddleName;
        public Title Title;
        public Gender Gender;
        public DateTime DateOfBirth;
        public NHSNumber NhsNumber;

        public AddPatient(int userId, Guid patientId, string firstname, string surname, string middlename, 
            Title title, Gender gender, DateTime dateOfBirth, NHSNumber nhsNumber) 
            : base(userId) {
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