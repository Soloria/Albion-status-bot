namespace ASB.Etc
{
    using System;

    public static class PrimitivesExtensions
    {
        public static ReactiveContext<string> If(this string str, Func<bool, string> cond)
        {
            var ctx = new ReactiveContext<string>();
            ctx.Value = str;
        }
    }

    public class ReactiveContext<T>
    {
        public ReactiveContext(T val) => Value = val;
        public T Value { get; set; }
        public bool Signal { get; set; }
    }
}