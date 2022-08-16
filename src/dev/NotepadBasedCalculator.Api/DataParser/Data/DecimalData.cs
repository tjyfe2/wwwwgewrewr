﻿namespace NotepadBasedCalculator.Api
{
    public sealed record DecimalData : Data<float>, INumericData
    {
        public bool IsNegative => Value < 0;

        public DecimalData(string lineTextIncludingLineBreak, int startInLine, int endInLine, float value)
            : base(
                  lineTextIncludingLineBreak,
                  startInLine,
                  endInLine,
                  value,
                  PredefinedTokenAndDataTypeNames.Numeric,
                  PredefinedTokenAndDataTypeNames.SubDataTypeNames.Decimal)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}