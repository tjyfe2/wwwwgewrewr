﻿using NotepadBasedCalculator.BuiltInPlugins.StatementParsersAndInterpreters.Comment;

namespace NotepadBasedCalculator.BuiltInPlugins.Statements.Comment
{
    [Export(typeof(IStatementParser))]
    [Culture(SupportedCultures.Any)]
    [Order(int.MinValue)]
    internal sealed class CommentStatementParser : ParserBase, IStatementParser
    {
        public bool TryParseStatement(string culture, LinkedToken currentToken, out Statement? statement)
        {
            if (currentToken.Token.Is(PredefinedTokenAndDataTypeNames.CommentOperator))
            {
                LinkedToken lastTokenInLine = currentToken;
                LinkedToken? nextToken = currentToken.Next;
                while (nextToken is not null)
                {
                    lastTokenInLine = nextToken;
                    nextToken = nextToken.Next;
                }

                statement = new CommentStatement(DiscardWords(currentToken)!, lastTokenInLine);
                return true;
            }

            statement = null;
            return false;
        }
    }
}
