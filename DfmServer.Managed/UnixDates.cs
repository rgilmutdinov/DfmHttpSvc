using System;

namespace DfmServer.Managed
{
    public static class UnixDates
    {
        private static readonly DateTime Base = new DateTime(1970, 1, 1);

        public static readonly DateTime Min = Base.Add(TimeSpan.FromSeconds(Int32.MinValue));
        public static readonly DateTime Max = Base.Add(TimeSpan.FromSeconds(Int32.MaxValue));
    }
}
