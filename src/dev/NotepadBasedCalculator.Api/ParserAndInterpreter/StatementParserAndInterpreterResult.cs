﻿namespace NotepadBasedCalculator.Api
{
    public class StatementParserAndInterpreterResult
    {
        public IData? ResultedData { get; set; } = null;

        public Statement? ParsedStatement { get; set; } = null;

        public DataOperationException? Error { get; set; } = null;
    }
}
