using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Linq;

namespace Simple.Testing
{
    public delegate void WhenAction<in T>(T item);
    public delegate bool Expectation<in T>(T obj);
    public class FailingSpecification<TSut, TException> : TypedSpecification<TException> where TException : Exception
    {
        public Action Before;
        public Func<TSut> On;
        public WhenAction<TSut> When;
        public List<Expression<Func<TException, bool>>> Expect = new List<Expression<Func<TException, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen()
        {
            return (Func<TSut, TException>)(x =>
                {
                    try
                    {
                        When(x);
                    }
                    catch (TException ex)
                    {
                        return ex;
                    }
                    return null;
                });
        }

        IEnumerable<Assertion<TException>> TypedSpecification<TException>.GetAssertions()
        {
            return Expect.Select(e => new PAssertion<TException>(e));
        }


        public IEnumerable<Expression<Func<TException, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
        public void Document(RunResult result, TextWriter output)
        {
            Console.WriteLine("Do something");
        }
    }

    public sealed class PartialApplicationVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _paramExprToReplace;
        private readonly ConstantExpression _valuetoApply;

        private PartialApplicationVisitor(ParameterExpression paramExprToReplace, ConstantExpression valueToApply)
        {
            _paramExprToReplace = paramExprToReplace;
            _valuetoApply = valueToApply;
        }

        public static Expression<Func<bool>> Apply<T>(Expression<Func<T, bool>> expr, object value)
        {
            var paramExprToReplace = expr.Parameters[0];
            var valueToApply = Expression.Constant(value, value.GetType());
            var visitor = new PartialApplicationVisitor(paramExprToReplace, valueToApply);

            var oldBody = expr.Body;
            var newBody = visitor.Visit(oldBody);
            return Expression.Lambda<Func<bool>>(newBody);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return (node == _paramExprToReplace) ? _valuetoApply : base.VisitParameter(node);
        }
    }


    public sealed class PAssertion<T> : Assertion<T>
    {
        readonly Expression<Func<T, bool>> _expression;
        public PAssertion(Expression<Func<T, bool>> expression)
        {
            _expression = expression;
        }

        public IEnumerable<ExpectationResult> Assert(object fromWhen)
        {
            var partiallyApplied = PartialApplicationVisitor.Apply(_expression, fromWhen);
            ExpectationResult result;
            try
            {
                PAssert.PAssert.IsTrue(partiallyApplied);
                result = (new ExpectationResult { Passed = true, Text = PAssert.PAssert.CreateSimpleFormatFor   (partiallyApplied), OriginalExpression = _expression });
            }
            catch (Exception ex)
            {
                
                result =(new ExpectationResult { Passed = false, Text = PAssert.PAssert.CreateSimpleFormatFor(partiallyApplied), OriginalExpression = _expression, Exception = ex });
            }
            yield return result;
        }
    }
}