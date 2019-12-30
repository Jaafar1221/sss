﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.NetflixQualityCheck;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using static Nikse.SubtitleEdit.Forms.FixCommonErrors;

namespace Nikse.SubtitleEdit.Forms
{
    public partial class NetflixFixErrors : Form
    {
        private readonly Subtitle _subtitle;
        private readonly SubtitleFormat _subtitleFormat;
        private readonly bool _loading;
        private NetflixQualityController _netflixQualityController;

        public NetflixFixErrors(Subtitle subtitle, SubtitleFormat subtitleFormat)
        {
            InitializeComponent();

            _subtitle = subtitle;
            _subtitleFormat = subtitleFormat;

            _loading = true;
            var language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
            InitializeLanguages(language);
            RefreshCheckBoxes(language);
            _loading = false;
            RuleCheckedChanged(null, null);
        }

        private void RefreshCheckBoxes(string language)
        {
            _netflixQualityController = new NetflixQualityController { Language = language };

            checkBoxNoItalics.Checked = !_netflixQualityController.AllowItalics;
            checkBoxNoItalics.Enabled = !_netflixQualityController.AllowItalics;

            var checkFrameRate = _subtitleFormat.GetType() == new NetflixTimedText().GetType();
            checkBoxTtmlFrameRate.Checked = checkFrameRate;
            checkBoxTtmlFrameRate.Enabled = checkFrameRate;

            checkBoxDialogHypenNoSpace.Checked = _netflixQualityController.DualSpeakersHasHyphenAndNoSpace;
            checkBoxDialogHypenNoSpace.Enabled = _netflixQualityController.DualSpeakersHasHyphenAndNoSpace;

            checkBox17CharsPerSecond.Text = string.Format("Maximum {0} characters per second (excl. white spaces)", _netflixQualityController.CharactersPerSecond);
            checkBoxMaxLineLength.Text = string.Format("Maximum line length ({0})", _netflixQualityController.SingleLineMaxLength);
        }

        private void InitializeLanguages(string language)
        {
            comboBoxLanguage.BeginUpdate();
            comboBoxLanguage.Items.Clear();
            var ci = CultureInfo.GetCultureInfo(language);
            foreach (var x in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if (!string.IsNullOrWhiteSpace(x.ToString()) && !x.EnglishName.Contains("("))
                {
                    comboBoxLanguage.Items.Add(new LanguageItem(x, x.EnglishName));
                }
            }
            comboBoxLanguage.Sorted = true;
            int languageIndex = 0;
            int j = 0;
            foreach (var x in comboBoxLanguage.Items)
            {
                var li = (LanguageItem)x;
                if (li.Code.TwoLetterISOLanguageName == ci.TwoLetterISOLanguageName)
                {
                    languageIndex = j;
                    break;
                }
                if (li.Code.TwoLetterISOLanguageName == "en")
                {
                    languageIndex = j;
                }
                j++;
            }
            comboBoxLanguage.SelectedIndex = languageIndex;
            comboBoxLanguage.SelectedIndexChanged += RuleCheckedChanged;
            comboBoxLanguage.EndUpdate();
        }

        private void RuleCheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            _netflixQualityController.RunChecks(_subtitle, GetAllSelectedChecks());
            listViewFixes.BeginUpdate();
            listViewFixes.Items.Clear();
            foreach (var record in _netflixQualityController.Records)
            {
                AddFixToListView(
                    record.OriginalParagraph,
                    record.Comment,
                    record.OriginalParagraph != null ? record.OriginalParagraph.ToString() : string.Empty,
                    record.FixedParagraph != null ? record.FixedParagraph.ToString() : string.Empty);
            }
            listViewFixes.EndUpdate();
        }

        private void AddFixToListView(Paragraph p, string action, string before, string after)
        {
            var item = new ListViewItem(string.Empty) { Checked = true, Tag = p };
            item.SubItems.Add(p.Number.ToString());
            item.SubItems.Add(action);
            item.SubItems.Add(before.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(after.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            listViewFixes.Items.Add(item);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private List<INetflixQualityChecker> GetAllSelectedChecks()
        {
            var list = new List<INetflixQualityChecker>();
            if (checkBoxMinDuration.Checked)
            {
                list.Add(new NetflixCheckMinDuration());
            }

            if (checkBoxMaxDuration.Checked)
            {
                list.Add(new NetflixCheckMaxDuration());
            }

            if (checkBox17CharsPerSecond.Checked)
            {
                list.Add(new NetflixCheckMaxCps());
            }

            if (checkBoxGapMinTwoFrames.Checked)
            {
                list.Add(new NetflixCheckTwoFramesGap());
            }

            if (checkBoxTwoLinesMax.Checked)
            {
                list.Add(new NetflixCheckNumberOfLines());
            }

            if (checkBoxDialogHypenNoSpace.Checked)
            {
                list.Add(new NetflixCheckDialogHyphenSpace());
            }

            if (checkBoxSquareBracketForHi.Checked)
            {
                list.Add(new NetflixCheckTextForHiUseBrackets());
            }

            if (checkBoxSpellOutStartNumbers.Checked)
            {
                list.Add(new NetflixCheckStartNumberSpellOut());
            }

            if (checkBoxWriteOutOneToTen.Checked)
            {
                list.Add(new NetflixCheckNumbersOneToTenSpellOut());
            }

            if (checkBoxCheckValidGlyphs.Checked)
            {
                list.Add(new NetflixCheckGlyph());
            }

            if (checkBoxNoItalics.Checked)
            {
                list.Add(new NetflixCheckItalics());
            }

            if (checkBoxTtmlFrameRate.Checked)
            {
                list.Add(new NetflixCheckTimedTextFrameRate());
            }

            if (checkBoxMaxLineLength.Checked)
            {
                list.Add(new NetflixCheckMaxLineLength());
            }

            if (checkBoxWhiteSpace.Checked)
            {
                list.Add(new NetflixCheckWhiteSpace());
            }

            return list;
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            var language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
            InitializeLanguages(language);
            RefreshCheckBoxes(language);
        }
    }
}
