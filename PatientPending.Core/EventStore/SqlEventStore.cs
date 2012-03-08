using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PatientPending.Core.Bus;

namespace PatientPending.Core
{
	public class SqlEventStore : SqlActuator, IEventStore
	{
		private readonly IEventPublisher _publisher;

        public SqlEventStore(IEventPublisher publisher, string connectionStringName = "")
            : base(connectionStringName)
		{
			_publisher = publisher;
		}

		public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
		{
			var commands = new List<DbCommand>();
			// Check if aggregate exists, add if doesn't
			var version = Scalar("SELECT Version FROM Aggregates WHERE AggregateId=@0", aggregateId);
			if(version == null) {
				commands.Add(CreateCommand("INSERT INTO Aggregates (AggregateId, Type, Version) VALUES (@0,@1,@2)", null, aggregateId, typeof (Event).Name, 0));
				version = 0;
			}
			// Perform concurrency validation
			if ((int)version != expectedVersion && expectedVersion != -1) {
				throw new ConcurrencyException();
			}
			// iterate through events and create inserts, serialising the body of the event
			var i = expectedVersion;
			foreach (var @event in events) {
				i++;
				@event.Version = i;
				var serialisedEvent = JsonConvert.SerializeObject(@event, new StringEnumConverter());
				commands.Add(CreateCommand("INSERT INTO Events (EventId, AggregateId, Data, Type, Version) VALUES (@0,@1,@2,@3,@4)", null, 
					@event.Id, aggregateId, serialisedEvent, @event.GetType().AssemblyQualifiedName, i));
			}
			// update the expected version at the aggregate
			commands.Add(CreateCommand("UPDATE Aggregates SET Version=@0 WHERE AggregateId=@1", null, i, aggregateId));
			// validate the transation was successful
			if (Execute(commands) != commands.Count)
				throw new DBConcurrencyException("Not all commands where executed successfully.");
			// only once events have been persisted, publish the events
			foreach (var @event in events) {
				_publisher.Publish(@event);
			}
		}

		//NOTE: this could be slow depending on event volume
		public List<Event> GetEventsForAggregate(Guid aggregateId)
		{
			var result = Fetch("SELECT * FROM Events WHERE AggregateId = @0 ORDER BY Version", aggregateId);
			return result.Select(item => (Event)DeserializeObject(item.Data, item.Type)).ToList();
		}

        public List<Event> GetAllEvents()
        {
            var result = Fetch("SELECT * FROM Events ORDER BY Version");
            return result.Select(item => (Event)DeserializeObject(item.Data, item.Type)).ToList();
        }

	    private static object DeserializeObject(string data, string type)
	    {
	        var domainEvent = JsonConvert.DeserializeObject(data, Type.GetType(type));
	        return new EventConverter().Convert(domainEvent);
	    }

		public IList<Event> PeekChanges() {
			throw new NotImplementedException();
		}
	}
}