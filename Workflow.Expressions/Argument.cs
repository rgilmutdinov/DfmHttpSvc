using System;

namespace Workflow.Expressions
{
    public sealed class Argument
    {
        private const char LiteralCharacter = '\'';

        public static Argument Null => new Argument(null);

        private static string StripLiteral(string s)
        {
            string literalAsString = LiteralCharacter.ToString();
            if (s.StartsWith(literalAsString) && s.EndsWith(literalAsString))
            {
                string newString = s.Substring(1, s.Length - 2);
                if (newString.Contains(literalAsString))
                {
                    return s;
                }
                return newString;
            }
            return s.Replace("''", "'");
        }

        private readonly object _arg;

        public Argument(int value) : this((object) value)
        {
        }

        public Argument(double value) : this((object) value)
        {
        }

        public Argument(DateTime value) : this((object) value)
        {
        }

        public Argument(string value) : this((object) value)
        {
        }

        public Argument(bool value) : this((object) value)
        {
        }

        private Argument(object obj)
        {
            this._arg = obj;
        }

        public object Object => this._arg;
        public bool IsNull => this._arg == null;

        public bool IsDate
        {
            get
            {
                try
                {
                    if (IsNull)
                    {
                        return false;
                    }

                    ToDate();
                    return true;
                }
                catch (ArgumentCastException e)
                {
                    return false;
                }
            }
        }

        public bool IsDouble
        {
            get
            {
                try
                {
                    if (IsNull)
                    {
                        return false;
                    }

                    ToDouble();
                    return true;
                }
                catch (ArgumentCastException e)
                {
                    return false;
                }
            }
        }

        public bool IsInteger
        {
            get
            {
                try
                {
                    if (IsNull)
                    {
                        return false;
                    }

                    if (this._arg is double d)
                    {
                        return Math.Abs(d % 1) <= (Double.Epsilon * 100);
                    }

                    if (this._arg is float f)
                    {
                        return Math.Abs(f % 1) <= (Single.Epsilon * 100);
                    }

                    ToInteger();
                    return true;
                }
                catch (ArgumentCastException e)
                {
                    return false;
                }
            }
        }

        public bool IsTime
        {
            get
            {
                try
                {
                    if (IsNull)
                    {
                        return false;
                    }

                    if (IsInteger)
                    {
                        return true;
                    }

                    ToTime();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool IsLiteral => !IsNull && !StripLiteral(this._arg.ToString()).Equals(this._arg.ToString());

        public bool IsResolved => IsDate || IsDouble || IsLiteral;

        public override bool Equals(Object o)
        {
            if (o is Argument) {
                try
                {
                    Argument a = (Argument) o;
                    if (IsDouble && a.IsDouble)
                    {
                        return Math.Abs(ToDouble() - a.ToDouble()) <= (Double.Epsilon * 100);
                    }

                    if (IsDate && a.IsDate)
                    {
                        return ToDate() == a.ToDate();
                    }

                    if (IsLiteral || a.IsLiteral)
                    {
                        return ToString().Equals(a.ToString());
                    }

                    return Object.Equals(a.Object);
                }
                catch (Exception exc)
                {
                    return false;
                }
            }
            return false;
        }

        public double ToDouble()
        {
            if (this._arg is double d)
            {
                return d;
            }

            if (this._arg is float f)
            {
                return f;
            }

            if (this._arg is int i)
            {
                return i;
            }

            if (this._arg is string s)
            {
                try
                {
                    return double.Parse(s);
                }
                catch (Exception e)
                {
                    // Allow to pass through to ArgumentCastException
                }
            }

            throw ArgumentCastException.Create("double", this);
        }

        public int ToInteger()
        {
            if (this._arg is int i)
            {
                return i;
            }

            if (this._arg is double d)
            {
                return (int) d;
            }

            if (this._arg is string s)
            {
                try
                {
                    return int.Parse(s);
                }
                catch (Exception e)
                {
                    // Allow to pass through to ArgumentCastException
                }
            }

            throw ArgumentCastException.Create("integer", this);
        }

        public int ToTime()
        {
            if (IsInteger)
            {
                return ToInteger();
            }

            if (this._arg is string s) {
                return (int) DateUtils.MultiParseTime(s);
            }

            throw ArgumentCastException.Create("time", this);
        }

        public bool ToBoolean()
        {
            if (this._arg is bool b)
            {
                return b;
            }

            if (this._arg is string s)
            {
                if (s.Equals("true", StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }

                if (s.Equals("false", StringComparison.OrdinalIgnoreCase)) {
                    return false;
                }
            }

            throw ArgumentCastException.Create("boolean", this);
        }

        public DateTime? ToDate()
        {
            if (this._arg is DateTime dateTime) {
                return dateTime;
            }

            if (this._arg is string s)
            {
                DateTime? date = DateUtils.MultiParseDate(s);
                if (date != null)
                {
                    return date;
                }
            }

            throw ArgumentCastException.Create("date", this);
        }

        public override string ToString()
        {
            if (this._arg == null)
            {
                return "null";
            }

            return StripLiteral(this._arg.ToString());
        }

        private bool Equals(Argument other)
        {
            return Equals(this._arg, other._arg);
        }

        public override int GetHashCode()
        {
            return this._arg == null ? 0 : this._arg.GetHashCode();
        }
    }
}
