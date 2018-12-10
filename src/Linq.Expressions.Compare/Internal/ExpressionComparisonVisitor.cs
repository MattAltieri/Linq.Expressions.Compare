using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq.Expressions.Compare.Internal {
    internal class ExpressionComparisonVisitor : ExpressionVisitor {
        
        private Queue<Expression> comparisonCandidates;
        private Expression currentNode;

        public ExpressionComparisonVisitor(Expression leftTree, Expression rightTree) {

            comparisonCandidates = new Queue<Expression>(new ExpressionTreeEnumerator(rightTree));

            AreEqual = true;

            Visit(leftTree);

            if (comparisonCandidates.Count > 0)
                AreEqual = false;
        }

        public bool AreEqual { get; private set; }

        #region Visit Overrides
        public override Expression Visit(Expression node) {

            if (node == null) return node;
            if (!AreEqual) return node;

            currentNode = comparisonCandidates.Count == 0 ? null : comparisonCandidates.Peek();
            if (currentNode == null ||
                // currentNode.NodeType != node.NodeType ||
                // currentNode.Type != node.Type) {
                !CompareExpressionTypes(currentNode, node)) {
                
                AreEqual = false;
                return node;
            }

            comparisonCandidates.Dequeue();

            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) {

            BinaryExpression candidate = (BinaryExpression)currentNode;

            // if (node.Method != candidate.Method ||
            //     node.IsLifted != candidate.IsLifted ||
            //     node.IsLiftedToNull != candidate.IsLiftedToNull) {
            if (!Compare(node.Method, candidate.Method) ||
                !Compare(node.IsLifted, candidate.IsLifted) ||
                !Compare(node.IsLiftedToNull, candidate.IsLiftedToNull)) {
                
                AreEqual = false;
                return node;
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node) {

            // if (node.Value != ((ConstantExpression)currentNode).Value) {
            if (!Compare(node.Value, ((ConstantExpression)currentNode).Value)) {
                AreEqual = false;
                return node;
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node) {

            // if (node.Member != ((MemberExpression)currentNode).Member) {
            if (!Compare(node.Member, ((MemberExpression)currentNode).Member)) {
                AreEqual = false;
                return node;
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {

            // if (node.Method != ((MethodCallExpression)currentNode).Method) {
            if (!Compare(node.Method, ((MethodCallExpression)currentNode).Method)) {
                AreEqual = false;
                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitParameter(ParameterExpression node) {

            // if (node.Name != ((ParameterExpression)currentNode).Name) {
            if (!Compare(node.Name, ((ParameterExpression)currentNode).Name)) {
                AreEqual = false;
                return node;
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node) {

            // if (node.TypeOperand != ((TypeBinaryExpression)currentNode).TypeOperand) {
            if (!Compare(node.TypeOperand, ((TypeBinaryExpression)currentNode).TypeOperand)) {
                AreEqual = false;
                return node;
            }

            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node) {

            UnaryExpression candidate = (UnaryExpression)currentNode;

            // if (node.Method != candidate.Method ||
            //     node.IsLifted != candidate.IsLifted ||
            //     node.IsLiftedToNull != candidate.IsLiftedToNull) {
            if (!Compare(node.Method, candidate.Method) ||
                !Compare(node.IsLifted, candidate.IsLifted) ||
                !Compare(node.IsLiftedToNull, candidate.IsLiftedToNull)) {
                
                AreEqual = false;
                return node;
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitNew(NewExpression node) {

            NewExpression candidate = (NewExpression)currentNode;

            // if (node.Constructor != candidate.Constructor) {
            if (!Compare(node.Constructor, candidate.Constructor)) {
                AreEqual = false;
                return node;
            }

            ReadOnlyCollection<MemberInfo> nodeMembers = node.Members;
            ReadOnlyCollection<MemberInfo> candidateMembers = candidate.Members;

            if ((nodeMembers?.Count ?? 0) != (candidateMembers?.Count ?? 0)) {
                AreEqual = false;
                return node;
            }

            for (int i = 0; i < (nodeMembers?.Count ?? 0); i++) {
                // if (nodeMembers[i] != candidateMembers[i]) {
                if (!Compare(nodeMembers[i], candidateMembers[i])) {
                    AreEqual = false;
                    return node;
                }
            }

            return base.VisitNew(node);
        }
        #endregion

        #region Helper Methods
        private bool Compare<T>(T left, T right)
            => EqualityComparer<T>.Default.Equals(left, right);

        private bool CompareExpressionTypes(Expression left, Expression right) {
            if (!Compare(left.NodeType, right.NodeType)) return false;
            if (!Compare(left.Type, right.Type)) return false;
            return true;
        }
        #endregion
    }
}