# Welcome to Patient Pending

Patient Pending is an attempt to collaboratively build a patient administration application that isn't complex to use, extend or maintain.

A secondary aim of this sample application is to further our knowledge of key software patterns and practices in a domain that we know relatively well.

Initial commit contained the basis of a light-weight CQRS 'framework' borrowing concepts from Greg Young's SimplestPossibleThing and Lokad.CQRS. It implements a very simple SQL Server event store and an example of an EventConverter for managing event versioning.

## Simple.Testing

We've just introduced [Simple.Testing](https://github.com/gregoryyoung/Simple.Testing) style specifications. The purpose of Simple.Testing (a concept initially devised by [Greg Young](http://www.twitter.com/gregyoung)) is to remove dependencies on 'heavy' testing frameworks whilst enabling the definition of testing specifications as close to the **Ubiquitous Language of the Business Domain**.  One of the key points Greg makes is that once you start introducing mocks into your tests you are inherently testing against artificial state.  Simple.Testing allows you to avoid a reliance on mocking frameworks and assert directly against the same state that will be arrived at in production.  Here's an example:

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

From the above example you can see that we are defining the Specification as being for the **AddPatient** event. We specify an appropriate handler for the command, we define our _When_ condition which in this case is an **AddPatient** command, and then we define our expectations, which is a collection of events.  You can see here that we are using a fluent event builder rather than specifying the event itself, and there's a good reason for this. One of the CQRS mantras is make only additive changes. So when we need to introduce additional functionality we introduce a new event (see PatientAdded_v2 for example). Now since our events are likely to change as the system evolves it's better to mask the actual event thats being expected so that when our events are up-versioned we do not change our tests, only the event returned by the builder.

The output when this specification is run looks like this:
	[Passed] Scenario A adding a patient - successful adding of patient

	When: Adding a Project named 'Mr Test Patient' with Id c16380f0-9391-4148-9196-7df0f4c6f06a.
			With NHS Number :401 023 2137
			With a middle name of 'A'
			With a gender of 'Male'
			With a date of birth of 14 January 1981

	Expectations:
	  [Passed] A Project named 'Mr Test Patient' with Id c16380f0-9391-4148-9196-7df0f4c6f06a was added.
			With NHS Number :401 023 2137
			With a middle name of 'A'
			With a gender of 'Male'
			With a date of birth of 14 January 1981
	------------------------------------------------------------

### TODO
* Extend the UI to show examples
* Extend the set of example Specifications
