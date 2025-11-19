// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using Kusto.Language;
using Kusto.Language.Editor;
using Kusto.Language.Parsing;
using Kusto.Language.Syntax;

namespace OpenTelemetry.Instrumentation.Kusto.Implementation;

internal static class KustoQuerySanitizer
{
    public static string Sanitize(string query)
    {
        // Parse
        var code = KustoCode.Parse(query);
        var root = code.Syntax;

        // Collect nodes that need replacements
        var collector = new ReplacementCollector();
        root.Accept(collector);

        // Build edits
        var edits = new List<TextEdit>();
        foreach (var (node, replacementValue) in collector.Replacements)
        {
            var replacement = TextEdit.Replacement(node.TextStart, node.Width, replacementValue);
            edits.Add(replacement);
        }

        // Apply edits to text
        var text = new EditString(query);
        if (edits.Count == 0)
        {
            return query;
        }

        if (!text.CanApplyAll(edits))
        {
            // TODO: Is there anything better to do here?
            return query;
        }

        var newText = text.ApplyAll(edits);
        return newText;
    }

    private sealed class ReplacementCollector : DefaultSyntaxVisitor
    {
        public readonly List<(SyntaxElement Element, string ReplacementValue)> Replacements = []; // TODO: Clean up to avoid allocs (e.g. use a struct and enum for operation)

        public override void VisitLiteralExpression(LiteralExpression node)
        {
            this.Replacements.Add((node, "?"));
        }

        public override void VisitDynamicExpression(DynamicExpression node)
        {
            this.Replacements.Add((node, "?"));
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpression node)
        {
            this.Replacements.Add((node.Operator, string.Empty));
            base.VisitPrefixUnaryExpression(node);
        }

        protected override void DefaultVisit(SyntaxNode node) => this.VisitChildren(node);

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
