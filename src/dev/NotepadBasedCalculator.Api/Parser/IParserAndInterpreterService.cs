﻿namespace NotepadBasedCalculator.Api
{
    public interface IParserAndInterpreterService
    {
        Task<bool> TryParseAndInterpretExpressionAsync(
            string culture,
            LinkedToken? currentToken,
            IVariableService variableService,
            ExpressionParserAndInterpreterResult result,
            CancellationToken cancellationToken);

        Task<bool> TryParseAndInterpretExpressionAsync(
            string expressionParserName,
            string culture,
            LinkedToken? currentToken,
            IVariableService variableService,
            ExpressionParserAndInterpreterResult result,
            CancellationToken cancellationToken);

        Task<bool> TryParseAndInterpretExpressionAsync(
            string culture,
            LinkedToken? currentToken,
            string? parseUntilTokenIsOfType,
            string? parseUntilTokenHasText,
            IVariableService variableService,
            ExpressionParserAndInterpreterResult result,
            CancellationToken cancellationToken);
    }
}
