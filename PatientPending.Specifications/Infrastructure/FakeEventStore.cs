using System;
using System.Collections.Generic;
using PatientPending.Core;

namespace PatientPending.Specifications {
	public class FakeEventStore : IEventStore
	{
		private readonly List<Event> _initialEvents = new List<Event>();
		private readonly List<Event> _changes = new List<Event>(); 

		public FakeEventStore(IEnumerable<Event> events)
		{
			_initialEvents.AddRange(events);
		}

		public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion) {
			_changes.AddRange(events);
		}

		public List<Event> GetEventsForAggregate(Guid aggregateId)
		{
			return _initialEvents;
		}

		public List<Event> GetAllEvents()
		{
			var fullList = new List<Event>(_initialEvents);
			fullList.AddRange(_changes);
			return fullList;
		}

		public IList<Event> PeekChanges()
		{
			return _changes;
		}
	}
}