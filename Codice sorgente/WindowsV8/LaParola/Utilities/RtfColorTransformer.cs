using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LaParola.Utilities
{
    public static partial class RtfColorTransformer
    {
        private static bool _apice;

        private static readonly DependencyProperty OriginalForegroundProperty =
    DependencyProperty.RegisterAttached(
        "OriginalForeground",
        typeof(Brush),
        typeof(RtfColorTransformer),
        new PropertyMetadata(null));

        public static Brush GetDarkThemedBrush(Brush originalBrush, Brush appForeground)
        {
            if (originalBrush is SolidColorBrush scb && IsTooDarkForDarkTheme(scb.Color))
            {
                return appForeground;
            }
            return originalBrush;
        }

        public static void ApplyThemeToDocument(FlowDocument doc, bool darkMode, Brush appForeground, bool trasformaApice = false)
        {
            if (doc == null)
                return;

            _apice = trasformaApice;

            foreach (Block block in doc.Blocks)
                ApplyThemeToBlock(block, darkMode, appForeground);
        }

        private static void ApplyThemeToBlock(Block block, bool darkMode, Brush appForeground)
        {
            ApplyThemeToElement(block, darkMode, appForeground);

            switch (block)
            {
                case Paragraph p:
                    foreach (Inline inline in p.Inlines)
                        ApplyThemeToInline(inline, darkMode, appForeground);
                    break;

                case Section s:
                    foreach (Block child in s.Blocks)
                        ApplyThemeToBlock(child, darkMode, appForeground);
                    break;

                case List list:
                    foreach (ListItem item in list.ListItems)
                        foreach (Block child in item.Blocks)
                            ApplyThemeToBlock(child, darkMode, appForeground);
                    break;

                case Table table:
                    foreach (TableRowGroup rg in table.RowGroups)
                        foreach (TableRow row in rg.Rows)
                            foreach (TableCell cell in row.Cells)
                                foreach (Block child in cell.Blocks)
                                    ApplyThemeToBlock(child, darkMode, appForeground);
                    break;
            }
        }

        private static void ApplyThemeToInline(Inline inline, bool darkMode, Brush appForeground)
        {
            ApplyThemeToElement(inline, darkMode, appForeground);
            if (_apice)
            {
                AdjustInlineTypo(inline);
            }

            if (inline is Span span)
            {
                foreach (Inline child in span.Inlines)
                    ApplyThemeToInline(child, darkMode, appForeground);
            }
        }

        private static void ApplyThemeToElement(TextElement element, bool darkMode, Brush appForeground)
        {
            object local = element.ReadLocalValue(TextElement.ForegroundProperty);
            if (darkMode)
            {
                if (local is SolidColorBrush scb && IsTooDarkForDarkTheme(scb.Color))
                {
                    // Save original brush once
                    if (element.GetValue(OriginalForegroundProperty) == null)
                    {
                        element.SetValue(OriginalForegroundProperty, scb);
                    }
                    element.Foreground = appForeground;
                }
            }
            else
            {
                // Restore original brush if we changed it
                if (element.GetValue(OriginalForegroundProperty) is Brush original)
                {
                    element.Foreground = original;
                    element.ClearValue(OriginalForegroundProperty);
                }
            }
        }

        private static void AdjustInlineTypo(Inline inline)
        {
            // Typography.Variants is an attached property, so we read it via GetValue
            var variant = (FontVariants)inline.GetValue(Typography.VariantsProperty);

            if (variant == FontVariants.Superscript)
            {
                // Clear OpenType variant so standard alphabetic characters render
                inline.ClearValue(Typography.VariantsProperty);

                // Apply inline-specific baseline properties safely
                inline.BaselineAlignment = BaselineAlignment.Superscript;
                inline.FontSize *= 0.7;
            }
            else if (variant == FontVariants.Subscript)
            {
                inline.ClearValue(Typography.VariantsProperty);
                inline.BaselineAlignment = BaselineAlignment.Subscript;
                inline.FontSize *= 0.7;
            }
        }

        private static bool IsTooDarkForDarkTheme(Color c)
        {
            // Pure black or very dark colors
            double luminance = (0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B);

            return c.A > 0 && luminance < 18;
        }
    }
}