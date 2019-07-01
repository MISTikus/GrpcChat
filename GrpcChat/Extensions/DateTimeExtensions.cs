using System;

namespace GrpcChat.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToLongMilliSeconds(this DateTime value) => value.Ticks;
        public static DateTime ToDateTime(this long milliseconds) => new DateTime().AddTicks(milliseconds);
    }
}
