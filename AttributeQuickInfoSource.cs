using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HyperLinkHelper
{
    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("AttributeHyperlinkQuickInfoProvider")]
    [ContentType("CSharp")]
    internal sealed class AttributeQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new AttributeQuickInfoSource());
        }
    }

    internal sealed class AttributeQuickInfoSource : IAsyncQuickInfoSource
    {
        private static readonly Regex Regex = new Regex(@"TestID\(\d+\)|\[Obsolete\]|\[ApiEndpoint\]", RegexOptions.Compiled);

        public Task<QuickInfoItem?> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(session.TextView.TextSnapshot);
            if (triggerPoint == null || !triggerPoint.HasValue)
            {
                return Task.FromResult<QuickInfoItem?>(null);
            }

            SnapshotPoint point = triggerPoint.Value;
            var line = point.GetContainingLine();
            string text = line.GetText();

            foreach (Match match in Regex.Matches(text))
            {
                var matchSpan = new SnapshotSpan(line.Start + match.Index, match.Length);
                if (point >= matchSpan.Start && point <= matchSpan.End)
                {
                    var trackingSpan = matchSpan.Snapshot.CreateTrackingSpan(matchSpan, SpanTrackingMode.EdgeInclusive);
                    var content = new ClassifiedTextElement(
                        new ClassifiedTextRun(PredefinedClassificationTypeNames.Text, "Go to resource (Alt + Left Click)"));

                    return Task.FromResult<QuickInfoItem?>(new QuickInfoItem(trackingSpan, content));
                }
            }

            return Task.FromResult<QuickInfoItem?>(null);
        }

        public void Dispose()
        {
        }
    }
}
