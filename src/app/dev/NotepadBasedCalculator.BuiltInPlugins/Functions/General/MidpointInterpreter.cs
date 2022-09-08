﻿namespace NotepadBasedCalculator.BuiltInPlugins.Functions.General
{
    [Export(typeof(IFunctionInterpreter))]
    [Name("general.midpoint")]
    [Culture(SupportedCultures.English)]
    internal sealed class MidpointInterpreter : IFunctionInterpreter
    {
        public Task<IData?> InterpretFunctionAsync(
            string culture,
            FunctionDefinition functionDefinition,
            IReadOnlyList<IData> detectedData,
            CancellationToken cancellationToken)
        {
            Guard.HasSizeEqualTo(detectedData, 2);
            detectedData[0].IsOfType("numeric");
            detectedData[1].IsOfType("numeric");
            var firstNumber = (INumericData)detectedData[0];
            var secondNumber = (INumericData)detectedData[1];

            double first = firstNumber.NumericValueInStandardUnit;
            double second = secondNumber.NumericValueInStandardUnit;
            double result = (first + second) / 2;

            return Task.FromResult((IData?)secondNumber.CreateFromStandardUnit(result).MergeDataLocations(firstNumber));
        }
    }
}
