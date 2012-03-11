using System;
using NUnit.Framework;
using PatientPending.Core;

namespace PatientPending.Specifications
{
    [TestFixture]
    public class PatientAddedEventConverter
    {
        private static readonly int UserId = 1;
        private static readonly Guid PatientId = Guid.NewGuid();

        private object ConvertedEvent;
        [SetUp]
        public void SetUp()
        {
            ConvertedEvent = new EventConverter().Convert(
                new PatientAdded(UserId, PatientId, "Test", "Patient", Title.Mr, new NHSNumber("401 023 2137")));
        }

        [Test]
        public void The_converted_event_will_is_the_latest_version()
        {
            ConvertedEvent.WillBeOfType<PatientAdded_v2>();
        }

        [Test]
        public void The_converted_event_will_contain_the_correct_data()
        {
            ConvertedEvent.As<PatientAdded_v2>().UserId.WillBe(UserId);
            ConvertedEvent.As<PatientAdded_v2>().FirstName.WillBe("Test");
            ConvertedEvent.As<PatientAdded_v2>().Surname.WillBe("Patient");
            ConvertedEvent.As<PatientAdded_v2>().Title.WillBe(Title.Mr);
            ConvertedEvent.As<PatientAdded_v2>().NhsNumber.WillBe(new NHSNumber("401 023 2137"));
            ConvertedEvent.As<PatientAdded_v2>().Gender.WillBe(Gender.Unknown);
            ConvertedEvent.As<PatientAdded_v2>().MiddleName.WillBe(string.Empty);
        }
    }

    public static class TestingExtensions
    {
        public static void WillBeOfType<T>(this object target)
        {
            Assert.IsTrue(target is T);
        }

        public static void WillBe(this object source, object target)
        {
            Assert.AreEqual(source, target);
        }
    }
}