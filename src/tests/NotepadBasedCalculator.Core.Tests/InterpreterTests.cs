﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NotepadBasedCalculator.Api;
using Xunit;

namespace NotepadBasedCalculator.Core.Tests
{
    public sealed class InterpreterTests : MefBaseTest
    {
        private readonly ParserAndInterpreter _parserAndInterpreter;
        private readonly TextDocument _textDocument;

        public InterpreterTests()
        {
            ParserAndInterpreterFactory parserAndInterpreterFactory = ExportProvider.Import<ParserAndInterpreterFactory>();
            _textDocument = new TextDocument();
            _parserAndInterpreter = parserAndInterpreterFactory.CreateInstance(SupportedCultures.English, _textDocument);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _parserAndInterpreter.Dispose();
        }

        [Fact]
        public async Task Intepreter_VariableDeclaration()
        {
            _textDocument.Text = "test = 2";
            IReadOnlyList<ParserAndInterpreterResultLine> lineResults = await _parserAndInterpreter.WaitAsync();

            Assert.Single(lineResults[0].StatementsAndData);
            var statement = (VariableDeclarationStatement)lineResults[0].StatementsAndData[0].ParsedStatement;
            IData data = ((DataExpression)statement.AssignedValue).Data;
            Assert.Equal("[Numeric] (7, 8): '2'", data.ToString());
        }

        [Theory]
        [InlineData("30+30", "60")]
        [InlineData("30+20%", "36")]
        [InlineData("30 USD + 20%", "36 USD")]
        [InlineData("20%", "0.2")]
        [InlineData("20% + 20%", "0.24")]
        [InlineData("20% + 1", "1.2")]
        [InlineData("1 + 2 USD", "3 USD")]
        [InlineData("June 23 2022 at 4pm + 1h", "6/23/2022 5:00:00 PM")]
        [InlineData("(12)3+(1 +2)(3(2))(1 +2)-3", "87")]
        public async Task Intepreter_SimpleCalculus(string input, string output)
        {
            _textDocument.Text = input;
            IReadOnlyList<ParserAndInterpreterResultLine> lineResults = await _parserAndInterpreter.WaitAsync();
            Assert.Single(lineResults);
            Assert.Equal(output, lineResults[0].SummarizedResultData.DisplayText);
        }

        [Fact]
        public async Task Intepreter_DocumentChange()
        {
            _textDocument.Text = string.Empty;
            IReadOnlyList<ParserAndInterpreterResultLine> lineResults = await _parserAndInterpreter.WaitAsync();
            Assert.Empty(lineResults[0].StatementsAndData);

            TypeInDocument("test = 2");

            lineResults = await _parserAndInterpreter.WaitAsync();

            Assert.Single(lineResults[0].StatementsAndData);
            var statement = (VariableDeclarationStatement)lineResults[0].StatementsAndData[0].ParsedStatement;
            IData data = ((DataExpression)statement.AssignedValue).Data;
            Assert.Equal("[Numeric] (7, 8): '2'", data.ToString());
        }

        private void TypeInDocument(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                _textDocument.Text += text[i];
            }
        }
    }
}
