﻿using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.NumberWithUnit;
using NotepadBasedCalculator.BuiltInPlugins.Grammars;
using UnitsNet;
using UnitsNet.Units;

namespace NotepadBasedCalculator.BuiltInPlugins.Data
{
    [Export(typeof(IDataParser))]
    [Culture(SupportedCultures.Any)]
    [Shared]
    public sealed class TemperatureDataParser : IDataParser
    {
        private const string Value = "value";
        private const string Unit = "unit";

        private readonly UnitMapProvider _unitMapProvider;

        [ImportingConstructor]
        public TemperatureDataParser(UnitMapProvider unitMapProvider)
        {
            _unitMapProvider = unitMapProvider;
        }

        public IReadOnlyList<IData>? Parse(string culture, TokenizedTextLine tokenizedTextLine, CancellationToken cancellationToken)
        {
            List<ModelResult> modelResults = NumberWithUnitRecognizer.RecognizeTemperature(tokenizedTextLine.LineTextIncludingLineBreak, culture);
            cancellationToken.ThrowIfCancellationRequested();

            var data = new List<IData>();

            if (modelResults.Count > 0)
            {
                UnitMap unitMap = _unitMapProvider.LoadUnitMap(culture);

                for (int i = 0; i < modelResults.Count; i++)
                {
                    ModelResult modelResult = modelResults[i];

                    if (modelResult.Resolution is not null)
                    {
                        string valueString = (string)modelResult.Resolution[Value];
                        string unit = (string)modelResult.Resolution[Unit];

                        if (unitMap.Temperature.TryGetValue(unit, out TemperatureUnit temperatureUnit))
                        {
                            data.Add(
                                new TemperatureData(
                                    tokenizedTextLine.LineTextIncludingLineBreak,
                                    modelResult.Start,
                                    modelResult.End + 1,
                                    new Temperature(double.Parse(valueString), temperatureUnit)));
                        }
                    }
                }
            }

            return data;
        }
    }
}
