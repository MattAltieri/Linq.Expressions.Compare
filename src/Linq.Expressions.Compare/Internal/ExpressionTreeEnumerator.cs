using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Linq.Expressions.Compare.Internal {
    internal class ExpressionTreeEnumerator : ExpressionVisitor, IEnumerable<Expression> {
        
        private List<Expression> expressions = new List<Expression>();

        public ExpressionTreeEnumerator() { }

        public ExpressionTreeEnumerator(Expression expression) => Visit(expression);

        public IEnumerator<Expression> GetEnumerator() => expressions.GetEnumerator();

        public override Expression Visit(Expression node) {

            if (node == null) return node;
            expressions.Add(node);
            return base.Visit(node);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}