using System;
using System.Text.RegularExpressions;

namespace PatientPending.Core {
    public struct NHSNumber {
        private readonly int[] _digits;
        private readonly long _number;

        public NHSNumber(long number) {
            _number = number;
            _digits = number.ToIntArray();
        }

        public NHSNumber(string number) {
            var numberTrimmed = number.StripWhitespace();
            if (Int64.TryParse(numberTrimmed, out _number))
                _digits = numberTrimmed.ToIntArray();
            else 
                throw new InvalidCastException("Cannot cast string as NHS Number");
        }

        public bool Validate() {
            if (_digits.Length != 10)
                return false;
            int sum = 0;
            int factor = 10;
            for (var i = 0; i < 9; i++) {
                sum += _digits[i]*factor;
                factor--;
            }
            var checkDigit = 11 - (sum%11);
            if (checkDigit == 11) checkDigit = 0;
            return checkDigit == _digits[9];
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return _number.Equals(((NHSNumber)obj)._number);
        }

        public static bool operator ==(NHSNumber c1, NHSNumber c2)
        {
            return c1._number == c2._number;
        }

        public static bool operator !=(NHSNumber c1, NHSNumber c2)
        {
            return c1._number != c2._number;
        }

        public override int GetHashCode()
        {
            return _number.GetHashCode();
        } 

        public override string ToString() {
            return string.Format("{0:### ### ####}", _number);
        }
    }
}