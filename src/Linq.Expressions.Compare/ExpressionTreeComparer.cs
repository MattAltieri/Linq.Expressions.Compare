using Linq.Expressions.Compare.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Linq.Expressions.Compare {
    public class ExpressionTreeComparer : IEqualityComparer<Expression> {
        
        public static ExpressionTreeComparer Instance = new ExpressionTreeComparer();

        public bool Equals(Expression left, Expression right) => new ExpressionComparisonVisitor(left, right).AreEqual;

        public int GetHashCode(Expression expression) => new HashCodeVisitor(expression).HashCode;
    }
}