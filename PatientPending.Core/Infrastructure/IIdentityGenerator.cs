using System;

namespace PatientPending.Core
{
    public interface IIdentityGenerator
    {
        long GetId();
    }

    public class IdentityGenerator : IIdentityGenerator
    {
        private long _seed;
        public IdentityGenerator(long seed)
        {
            _seed = seed;
        }

        [STAThread]
        public long GetId()
        {
            _seed++;
            return _seed;
        }
    }
}