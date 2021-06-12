﻿using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Core.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Nikse.SubtitleEdit.Logic
{
    public static class AssaIntellisense
    {
        public class IntellisenseItem
        {
            public string Value { get; set; }
            public string Hint { get; set; }
            public bool AllowInTransformations { get; set; }
            public string TypedWord { get; set; }
            public string ActiveTagAtCursor { get; set; }
            public Font Font { get; set; }

            public IntellisenseItem(string value, string hint, bool allowInTransformations)
            {
                Value = value;
                Hint = hint;
                AllowInTransformations = allowInTransformations;
            }

            public override string ToString()
            {
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    var textSize = g.MeasureString(Value, Font);
                    var spaceSize = g.MeasureString(" ", Font);
                    var v = Value;
                    if (textSize.Width < 200)
                    {
                        var missing = 200 - textSize.Width;
                        var rest = (int)(missing / spaceSize.Width);
                        v += "".PadRight(rest, ' ');
                    }

                    return v + "\t " + Hint;
                }
            }
        }

        private static readonly List<IntellisenseItem> Keywords = new List<IntellisenseItem>
        {
            new IntellisenseItem("{\\i1}",  "Italic on", false),
            new IntellisenseItem("{\\i0}",  "Italic off", false),

            new IntellisenseItem("{\\b1}",  "Bold on", false),
            new IntellisenseItem("{\\b0}",  "Bold off", false),

            new IntellisenseItem("{\\u1}",  "Underline on", false),
            new IntellisenseItem("{\\u0}",  "Underline off", false),

            new IntellisenseItem("{\\s1}",  "Strikeout on", false),
            new IntellisenseItem("{\\s0}",  "Strikeout off", false),

            new IntellisenseItem("{\\mov(x1,y1,x2,y2,start,end)}",  "Move", false),
            new IntellisenseItem("{\\pos(x,y)}",  "Set position", false),

            new IntellisenseItem("{\\c&Hbbggrr&}",  "Color", true),
            new IntellisenseItem("{\\2c&Hbbggrr&}",  "Color for outline", true),
            new IntellisenseItem("{\\3c&Hbbggrr&}",  "Color for opaque box", true),
            new IntellisenseItem("{\\4c&Hbbggrr&}",  "Color for shadow", true),

            new IntellisenseItem("{\\fade(a1,a2,a3,t1,t2,t3,t4)}",  "Fade advanced", false),
            new IntellisenseItem("{\\fad(fadein time,fadeout time>}",  "Fade", false),

            new IntellisenseItem("{\\fn<font_name>}",  "Font name", false),

            new IntellisenseItem("{\\fs<font_size>}",  "Font size", true),

            new IntellisenseItem("{\\an7}",  "Align top left", false),
            new IntellisenseItem("{\\an8}",  "Align top center", false),
            new IntellisenseItem("{\\an9}",  "Align top right", false),
            new IntellisenseItem("{\\an4}",  "Align center left", false),
            new IntellisenseItem("{\\an5}",  "Align center middle", false),
            new IntellisenseItem("{\\an6}",  "Align center right", false),
            new IntellisenseItem("{\\an1}",  "Align bottom left", false),
            new IntellisenseItem("{\\an2}",  "Align bottom middle", false),
            new IntellisenseItem("{\\an3}",  "Align bottom right", false),

            new IntellisenseItem("{\\bord<width>}",  "Border", true),
            new IntellisenseItem("{\\xbord<x>}",  "Border width", true),
            new IntellisenseItem("{\\ybord<y>}",  "Border height", true),

            new IntellisenseItem("{\\shad<depth>}",  "Shadow", true),
            new IntellisenseItem("{\\xshad<x>}",  "Shadow width", true),
            new IntellisenseItem("{\\yshad<y>}",  "Shadow height", true),

            new IntellisenseItem("{\\be}",  "Blur edges", true),
            new IntellisenseItem("{\\be0}",  "Blur edges off", true),
            new IntellisenseItem("{\\be1}",  "Blur edges on", true),

            new IntellisenseItem("{\\fscx<percent>}",  "Scale X percentage", true),
            new IntellisenseItem("{\\fscy<percent>}",  "Scale Y percentage", true),

            new IntellisenseItem("{\\fsp<pixels>}",  "Spacing between letters in pixels", true),

            new IntellisenseItem("{\\fr<degree>}",  "Angle (z axis for text rotation)", true),
            new IntellisenseItem("{\\frx<degree>}",  "Angle (x axis for text rotation)", true),
            new IntellisenseItem("{\\fry<degree>}",  "Angle (y axis for text rotation)", true),
            new IntellisenseItem("{\\frz<degree>}",  "Angle (z axis for text rotation)", true),

            new IntellisenseItem("{\\org<x,y>}",  "Set origin point for rotation", false),

            new IntellisenseItem("{\\fax<degree>}",  "Shearing transformation (x axis)", true),
            new IntellisenseItem("{\\fay<degree>}",  "Shearing transformation (y axis)", true),

            new IntellisenseItem("{\\fe<charset>}",  "Encoding", false),

            new IntellisenseItem("{\\alpha&Haa&}",  "Alpha (00=fully visible, ff=transparent)", true),
            new IntellisenseItem("{\\a2&Haa&}",  "Alpha for outline (00=fully visible, ff=transparent)", true),
            new IntellisenseItem("{\\a3&Haa&}",  "Alpha for opaque box (00=fully visible, ff=transparent)", true),
            new IntellisenseItem("{\\a4&Haa&}",  "Alpha for shadow (00=fully visible, ff=transparent)", true),

            new IntellisenseItem("{\\k<duration>}",  "Karaoke, delay in 100th of a second (10ms)", false),
            new IntellisenseItem("{\\K<duration>}",  "Karaoke right to left, delay in 100th of a second (10ms)", false),

            new IntellisenseItem("{\\clip(x1,y1,x2,y2)}",  "Clips (hides) any drawing outside the rectangle defined by the parameters.", true),
            new IntellisenseItem("{\\iclip(x1,y1,x2,y2)}",  "Clips (hides) any drawing inside the rectangle defined by the parameters.", true),

            new IntellisenseItem("{\\r}",  "Reset inline styles", false),

            new IntellisenseItem("{\\t(<style modifiers>)}",  "Animated transform", false),
            new IntellisenseItem("{\\t(<accel,style modifiers>)}",  "Animated transform", false),
            new IntellisenseItem("{\\t(<t1,t2,style modifiers>)}",  "Animated transform", false),
            new IntellisenseItem("{\\t(<t1,t2,accel,style modifiers>)}",  "Animated transform", false),
        };

        private static readonly List<string> LastAddedTags = new List<string>();

        public static void AddUsedTag(string tag)
        {
            LastAddedTags.Add(tag);
        }

        public static void CompleteItem(SETextBox tb, IntellisenseItem item)
        {
            tb.SuspendLayout();

            // remove old tag if any
            if (!string.IsNullOrEmpty(item.ActiveTagAtCursor) && tb.SelectionLength == 0)
            {
                RemoveTagAtCursor(tb);
            }

            // insert new tag
            var oldStart = tb.SelectionStart;
            if (item.TypedWord.Length > 0)
            {
                tb.SelectedText = item.Value.Remove(0, item.TypedWord.Length);
            }
            else
            {
                tb.SelectedText = item.Value;
            }
            var newStart = oldStart + item.Value.Length;

            // merge tags before/after
            var subtract = MergeTagAtCursor(tb, oldStart);
            subtract += MergeTagAtCursor(tb, newStart);
            tb.SelectionStart = newStart - subtract - item.TypedWord.Length;

            tb.ResumeLayout();

            AddUsedTag(item.Value);
        }

        public static void RemoveTagAtCursor(SETextBox tb)
        {
            if (tb.SelectionStart == 0 || tb.SelectedText.Length > 0)
            {
                return;
            }

            var before = tb.Text.Substring(0, tb.SelectionStart);
            var after = tb.Text.Remove(0, tb.SelectionStart);

            // no tag?
            var lastIndexOfEndBracket = before.LastIndexOf('}');
            var lastIndexOfBackslash = before.LastIndexOf('\\');
            if (lastIndexOfEndBracket > lastIndexOfBackslash ||
                lastIndexOfBackslash == -1 && lastIndexOfEndBracket == -1 && !before.EndsWith('{') && after.StartsWith('\\'))
            {
                return;
            }


            var start = lastIndexOfBackslash;
            var extra = 0;
            if (start == -1) // cursor right after "{"
            {
                if (tb.Text.Length > 2)
                {
                    var nextEndTagIndex = tb.Text.IndexOfAny(new[] { '\\', '}' }, 2);
                    if (nextEndTagIndex <= 0 || tb.Text[nextEndTagIndex] != '\\')
                    {
                        start = 0;
                        extra = 1;
                    }
                    else
                    {
                        start = 1;
                    }
                }
                else
                {
                    start = 0;
                    extra = 1;
                }
            }
            else if (start > 0 && tb.Text[start - 1] == '{')
            {
                var nextEndTagIndex = tb.Text.IndexOfAny(new[] { '\\', '}' }, start + 1);
                if (nextEndTagIndex <= 0 || tb.Text[nextEndTagIndex] != '\\')
                {
                    start--;
                    extra = 1;
                }
            }

            var startLetter = tb.Text[start];
            if (start + 1 > tb.Text.Length)
            {
                return;
            }

            var endTagIndex = tb.Text.IndexOfAny(new[] { '\\', '}' }, start + 1 + extra);
            if (endTagIndex > 0)
            {
                if (startLetter == '\\')
                {
                    tb.Text = tb.Text.Remove(start, endTagIndex - start);
                }
                else
                {
                    tb.Text = tb.Text.Remove(start, endTagIndex - start + 1);
                }

                tb.SelectionStart = start; // position cursor
            }
        }

        public static int MergeTagAtCursor(SETextBox tb, int cursorPosition)
        {
            if (cursorPosition >= tb.Text.Length)
            {
                return 0;
            }

            if (cursorPosition > 0 && tb.Text[cursorPosition - 1] == '}' && tb.Text[cursorPosition] == '{')
            {
                tb.Text = tb.Text.Remove(cursorPosition - 1, 2);
                return 2;
            }

            if (cursorPosition > 0 && tb.Text[cursorPosition - 1] == '\\' && tb.Text[cursorPosition] == '{')
            {
                tb.Text = tb.Text.Remove(cursorPosition - 1, 2);
                return 2;
            }

            if (cursorPosition > 1 && tb.Text[cursorPosition - 2] == '}' && tb.Text[cursorPosition - 1] == '{')
            {
                tb.Text = tb.Text.Remove(cursorPosition - 2, 2);
                return 2;
            }

            return 0;
        }

        public static bool AutoCompleteTextBox(SETextBox textBox, ListBox listBox)
        {
            var textBeforeCursor = string.IsNullOrEmpty(textBox.Text) ? string.Empty : textBox.Text.Substring(0, textBox.SelectionStart);
            var activeTagAtCursor = GetInsideTag(textBox, textBeforeCursor);
            var activeTagToCursor = GetLastString(textBeforeCursor) ?? string.Empty;
            var keywords = (activeTagToCursor.StartsWith('\\') ? Keywords.Select(p => new IntellisenseItem(p.Value.TrimStart('{'), p.Hint, p.AllowInTransformations)) : Keywords.Select(p => p)).ToList();

            if (textBeforeCursor.EndsWith("\\t(", StringComparison.Ordinal))
            {
                // use smaller list inside transformation
                keywords = Keywords.Where(p => p.AllowInTransformations).Select(p => new IntellisenseItem(p.Value.TrimStart('{'), p.Hint, p.AllowInTransformations)).ToList();
                activeTagToCursor = string.Empty;
            }

            var filteredList = keywords
                .Where(n => string.IsNullOrEmpty(activeTagToCursor) || n.Value.StartsWith(GetLastString(activeTagToCursor), StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredList.Count == 0 && activeTagToCursor.EndsWith("\\"))
            {
                // continuing ass tag, remove "{\" + "}"
                activeTagToCursor = string.Empty;
                filteredList = keywords
                    .Select(p => new IntellisenseItem(p.Value.Replace("{\\", string.Empty).RemoveChar('}'), p.Hint, p.AllowInTransformations))
                    .ToList();
            }

            listBox.Items.Clear();

            foreach (var item in filteredList)
            {
                item.TypedWord = activeTagToCursor;
                item.ActiveTagAtCursor = activeTagAtCursor;
                item.Font = listBox.Font;
                listBox.Items.Add(item);
            }

            if (listBox.Items.Count == 0)
            {
                return false;
            }

            listBox.SelectedIndex = 0;
            var endTag = GetEndTagFromLastTagInText(textBeforeCursor);
            if (!string.IsNullOrEmpty(endTag))
            {
                var item = filteredList.Find(p => p.Value == endTag);
                if (item != null)
                {
                    listBox.SelectedIndex = filteredList.IndexOf(item);
                }
            }
            else if (IsLastTwoTagsEqual())
            {
                var item = filteredList.Find(p => p.Value == LastAddedTags.Last());
                if (item != null)
                {
                    listBox.SelectedIndex = filteredList.IndexOf(item);
                }
            }

            if (Configuration.Settings.General.UseDarkTheme)
            {
                DarkTheme.SetDarkTheme(listBox);
            }

            listBox.Width = 480;
            listBox.Height = 200;
            var height = listBox.Items.Count * listBox.ItemHeight + listBox.Items.Count + listBox.ItemHeight;
            if (height < listBox.Height)
            {
                listBox.Height = height;
            }

            listBox.Visible = true;
            return true;
        }

        private static string GetInsideTag(SETextBox textBox, string before)
        {
            var lastIndexOfStartBracket = before.LastIndexOf("{", StringComparison.Ordinal);
            var lastStart = Math.Max(before.LastIndexOf("\\", StringComparison.Ordinal), lastIndexOfStartBracket);
            if (lastStart < 0)
            {
                return string.Empty;
            }

            var extra = (lastIndexOfStartBracket == lastStart ? 1 : 0);
            var endTagIndex = textBox.Text.IndexOfAny(new[] { '\\', '}' }, textBox.SelectionStart + extra);
            if (endTagIndex > 0)
            {
                var s = textBox.Text.Substring(lastStart, endTagIndex - lastStart + extra);
                return s;
            }

            return string.Empty;
        }

        private static bool IsLastTwoTagsEqual()
        {
            return LastAddedTags.Count >= 2 &&
                   LastAddedTags[LastAddedTags.Count - 1] == LastAddedTags[LastAddedTags.Count - 2];
        }
        private static string GetEndTagFromLastTagInText(string text)
        {
            var lastIdx = text.LastIndexOf("\\", StringComparison.Ordinal);
            if (lastIdx >= 0)
            {
                var s = text.Substring(lastIdx);
                if (s.StartsWith("\\i1}"))
                {
                    return "{\\i0}";
                }

                if (s.StartsWith("\\u1}"))
                {
                    return "{\\u0}";
                }

                if (s.StartsWith("\\b1}"))
                {
                    return "{\\b0}";
                }

                if (s.StartsWith("\\s1}"))
                {
                    return "{\\s0}";
                }

                if (s.StartsWith("\\be1}"))
                {
                    return "{\\be0}";
                }
            }

            return string.Empty;
        }

        private static string GetLastString(string s)
        {
            if (s.TrimEnd().EndsWith('\\') || s.TrimEnd().EndsWith('\\'))
            {
                if (s.EndsWith("{\\", StringComparison.OrdinalIgnoreCase))
                {
                    return "{\\";
                }

                return string.Empty;
            }

            var lastIndexOfStartBracket = s.LastIndexOf('\\');
            var lastSeparatorIndex = s.LastIndexOfAny(new[] { '{', '}' });
            if (lastIndexOfStartBracket > lastSeparatorIndex)
            {
                return s.Remove(0, lastIndexOfStartBracket);
            }

            if (lastSeparatorIndex > 0)
            {
                s = s.Remove(0, lastSeparatorIndex - 1);
                if (Keywords.Any(p => p.Value.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                {
                    return s;
                }
            }
            else
            {
                if (Keywords.Any(p => p.Value.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                {
                    return s;
                }
            }

            return string.Empty;
        }
    }
}