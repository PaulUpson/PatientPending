using System;
using System.Linq;
using NUnit.Framework;
using Simple.Testing;

namespace PatientPending.Specifications
{
	[TestFixture]
	public abstract class ContainsSpecifications
	{
		 protected TestCaseData[] GetSpecifications()
		 {
			 var type = GetType();
			 var runs = TypeReader.GetSpecificationsIn(type);
		 	return runs
				.Select(s => new TestCaseData(s).SetName(Name(s)))
				.ToArray();
		 }

		static string Name(SpecificationToRun r)
		{
			return (r.Specification.GetName() ?? r.FoundOn.Name).CleanupName() + " ";
		}

		// helpers
		protected static DateTime DateUtc(int year, int month, int day)
		{
			return new DateTime(year, month, day, 0,0,0, DateTimeKind.Unspecified);
		}

		[TestCaseSource("GetSpecifications")]
		public void Verify(SpecificationToRun spec)
		{
			var result = new SpecificationRunner().RunSpecification(spec);
			spec.Specification.Document(result);
			if(!result.Passed)
			{
				if (result.Thrown != null)
				{
					throw result.Thrown;
				}
				Assert.Fail(result.Message);
			}
		}
	}
}