using System;
using NUnit.Framework;
using PatientPending.Core;
using Simple.Testing;

namespace PatientPending.Specifications {
    [TestFixture]
    public class A_adding_a_patient : ContainsSpecifications {
        private static readonly int UserId = 1;
        private static readonly Guid PatientId = Guid.NewGuid();

        public Specification successful_adding_of_patient =
            new EventSpecification<AddPatient>
            {
                Handler = store => new PatientCommandHandlers(new Repository<Patient>(store)),
                When = new AddPatient(UserId, PatientId, "Test", "Patient", "A", Title.Mr, Gender.Male, DateTime.Parse("14/01/1981"), new NHSNumber("401 023 2137")),
                Expect = {
		                new PatientAddedEventBuilder()
						    .WithId(PatientId)
						    .WithFirstName("Test")
						    .WithSurname("Patient")
                            .WithMiddleName("A")
                            .WithTitle(Title.Mr)
                            .WithGender(Gender.Male)
							.WithDoB(DateTime.Parse("14/01/1981"))
						    .WithNhsNumber(new NHSNumber("401 023 2137"))
						    .Build()
		            }
            };
    }
}