using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Functional.Monads
{
    public class Result<T>
    {

        public virtual bool HasValue { get; } = true;

        private T value;

        public Result(T value)
        {
            this.value = value;
        }

        public Result<T> Fail(Action failureAction)
        {
            if (HasValue)
                return this;
            failureAction();
            return this;
        }

        public Result<T> Then(Action<T> successAction)
        {
            if (!HasValue)
                return this;
            successAction(value);
            return this;
        }

    }

    public class Failure<T> : Result<T>
    {

        public Failure() : base(default)
        { }

        public override bool HasValue => false;
    }
}
