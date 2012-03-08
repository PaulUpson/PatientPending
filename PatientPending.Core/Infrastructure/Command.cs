namespace PatientPending.Core {
	public class Command : IMessage
	{
		public readonly int UserId;

		public Command(int userId)
		{
			UserId = userId;
		}
	}
}