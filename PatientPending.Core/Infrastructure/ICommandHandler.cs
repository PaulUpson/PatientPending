namespace PatientPending.Core {
	public interface ICommandHandler<T>
	{
		void Handle(T message);
	}
}