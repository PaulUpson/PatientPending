using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using PatientPending.Core;
using Simple.Testing;
using Simple.Testing.PAssert;

namespace PatientPending.Specifications
{
	public class FailingEventSpecification<TCommand, TException> : TypedSpecification<TException>
		where TCommand : Command
		where TException : Exception
	{
		public List<Expression<Action>> Before = new List<Expression<Action>>();
		public List<Expression<Func<TException, bool>>> Expect = new List<Expression<Func<TException, bool>>>();
		public Action Finally;
		public List<Event> Given = new List<Event>();
		public TCommand When;
		public string Name;

		public delegate ICommandHandler<TCommand> FactoryDelegate(IEventStore store);

		public FactoryDelegate Handler;

		public string GetName() { return Name; }

		public Action GetBefore()
		{
			return () =>
			{
				foreach (var expression in Before)
				{
					expression.Compile()();
				}
			};
		}

		public Delegate GetOn()
		{
			return new Func<CommandHandlerFeed>(() =>
			{
				var store = new FakeEventStore(Given);
				var cmdHandler = Handler(store);
				return new CommandHandlerFeed(store, cmdHandler);
			});
		}

		sealed class CommandHandlerFeed
		{
			public readonly IEventStore EventStore;
			public readonly ICommandHandler<TCommand> CommandHandler;

			public CommandHandlerFeed(IEventStore eventStore, ICommandHandler<TCommand> commandHandler)
			{
				EventStore = eventStore;
				CommandHandler = commandHandler;
			}
		}

		public Delegate GetWhen()
		{
			return new Func<CommandHandlerFeed, TException>(feed =>
			{
				try  {
					feed.CommandHandler.Handle(When);
					return null;
				}
				catch (TException exception) 
				{
					return exception;
				}
			});
		}

		public IEnumerable<Assertion<TException>> GetAssertions()
		{
			return Expect.Select(x => new PAssertion<TException>(x));
		}

		public Action GetFinally()
		{
			return Finally;
		}

		public void Document(RunResult result, TextWriter output = null)
		{
			PrintEvil.Document(output, result, Before, Given.ToArray(), When, string.Empty);
		}
	}


	public class EventSpecification<TCommand> : TypedSpecification<Event[]>
		where TCommand : Command
	{
		public List<Expression<Action>> Before = new List<Expression<Action>>();
		public List<Event> Expect = new List<Event>();
		public Action Finally;
		public List<Event> Given = new List<Event>();
		public TCommand When;
		public string Name;

		public delegate ICommandHandler<TCommand> FactoryDelegate(IEventStore store);

		public FactoryDelegate Handler;

		public string GetName() { return Name; }

		public Action GetBefore()
		{
			return () =>
					{
						foreach (var expression in Before)
						{
							expression.Compile()();
						}
					};
		}

		public Delegate GetOn()
		{
			return new Func<CommandHandlerFeed>(() =>
			        {
			            var store = new FakeEventStore(Given);
			            var cmdHandler = Handler(store);
			            return new CommandHandlerFeed(store, cmdHandler);
			        });
		}

		sealed class CommandHandlerFeed
		{
			public readonly IEventStore EventStore;
			public readonly ICommandHandler<TCommand> CommandHandler;

			public CommandHandlerFeed(IEventStore eventStore, ICommandHandler<TCommand> commandHandler)
			{
				EventStore = eventStore;
				CommandHandler = commandHandler;
			}
		}

		public Delegate GetWhen()
		{
			return new Func<CommandHandlerFeed, IList<Event>>(feed =>
			        {
			            feed.CommandHandler.Handle(When);
			            return feed.EventStore.PeekChanges();
			        });
		}

		public IEnumerable<Assertion<Event[]>> GetAssertions()
		{
			yield return new EventAssertions(Expect);
		}

		public Action GetFinally()
		{
			return Finally;
		}

		public void Document(RunResult result, TextWriter output = null)
		{
			PrintEvil.Document(output, result, Before, Given.ToArray(), When, string.Empty);
		}
	}

	static class PrintEvil {
		private static TextWriter _writer;

		private static void PrintAdjusted(string adj, string text)
		{
			bool first = true;
			foreach (var s in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
			{
				_writer.Write(first ? adj : new string(' ', adj.Length));
				_writer.WriteLine(s);
				first = false;
			}
		}
		
		public static void Document(RunResult result, List<Expression<Action>> before, Event[] given, Command when, string decisions) {
			Document(null, result, before, given, when, decisions);
		}

		public static void Document(TextWriter output, RunResult result, List<Expression<Action>> before, Event[] given, Command when, string decisions) {
			_writer = output ?? Console.Out;
			
			var passed = result.Passed ? "[Passed]" : "[Failed]";
			_writer.WriteLine("{2} Scenario {0} - {1}", SimpleExtensions.CleanupName(result.FoundOnMemberInfo.DeclaringType.Name), SimpleExtensions.CleanupName(result.Name), passed);

			if(before.Any())
			{
				_writer.WriteLine();
				_writer.WriteLine("Environment: ");
				foreach (var expression in before)
				{
					PrintAdjusted("   ", PAssert.CreateSimpleFormatFor(expression));
				}
			}

			if (given.Any())
			{
				_writer.WriteLine();
				_writer.WriteLine("Given:");

				for (int i = 0; i < given.Length; i++)
				{
					PrintAdjusted(string.Format(" {0}. ", (i + 1)), given[i].ToString().Trim());
				}
			}

			if (when != null)
			{
				_writer.WriteLine();
				PrintAdjusted("When: ", when.ToString().Trim());
			}

			_writer.WriteLine();
			_writer.WriteLine("Expectations:");
			foreach (var expectation in result.Expectations)
			{
				PrintAdjusted(string.Format("  {0} ", (expectation.Passed ? "[Passed]" : "[Failed]")), expectation.Text.Trim());
				if(!expectation.Passed && expectation.Exception != null)
				{
					PrintAdjusted("                ", expectation.Exception.Message);
				}
			}

			if(result.Thrown != null)
			{
				_writer.WriteLine("Specification failed:" + result.Message.Trim());
				_writer.WriteLine();
				_writer.WriteLine(result.Thrown);
			}

			if(decisions.Length > 0)
			{
				_writer.WriteLine();
				_writer.WriteLine("Decisions made:");
				PrintAdjusted("  ", decisions);
			}

			_writer.WriteLine(new string('-', 60));
			_writer.WriteLine();
		}		
	}
}