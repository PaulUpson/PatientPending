using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Simple.Testing;

namespace SimpleTests.Runner
{
	class Program
	{
		static void Main(string[] args) {
			bool showHelp = false;
			string outputFile = null;
			IEnumerable<string> assembles = Enumerable.Empty<string>();

			var optionSet = new Options {
			        {"h|help", "show this message and exit", x => showHelp = x != null},
			        {"a=|assembles=", "comma-seperated list of names of assemblies to test", x => assembles = x.Split(',') },
					{"o=|outputFile=", "send output to the text file specified rather than console", x => outputFile = x }
			};

			try
			{
				optionSet.Parse(args);
				if (showHelp)
				{
					ShowHelp(optionSet);
					return;
				}
				if (!assembles.Any())
				{
					throw new InvalidOperationException("No assemblies specified.");
				}
			}
			catch (InvalidOperationException exception)
			{
				Console.Write(string.Format("{0}: ", AppDomain.CurrentDomain.FriendlyName));
				Console.WriteLine(exception.Message);
				Console.WriteLine("Try {0} --help for more infomation", AppDomain.CurrentDomain.FriendlyName);
				return;
			}
			Console.WriteLine("Running specifications ...");
			using (var sw = GetTextWriter(outputFile))
			{
				SimpleRunner.SetWriter(sw);
				assembles.ForEach(SimpleRunner.RunAllInAssembly);
			}
			Console.WriteLine("... Finished running specifications");
		}

		private static void ShowHelp(Options optionSet) {
			Console.WriteLine("Test specification runner for Simple.Testing.");
			Console.WriteLine();
			Console.WriteLine("Options:");
			optionSet.WriteOptionDescriptions(Console.Out);
		}

		private static TextWriter GetTextWriter(string outputFile)
		{
			return outputFile != null ? new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\" + outputFile) : Console.Out;
		}
	}

	public static class SimpleRunner {
		private static TextWriter _writer;

		public static void RunAllInAssembly(string assemblyName) {
			var assembly = Assembly.LoadFrom(assemblyName);
			RunAllInAssembly(assembly);
		}

		public static void RunAllInAssembly(Assembly assembly) {
			new RootGenerator(assembly).GetSpecifications().OrderBy(x => x.FoundOn.DeclaringType.Name)
				.ForEach(spec => new SpecificationRunner().RunAndDocumentSpec(spec, _writer));
		}
				public static void SetWriter(TextWriter output) {
			_writer = output;
		}

		public static TextWriter GetWriter() {
			return _writer;
		}
	}

	public static class Extensions {
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			foreach (var item in sequence) action(item);
		}
	}


}
