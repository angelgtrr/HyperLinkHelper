using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace HyperLinkHelper
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "attributeHyperlink")]
    [Name("attributeHyperlink")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AttributeClassifierFormat : ClassificationFormatDefinition
    {
        public AttributeClassifierFormat()
        {
            this.DisplayName = "Attribute Hyperlink";
            this.ForegroundColor = Colors.Blue;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
}
