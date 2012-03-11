﻿using System.Collections.Generic;
using System.Linq;

namespace Simple.Testing.PAssert.Infrastructure.Nodes
{
    internal class MethodCallNode : MemberAccessNode
    {
        internal MethodCallNode()
        {
            Parameters = new List<Node>();
        }

        [NotNull]
        public List<Node> Parameters { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            base.Walk(walker, depth);
            walker("(");
            foreach (var parameter in Parameters.Take(1))
            {
                parameter.Walk(walker, depth);
            }
            foreach (var parameter in Parameters.Skip(1))
            {
                walker(", ");
                parameter.Walk(walker, depth);
            }
            walker(")");
        }
    }
}