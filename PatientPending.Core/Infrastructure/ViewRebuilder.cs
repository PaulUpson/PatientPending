using System.Threading;
using PatientPending.Core.Bus;

namespace PatientPending.Core {
	public class ViewRebuilder {
        /* This is for use on application start to rebuild the 
         * in memory views - should become obsolete once views
         * are persisted */
		private readonly IEventStore _store;
		private readonly IEventPublisher _publisher;

		public ViewRebuilder(IEventStore storage, IEventPublisher publisher) {
			_store = storage;
			_publisher = publisher;
		}

        /* This is a bit dirty but since the publish action
         * is threaded we need to delay when republishing
         * to avoid race conditions */
		public void RebuildAll(int milliseconds = 5) {
			var events = _store.GetAllEvents();
			foreach (var @event in events)
			{
				_publisher.Publish(@event);
                Thread.Sleep(milliseconds);
			}
		}
	}
}