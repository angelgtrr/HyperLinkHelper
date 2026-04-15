using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;

namespace HyperLinkHelper
{
    [Export(typeof(IMouseProcessorProvider))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [Name("AttributeHyperlinkMouseProcessor")]
    internal sealed class AttributeMouseProcessorProvider : IMouseProcessorProvider
    {
        public IMouseProcessor GetAssociatedProcessor(IWpfTextView textView)
        {
            return new AttributeMouseProcessor(textView);
        }
    }

    internal class AttributeMouseProcessor : MouseProcessorBase
    {
        private readonly IWpfTextView view;

        // Mapping table: attribute → URL
        private readonly Dictionary<string, string> attributeLinks = new Dictionary<string, string>
        {
            { "[Obsolete]", "https://learn.microsoft.com/dotnet/csharp/language-reference/attributes/obsolete" },
            { "[ApiEndpoint]", "https://docs.myapi.com" },
            { "[MyAttribute]", "https://your-url-here.com" }
        };

        public AttributeMouseProcessor(IWpfTextView view)
        {
            this.view = view;
        }

        public override void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Only trigger if Alt is pressed
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                var caretPos = view.Caret.Position.BufferPosition;
                var line = caretPos.GetContainingLine();
                string text = line.GetText();

                foreach (var kvp in attributeLinks)
                {
                    if (text.Contains(kvp.Key))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = kvp.Value,
                            UseShellExecute = true
                        });
                        break;
                    }
                }
            }

            base.PreprocessMouseLeftButtonUp(e);
        }
    }
}
