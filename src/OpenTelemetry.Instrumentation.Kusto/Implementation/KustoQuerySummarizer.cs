// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Text;
using Kusto.Language;
using Kusto.Language.Symbols;
using Kusto.Language.Syntax;

namespace OpenTelemetry.Instrumentation.Kusto.Implementation;

internal static class KustoQuerySummarizer
{
    public static string Summarize(string query)
    {
        var code = KustoCode.ParseAndAnalyze(query);

        var walker = new SummaryVisitor();
        code.Syntax.Accept(walker);

        var sb = new StringBuilder();
        foreach (var segment in walker.Builder)
        {
            sb.Append(segment).Append(' ');
        }

        sb.TrimEnd();
        return sb.ToString(0, Math.Min(255, sb.Length));
    }

    private sealed class SummaryVisitor : DefaultSyntaxVisitor
    {
        public readonly List<string> Builder = new();

        public override void VisitPipeExpression(PipeExpression node)
        {
            node.Expression.Accept(this);
            this.Builder.Add("|");
            node.Operator.Accept(this);
        }

        public override void VisitNameReference(NameReference node)
        {
            if (node.ResultType is TableSymbol ts)
            {
                this.Builder.Add(ts.Name);
            }
            else if (node.ResultType is ErrorSymbol)
            {
                this.Builder.Add(node.ToString(IncludeTrivia.SingleLine));
            }
        }

        public override void VisitFunctionCallExpression(FunctionCallExpression node)
        {
            if (node.Name.SimpleName == "materialized_view")
            {
                this.Builder.Add(node.ToString(IncludeTrivia.SingleLine));
            }
        }

        public override void VisitDataTableExpression(DataTableExpression node) => this.Builder.Add(node.DataTableKeyword.Text);

        public override void VisitCustomCommand(CustomCommand node)
        {
            this.Builder.Add(node.DotToken + node.Custom.GetFirstToken().ToString(IncludeTrivia.SingleLine));
        }

        protected override void DefaultVisit(SyntaxNode node)
        {
            if (node is QueryOperator qo)
            {
                this.VisitQueryOperator(qo);
            }
            else
            {
                this.VisitChildren(node);
            }
        }

        private void VisitQueryOperator(QueryOperator node)
        {
            if (node is BadQueryOperator)
            {
                return;
            }

            this.Builder.Add(node.GetFirstToken().ToString(IncludeTrivia.SingleLine));

            this.VisitChildren(node);
        }

        private void VisitChildren(SyntaxNode node)
        {
            if (node != null)
            {
                for (int i = 0; i < node.ChildCount; i++)
                {
                    if (node.GetChild(i) is SyntaxNode child)
                    {
                        child.Accept(this);
                    }
                }
            }
        }
    }
}
