using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace Core.Services;

public static class BBCodeService
{
    public static readonly Dictionary<string, Action<FormattedText, string>> BBCodeActions = new()
    {
        ["b"] = (formattedText, _) => formattedText.SetFontWeight(FontWeight.Bold),

        ["i"] = (formattedText, _) => formattedText.SetFontStyle(FontStyle.Italic),

        ["u"] = (formattedText, _) => formattedText.SetTextDecorations(TextDecorations.Underline),

        ["s"] = (formattedText, _) => formattedText.SetTextDecorations(TextDecorations.Strikethrough),

        ["size"] = (formattedText, internalString) => formattedText.SetFontSize(double.Parse(internalString)),

        ["color"] = (formattedText, internalString) =>
        {
            var color = System.Drawing.Color.FromName(internalString);
            IBrush brush = new SolidColorBrush(new Color(color.A, color.R, color.G, color.B));
            formattedText.SetForegroundBrush(brush); // Example color
        },

        #region Effects

        ["sine"] = (formattedText, internalString) => {},

        ["flicker"] = (formattedText, internalString) => {},

        ["nervous"] = (formattedText, internalString) => {}

        #endregion
    };
}