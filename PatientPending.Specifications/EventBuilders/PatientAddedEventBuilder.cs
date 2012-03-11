using System;
using PatientPending.Core;

namespace PatientPending.Specifications {
    public class PatientAddedEventBuilder {
        private readonly int _userId = 1;
        private Guid _patientId;
        private string _firstName;
        private string _surname;
        private string _middleName;
        private Title _title;
        private DateTime _dateOfBirth;
        private Gender _gender;
        private NHSNumber _nhsNumber;

        public PatientAddedEventBuilder WithId(Guid patientId) {
            _patientId = patientId;
            return this;
        }

        public PatientAddedEventBuilder WithFirstName(string firstname) {
            _firstName = firstname;
            return this;
        }

        public PatientAddedEventBuilder WithSurname(string surname) {
            _surname = surname;
            return this;
        }

        public PatientAddedEventBuilder WithMiddleName(string middleName) {
            _middleName = middleName;
            return this;
        }

        public PatientAddedEventBuilder WithTitle(Title title) {
            _title = title;
            return this;
        }

        public PatientAddedEventBuilder WithGender(Gender gender) {
            _gender = gender;
            return this;
        }

        public PatientAddedEventBuilder WithDoB(DateTime dateOfBirth) {
            _dateOfBirth = dateOfBirth;
            return this;
        }

        public PatientAddedEventBuilder WithNhsNumber(NHSNumber nhsNumber) {
            _nhsNumber = nhsNumber;
            return this;
        }

        public PatientAdded_v2 Build() {
            return new PatientAdded_v2(_userId, _patientId, _firstName, _surname, _middleName, _title, _gender, _dateOfBirth, _nhsNumber);
        }
    }
}