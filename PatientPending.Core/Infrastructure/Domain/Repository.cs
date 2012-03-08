using System;

namespace PatientPending.Core
{
	public interface IRepository<T> where T : AggregateRoot, new()
	{
		void Save(AggregateRoot aggregate, int expectedVersion);
		T GetById(Guid id);
		bool TryGetById(Guid id, out T item);
	}

	public class Repository<T> : IRepository<T> where T: AggregateRoot, new() //shortcut you can do as you see fit with new()
	{
		private readonly IEventStore _storage;

		public Repository(IEventStore storage)
		{
			_storage = storage;
		}

		public void Save(AggregateRoot aggregate, int expectedVersion)
		{
			_storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
		}

		public T GetById(Guid id)
		{
			var obj = new T();//lots of ways to do this
			var e = _storage.GetEventsForAggregate(id);
			obj.LoadFromHistory(e);
			return obj;
		}

		public bool TryGetById(Guid id, out T item) {
			try {
				item = GetById(id);
				if (item.Id != id) { //fix for fake bus that doesnt throw
					item = null;
					return false;
				}
				return true;
			}
			catch(Exception e) {
				item = null;
				return false;
			}
		}
	}
}