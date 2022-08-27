﻿using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.NumberWithUnit;
using NotepadBasedCalculator.BuiltInPlugins.Data.Definition;

namespace NotepadBasedCalculator.BuiltInPlugins.Data
{
    [Export(typeof(IDataParser))]
    [Culture(SupportedCultures.Any)]
    [Shared]
    public sealed class CurrencyDataParser : IDataParser
    {
        private const string Value = "value";
        private const string Unit = "unit";
        private const string IsoCurrency = "isoCurrency";
        private const string TypeName = "currency";

        public IReadOnlyList<IData>? Parse(string culture, TokenizedTextLine tokenizedTextLine, CancellationToken cancellationToken)
        {
            List<ModelResult> modelResults = NumberWithUnitRecognizer.RecognizeCurrency(tokenizedTextLine.LineTextIncludingLineBreak, culture);
            cancellationToken.ThrowIfCancellationRequested();

            var data = new List<IData>();

            for (int i = 0; i < modelResults.Count; i++)
            {
                ModelResult modelResult = modelResults[i];
                switch (modelResult.TypeName)
                {
                    case TypeName:
                        string valueString = (string)modelResult.Resolution[Value];
                        string unit = (string)modelResult.Resolution[Unit];
                        string isoCurrency = string.Empty;
                        if (modelResult.Resolution.TryGetValue(IsoCurrency, out object? isoCurrencyObject))
                        {
                            isoCurrency = isoCurrencyObject as string ?? string.Empty;
                        }

                        data.Add(
                            new CurrencyData(
                                tokenizedTextLine.LineTextIncludingLineBreak,
                                modelResult.Start,
                                modelResult.End + 1,
                                new CurrencyValue(
                                    double.Parse(valueString),
                                    unit,
                                    isoCurrency)));

                        break;

                    default:
#if DEBUG
                        ThrowHelper.ThrowNotSupportedException();
#endif
                        break;
                }
            }

            return data;
        }
    }
}
