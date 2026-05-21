using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace LaParola.Utilities
{
    public static partial class RtfColorTransformer
    {
        private static readonly DependencyProperty OriginalForegroundProperty =
    DependencyProperty.RegisterAttached(
        "OriginalForeground",
        typeof(Brush),
        typeof(RtfColorTransformer),
        new PropertyMetadata(null));

        /* tweakable in these ways
          lum < 40      // what counts as "black"
lum > 215     // what counts as "white"
220           // light replacement
60            // dark replacement
0.85 / 0.15   // softness mix
            */
        /*
        public static string TransformColorTableForDarkMode(string rtf)
        {
            return RegexRTFColorTable().Replace(rtf, match =>
                {
                    double r = int.Parse(match.Groups[1].Value);
                    double g = int.Parse(match.Groups[2].Value);
                    double b = int.Parse(match.Groups[3].Value);

                    // Perceived luminance
                    double lum = 0.299 * r + 0.587 * g + 0.114 * b;

                    int rNew, gNew, bNew;

                    if (lum < 40) // very dark → make light
                    {
                        rNew = gNew = bNew = 220;
                    }
                    else if (lum > 215) // very light → make dark gray
                    {
                        rNew = gNew = bNew = 60;
                    }
                    else
                    {
                        // Invert luminance but preserve color ratio
                        double factor = (255 - lum) / lum;

                        rNew = Clamp(r * factor);
                        gNew = Clamp(g * factor);
                        bNew = Clamp(b * factor);

                        // Optional: soften extremes a bit
                        rNew = Soften(rNew);
                        gNew = Soften(gNew);
                        bNew = Soften(bNew);
                    }

                    return $"\\red{rNew}\\green{gNew}\\blue{bNew}";
                });
        }

        private static int Clamp(double value)
        {
            return (int)Math.Max(0, Math.Min(255, value));
        }

        // Pull values slightly toward mid-range to avoid neon artifacts
        private static int Soften(int value)
        {
            return (int)(value * 0.85 + 255 * 0.15);
        }

        [GeneratedRegex(@"\\red(\d+)\\green(\d+)\\blue(\d+)")]
        private static partial Regex RegexRTFColorTable();
        */

        public static void ApplyThemeToDocument(FlowDocument doc, bool darkMode, Brush appForeground)
        {
            if (doc == null)
                return;

            foreach (Block block in doc.Blocks)
                ApplyThemeToBlock(block, darkMode, appForeground);
        }

        private static void ApplyThemeToBlock(Block block, bool darkMode, Brush appForeground)
        {
            ProcessElement(block, darkMode, appForeground);

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
            ProcessElement(inline, darkMode, appForeground);

            if (inline is Span span)
            {
                foreach (Inline child in span.Inlines)
                    ApplyThemeToInline(child, darkMode, appForeground);
            }
        }

        private static void ProcessElement(TextElement element, bool darkMode, Brush appForeground)
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

        private static bool IsTooDarkForDarkTheme(Color c)
        {
            // Pure black or very dark colors
            //return c.A > 0 && c.R < 40 && c.G < 40 && c.B < 40;
            double luminance =
    (0.2126 * c.R +
     0.7152 * c.G +
     0.0722 * c.B);

            return c.A > 0 && luminance < 80;
        }
    }
}