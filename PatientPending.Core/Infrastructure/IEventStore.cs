using System;
using System.Collections.Generic;

namespace PatientPending.Core
{
	public interface IEventStore
	{
		void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion);
		List<Event> GetEventsForAggregate(Guid aggregateId);
		List<Event> GetAllEvents();
		IList<Event> PeekChanges();
	}
}