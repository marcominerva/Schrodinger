using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace System
{
    public class Schrodinger : IComparable, IEquatable<Schrodinger>
    {
        private const int TYPES_COUNT = 19;
        private const string MIN_VALUE_PROPERTY = "MinValue";
        private const string MAX_VALUE_PROPERTY = "MaxValue";
        private const int MAX_STRING_LENGTH = 1048576;

        private Random rnd;
        private Type type;
        private bool hasValue;
        private dynamic value;

        private static readonly object syncObject = new object();

        public new Type GetType()
        {
            // Until you check it, it could be any type.
            if (type == null)
            {
                var typeNumber = rnd.Next(0, TYPES_COUNT);
                type = GetActualType(typeNumber);
            }

            return type;
        }

        public dynamic Value => GetValue();

        public override string ToString()
        {
            return GetValue().ToString();
        }

        private dynamic GetValue()
        {
            lock (syncObject)
            {
                if (!hasValue)
                {
                    // Waits 10 milliseconds to ensure difference in random seed.
                    new ManualResetEvent(false).WaitOne(10);
                    rnd = new Random(unchecked((int)DateTime.Now.Ticks));

                    type = this.GetType();
                    if (type == typeof(bool))
                    {
                        value = (rnd.Next(int.MinValue, int.MaxValue) % 2 == 0);
                    }
                    else if (type == typeof(string))
                    {
                        // Generates a random string.
                        value = CreateRandomString();
                    }
                    else if (type == typeof(char))
                    {
                        var str = CreateRandomString();
                        var index = rnd.Next(str.Length);
                        value = str[index];
                    }
                    else if (type == typeof(Guid))
                    {
                        value = Guid.NewGuid();
                    }
                    else if (type == typeof(object))
                    {
                        value = (rnd.Next(int.MinValue, int.MaxValue) % 2 == 0 ? new object() : null);
                    }
                    else
                    {
                        GetBoundaries(type, out var minValue, out var maxValue);

                        var temp = rnd.NextDouble() * (maxValue - minValue) + minValue;

                        // Handles dates and times separately.
                        if (type == typeof(DateTime))
                        {
                            value = new DateTime((long)temp);
                        }
                        else if (type == typeof(DateTimeOffset))
                        {
                            value = new DateTimeOffset((long)temp, TimeSpan.FromHours(rnd.Next(-14, 15)));
                        }
                        else if (type == typeof(TimeSpan))
                        {
                            value = new TimeSpan((long)temp);
                        }
                        else
                        {
                            value = Convert.ChangeType(temp, type, CultureInfo.InvariantCulture);
                        }
                    }

                    hasValue = true;
                }
            }

            return value;
        }

        private Type GetActualType(int typeNumber)
        {
            switch (typeNumber)
            {
                case 0:
                    return typeof(bool);

                case 1:
                    return typeof(byte);

                case 2:
                    return typeof(char);

                case 3:
                    return typeof(decimal);

                case 4:
                    return typeof(double);

                case 5:
                    return typeof(float);

                case 6:
                    return typeof(int);

                case 7:
                    return typeof(long);

                case 8:
                    return typeof(sbyte);

                case 9:
                    return typeof(short);

                case 10:
                    return typeof(uint);

                case 11:
                    return typeof(ulong);

                case 12:
                    return typeof(ushort);

                case 13:
                    return typeof(string);

                case 14:
                    return typeof(DateTime);

                case 15:
                    return typeof(DateTimeOffset);

                case 16:
                    return typeof(TimeSpan);

                case 17:
                    return typeof(Guid);

                case 18:
                default:
                    return typeof(object);
            }
        }

        private void GetBoundaries(Type type, out double minValue, out double maxValue)
        {
            // Checks whether the type has boundaries (i.e., MinValue and MaxValue properties) and then extracts their values.
            minValue = maxValue = 0D;

            try
            {
                if (type == typeof(DateTime))
                {
                    minValue = DateTime.MinValue.Ticks;
                    maxValue = DateTime.MaxValue.Ticks;
                }
                else if (type == typeof(DateTimeOffset))
                {
                    minValue = DateTimeOffset.MinValue.Ticks;
                    maxValue = DateTimeOffset.MaxValue.Ticks;
                }
                else if (type == typeof(TimeSpan))
                {
                    minValue = TimeSpan.MinValue.Ticks;
                    maxValue = TimeSpan.MaxValue.Ticks;
                }
                else
                {
                    var minField = type.GetField(MIN_VALUE_PROPERTY);
                    var maxField = type.GetField(MAX_VALUE_PROPERTY);

                    if (minField != null && maxField != null)
                    {
                        minValue = (double)Convert.ChangeType(minField.GetValue(null), typeof(double), CultureInfo.InvariantCulture);
                        maxValue = (double)Convert.ChangeType(maxField.GetValue(null), typeof(double), CultureInfo.InvariantCulture);
                    }
                }
            }
            catch
            {
            }
        }

        private string CreateRandomString()
        {
            var length = rnd.Next(0, MAX_STRING_LENGTH);

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new string(Enumerable.Repeat(chars, length)
                          .Select(s => s[rnd.Next(s.Length)])
                          .ToArray());

            return result;
        }

        public int CompareTo(object obj)
        {
            // If the type hasn't a value, the comparison is random.
            if (!hasValue)
            {
                var retVal = rnd.Next(-1, 2);
                return retVal;
            }

            if (obj is Schrodinger dest)
            {

                try
                {
                    if (value < dest.value)
                    {
                        return -1;
                    }
                }
                catch
                {
                    return -1;
                }

                try
                {
                    if (value > dest.value)
                    {
                        return 1;
                    }
                }
                catch
                {
                    return 1;
                }

                return 0;
            }

            return -1;
        }

        public bool Equals(Schrodinger other)
        {
            return CompareTo(other) == 0;
        }
    }
}