using System;
using System.Collections.Generic;

namespace PatientPending.Core
{
    public class EventConverter
    {
        private readonly Dictionary<Type, Func<object, object>> _converters;

        public EventConverter()
        {
            _converters = new Dictionary<Type, Func<object, object>>();
            RegisterEventConverters();
        }

        private void RegisterEventConverters()
        {
            _converters.Add(typeof(PatientAdded), x => new PatientAddedEventConverter().Convert((PatientAdded)x));
        }

        public object Convert(object sourceEvent)
        {
            Func<object, object> converter;
            return _converters.TryGetValue(sourceEvent.GetType(), out converter)
                       ? Convert(converter(sourceEvent))
                       : sourceEvent;
        }
    }

    public interface IEventConverter<TSourceEvent, TTargetEvent>
        where TSourceEvent : Event
        where TTargetEvent : Event
    {
        TTargetEvent Convert(TSourceEvent sourceEvent);
    }

    public class PatientAddedEventConverter : IEventConverter<PatientAdded, PatientAdded_v2>
    {
        public PatientAdded_v2 Convert(PatientAdded sourceEvent) {
            var theEvent = new PatientAdded_v2(sourceEvent.UserId, sourceEvent.PatientId, sourceEvent.FirstName,
                                               sourceEvent.Surname, string.Empty, sourceEvent.Title, Gender.Other,
                                               DateTime.MinValue, sourceEvent.NhsNumber);
            theEvent.Version = sourceEvent.Version;
            return theEvent;
        }
    }
}