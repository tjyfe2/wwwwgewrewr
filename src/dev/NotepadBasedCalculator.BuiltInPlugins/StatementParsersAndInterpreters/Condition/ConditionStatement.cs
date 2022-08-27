﻿namespace NotepadBasedCalculator.BuiltInPlugins.StatementParsersAndInterpreters.Condition
{
    internal sealed class ConditionStatement : Statement
    {
        internal Expression Condition { get; }

        internal IData? StatementResult { get; }

        internal ConditionStatement(LinkedToken firstToken, LinkedToken lastToken, Expression condition, IData? statementResult)
            : base(firstToken, lastToken)
        {
            Condition = condition;
            StatementResult = statementResult;
        }

        public override string ToString()
        {
            return "Condition";
        }
    }
}
