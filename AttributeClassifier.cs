using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;

namespace HyperLinkHelper
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("CSharp")] // only run in C# files
    internal class AttributeClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new AttributeClassifier(ClassificationRegistry));
        }
    }

    internal class AttributeClassifier : IClassifier
    {
        private readonly IClassificationType classificationType;
        private readonly Regex regex = new Regex(@"

\[\w+.*?\]

", RegexOptions.Compiled);

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public AttributeClassifier(IClassificationTypeRegistryService registry)
        {
            classificationType = registry.GetClassificationType("attributeHyperlink");
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();
            string text = span.GetText();

            foreach (Match match in regex.Matches(text))
            {
                var matchSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                result.Add(new ClassificationSpan(matchSpan, classificationType));
            }

            return result;
        }
    }
}
