using System;

namespace PatientPending.Core {
    public class Patient : AggregateRoot {
        private Guid _id;

        public void Apply(PatientAdded e) {
            _id = e.PatientId;
        }

        public Patient(int userId, Guid patientId, string firstname, string surname, string middlename,
            Title title, Gender gender, DateTime dateOfBirth, NHSNumber nhsNumber)
        {
            //validation rules here
            ApplyChange(new PatientAdded_v2(userId, patientId, firstname, surname, middlename, title, gender, dateOfBirth, nhsNumber));
        }

        public Patient() {}

        public override Guid Id {
            get { return _id; }
        }
    }
}