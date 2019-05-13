namespace ASB.Etc
{
    using System;

    public static class PrimitivesExtensions
    {
        public static ReactiveContext<string> If(this string str, Func<string, bool> cond) 
            => new ReactiveContext<string>().WithValue(str).WithSignal(cond(str));

        public static ReactiveContext<string> Then(this ReactiveContext<string> ctx, Action<string> actor)
            => ctx.Signal ? ctx.Pipe(() => actor(ctx.Value)) : ctx;
        public static ReactiveContext<string> Else(this ReactiveContext<string> ctx, Action<string> actor) 
            => !ctx.Signal ? ctx.Pipe(()=> actor(ctx.Value)) : ctx;
    }

    public class ReactiveContext<T>
    {
        public T Value { get; set; }
        public bool Signal { get; set; }

        public ReactiveContext<T> WithSignal(bool result)
        {
            this.Signal = result;
            return this;
        }
        public ReactiveContext<T> WithValue(T result)
        {
            this.Value = result;
            return this;
        }

        public ReactiveContext<T> Pipe(Action actor)
        {
            actor();
            return this;
        }
        public static implicit operator bool(ReactiveContext<T> t) => t.Signal;
    }
}