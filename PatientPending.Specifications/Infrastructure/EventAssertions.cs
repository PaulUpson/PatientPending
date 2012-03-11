using System;
using System.Collections.Generic;
using PatientPending.Core;
using Simple.Testing;

namespace PatientPending.Specifications
{
	public class EventAssertions : Assertion<Event[]>
	{
		private readonly List<Event> _expected; 

		public EventAssertions(List<Event> expected)
		{
			_expected = expected;
		}

		public IEnumerable<ExpectationResult> Assert(object fromWhen)
		{
			var actual = ((List<Event>) fromWhen);

			for (int i = 0; i < _expected.Count; i++)
			{
				var expectedHumanReadable =_expected[i].ToString();
				if (actual.Count > i)
				{
					var diffs = CompareObjects.FindDifferences(_expected[i], actual[i]);
					if(string.IsNullOrEmpty(diffs))
					{
						yield return new ExpectationResult
						             	{
						             		Passed = true, Text = expectedHumanReadable
						             	};
					}
					else
					{
						var actualHumanReadable = actual[i].ToString();

						if(actualHumanReadable != expectedHumanReadable)
						{
							yield return new ExpectationResult
							{
							    Passed = false,
							    Text = expectedHumanReadable,
							    Exception = new InvalidOperationException("Was:" + actualHumanReadable)
							};
						}
						else
						{
							yield return new ExpectationResult
							{
							    Passed = false,
							    Text = expectedHumanReadable,
							    Exception = new InvalidOperationException(diffs)
							};
						}
					}
				}
				else
				{
					yield return new ExpectationResult
					{
					    Passed = false,
					    Text = expectedHumanReadable,
					    Exception = new InvalidOperationException("Missing")
					};
				}
			}

			for (int i = _expected.Count; i < actual.Count; i++)
			{
				yield return new ExpectationResult
				             	{
				             		Passed = false,
				             		Text = "Unexpected message",
				             		Exception = new InvalidOperationException("Was:" + actual[i])
				             	};
			}
		}
	}
}