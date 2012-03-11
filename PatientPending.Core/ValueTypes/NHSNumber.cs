using System;
using System.Text.RegularExpressions;

namespace PatientPending.Core {
    public struct NHSNumber {
        private static readonly Regex NumericRegex = new Regex("\\d+");
        private readonly int[] _number;

        public NHSNumber(int number) {
            _number = number.ToIntArray();
        }

        public NHSNumber(string number) {
            var numberTrimmed = number.StripWhitespace();
            if (NumericRegex.IsMatch(numberTrimmed))
                _number = numberTrimmed.ToIntArray();
            else throw new InvalidCastException("Cannot cast string as NHS Number");
        }

        public bool Validate() {
            if (_number.Length != 10)
                return false;
            int sum = 0;
            int factor = 10;
            for (var i = 0; i < 9; i++) {
                sum += _number[i]*factor;
                factor--;
            }
            var checkDigit = 11 - (sum%11);
            if (checkDigit == 11) checkDigit = 0;
            return checkDigit == _number[9];
        }

        public override string ToString() {
            return string.Format("{0:### ### ####}", _number.ToInt32());
        }
    }
}