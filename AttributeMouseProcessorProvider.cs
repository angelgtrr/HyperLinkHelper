using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

                // Regex to capture TestID(number)
                var match = Regex.Match(text, @"TestID\((\d+)\)");
                if (match.Success)
                {
                    string id = match.Groups[1].Value; // e.g. "100"
                    string url = $"https://sdiqa.testmo.net/repositories/1?group_id=28&case_id={id}";

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                    return;
                }

                // Handle other fixed attributes
                if (text.Contains("[Obsolete]"))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://learn.microsoft.com/dotnet/csharp/language-reference/attributes/obsolete",
                        UseShellExecute = true
                    });
                }
                else if (text.Contains("[ApiEndpoint]"))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://docs.myapi.com",
                        UseShellExecute = true
                    });
                }
            }

            base.PreprocessMouseLeftButtonUp(e);
        }
    }
}
