namespace PatientPending.Core {
    public class PatientCommandHandlers : ICommandHandler<AddPatient> {
        private IRepository<Patient> _repository;
        
        public PatientCommandHandlers(IRepository<Patient> repository) {
            _repository = repository;
        }

        public void Handle(AddPatient message) {
            var item = new Patient(message.UserId, message.PatientId, message.FirstName, message.Surname,
                                   message.MiddleName, message.Title, message.Gender, message.DateOfBirth,
                                   message.NhsNumber);
            _repository.Save(item, -1);
        }
    }
}