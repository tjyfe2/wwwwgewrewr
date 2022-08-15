﻿namespace NotepadBasedCalculator.Api
{
    public class DataExpression : ReferenceExpression
    {
        public IData Data { get; }

        public DataExpression(LinkedToken firstToken, LinkedToken lastToken, IData data)
            : base(firstToken, lastToken)
        {
            Guard.IsNotNull(data);
            Data = data;
        }

        public override string ToString()
        {
            return "Data";
        }
    }
}
