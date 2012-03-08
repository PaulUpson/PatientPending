using System;

namespace PatientPending.Core {
	public class Event : IMessage {
		public Guid Id;
		public int Version;
		public DateTime Timestamp;
		public int UserId;

		public Event(int userId) {
			Id = Guid.NewGuid();
			Timestamp = DateTime.UtcNow;
			UserId = userId;
		}
	}
}