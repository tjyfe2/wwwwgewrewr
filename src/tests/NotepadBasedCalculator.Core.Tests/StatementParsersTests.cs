﻿using System.Threading.Tasks;
using NotepadBasedCalculator.Api;
using NotepadBasedCalculator.BuiltInPlugins.Statements.Comment;
using NotepadBasedCalculator.BuiltInPlugins.Statements.Condition;
using NotepadBasedCalculator.BuiltInPlugins.Statements.Header;
using NotepadBasedCalculator.BuiltInPlugins.Statements.NumericalCalculus;
using Xunit;

namespace NotepadBasedCalculator.Core.Tests
{
    public sealed class StatementParsersTests : MefBaseTest
    {
        [Fact]
        public async Task CommentStatement()
        {
            Parser parser = ExportProvider.Import<Parser>();
            ParserResult parserResult = await parser.ParseAsync(" 132 //comment");
            Assert.Equal(2, parserResult.Lines[0].Statements.Count);
            Statement statement = parserResult.Lines[0].Statements[1];
            Assert.IsType<CommentStatement>(statement);
            Assert.Equal("[comment_operators] (5, 7): '//'", statement.FirstToken.ToString());
            Assert.Equal("[Word] (7, 14): 'comment'", statement.LastToken.ToString());

            parserResult = await parser.ParseAsync(" 132 // comment");
            Assert.Equal(2, parserResult.Lines[0].Statements.Count);
            statement = parserResult.Lines[0].Statements[1];
            Assert.IsType<CommentStatement>(statement);
            Assert.Equal("[comment_operators] (5, 7): '//'", statement.FirstToken.ToString());
            Assert.Equal("[Word] (8, 15): 'comment'", statement.LastToken.ToString());

            parserResult = await parser.ParseAsync(" 132 // comment // comment 2");
            Assert.Equal(2, parserResult.Lines[0].Statements.Count);
            statement = parserResult.Lines[0].Statements[1];
            Assert.IsType<CommentStatement>(statement);
            Assert.Equal("[comment_operators] (5, 7): '//'", statement.FirstToken.ToString());
            Assert.Equal("[Numeric] (27, 28): '2'", statement.LastToken.ToString());

            parserResult = await parser.ParseAsync(" 132 / / comment");
            Assert.Single(parserResult.Lines[0].Statements);
            statement = parserResult.Lines[0].Statements[0];
            Assert.IsType<NumericalCalculusStatement>(statement);
        }

        [Fact]
        public async Task HeaderStatement()
        {
            Parser parser = ExportProvider.Import<Parser>();
            ParserResult parserResult = await parser.ParseAsync("#Header");
            Assert.Single(parserResult.Lines[0].Statements);
            Statement statement = parserResult.Lines[0].Statements[0];
            Assert.IsType<HeaderStatement>(statement);
            Assert.Equal("[header_operators] (0, 1): '#'", statement.FirstToken.ToString());
            Assert.Equal("[Word] (1, 7): 'Header'", statement.LastToken.ToString());

            parserResult = await parser.ParseAsync(" # Header");
            Assert.Single(parserResult.Lines[0].Statements);
            statement = parserResult.Lines[0].Statements[0];
            Assert.IsType<HeaderStatement>(statement);
            Assert.Equal("[header_operators] (1, 2): '#'", statement.FirstToken.ToString());
            Assert.Equal("[Word] (3, 9): 'Header'", statement.LastToken.ToString());

            parserResult = await parser.ParseAsync(" ### Header");
            Assert.Single(parserResult.Lines[0].Statements);
            statement = parserResult.Lines[0].Statements[0];
            Assert.IsType<HeaderStatement>(statement);
            Assert.Equal("[header_operators] (1, 4): '###'", statement.FirstToken.ToString());
            Assert.Equal("[Word] (5, 11): 'Header'", statement.LastToken.ToString());

            parserResult = await parser.ParseAsync(" 132 # Header");
            Assert.Single(parserResult.Lines[0].Statements);
            statement = parserResult.Lines[0].Statements[0];
            Assert.IsType<NumericalCalculusStatement>(statement);
        }

        [Fact]
        public async Task ConditionStatement()
        {
            Parser parser = ExportProvider.Import<Parser>();
            ParserResult parserResult = await parser.ParseAsync(" if 23 less than or equal to twenty four then ");
            Assert.Single(parserResult.Lines[0].Statements);
            var statement = (ConditionStatement)parserResult.Lines[0].Statements[0];
            var condition = (BinaryOperatorExpression)statement.Condition;
            Assert.Equal("([Numeric] (4, 6): '23' <= [Numeric] (29, 40): 'twenty four')", condition.ToString());

            parserResult = await parser.ParseAsync("23 == twenty three");
            Assert.Single(parserResult.Lines[0].Statements);
            statement = (ConditionStatement)parserResult.Lines[0].Statements[0];
            condition = (BinaryOperatorExpression)statement.Condition;
            Assert.Equal("([Numeric] (0, 2): '23' == [Numeric] (6, 18): 'twenty three')", condition.ToString());

            parserResult = await parser.ParseAsync("23 <= twenty three == 23");
            Assert.Equal(2, parserResult.Lines[0].Statements.Count);
            statement = (ConditionStatement)parserResult.Lines[0].Statements[0];
            condition = (BinaryOperatorExpression)statement.Condition;
            Assert.Equal("([Numeric] (0, 2): '23' <= [Numeric] (6, 18): 'twenty three')", condition.ToString());
            var statement2 = (NumericalCalculusStatement)parserResult.Lines[0].Statements[1];
            Expression expression = statement2.NumericalCalculusExpression;
            Assert.Equal("[Numeric] (22, 24): '23'", expression.ToString());

            string input = "if one hundred thousand dollars of income + (30% tax / two people) > 150k then test";

            parserResult = await parser.ParseAsync(input);
            Assert.Single(parserResult.Lines[0].Statements);
            statement = (ConditionStatement)parserResult.Lines[0].Statements[0];
            condition = (BinaryOperatorExpression)statement.Condition;
            Assert.Equal("(([Numeric] (3, 31): 'one hundred thousand dollars' + (([Numeric] (45, 48): '30%' / [Numeric] (55, 58): 'two'))) > [Numeric] (69, 73): '150k')", condition.ToString());
        }

        [Fact]
        public async Task VariableDeclarationAndReference()
        {
            string input =
@"hello There = 2
hello There + 2
   hello There     =    3
well hello There = 5
hello There
Hum well hello There how are you?
hello there";

            Parser parser = ExportProvider.Import<Parser>();
            ParserResult parserResult = await parser.ParseAsync(input);

            Assert.Equal(7, parserResult.Lines.Count);

            Assert.Single(parserResult.Lines[0].Statements);
            Assert.IsType<VariableDeclarationStatement>(parserResult.Lines[0].Statements[0]);
            Assert.Equal("Variable $(hello There) = [Numeric] (14, 15): '2'", parserResult.Lines[0].Statements[0].ToString());

            Assert.Single(parserResult.Lines[1].Statements);
            Assert.IsType<NumericalCalculusStatement>(parserResult.Lines[1].Statements[0]);
            Assert.Equal("($(hello There) + [Numeric] (14, 15): '2')", parserResult.Lines[1].Statements[0].ToString());

            Assert.Single(parserResult.Lines[2].Statements);
            Assert.IsType<VariableDeclarationStatement>(parserResult.Lines[2].Statements[0]);
            Assert.Equal("Variable $(hello There) = [Numeric] (24, 25): '3'", parserResult.Lines[2].Statements[0].ToString());

            Assert.Single(parserResult.Lines[3].Statements);
            Assert.IsType<VariableDeclarationStatement>(parserResult.Lines[3].Statements[0]);
            Assert.Equal("Variable $(well hello There) = [Numeric] (19, 20): '5'", parserResult.Lines[3].Statements[0].ToString());

            Assert.Single(parserResult.Lines[4].Statements);
            Assert.IsType<NumericalCalculusStatement>(parserResult.Lines[4].Statements[0]);
            Assert.Equal("$(hello There)", parserResult.Lines[4].Statements[0].ToString());

            Assert.Single(parserResult.Lines[5].Statements);
            Assert.IsType<NumericalCalculusStatement>(parserResult.Lines[5].Statements[0]);
            Assert.Equal("$(well hello There)", parserResult.Lines[5].Statements[0].ToString());

            Assert.Empty(parserResult.Lines[6].Statements);
        }
    }
}
