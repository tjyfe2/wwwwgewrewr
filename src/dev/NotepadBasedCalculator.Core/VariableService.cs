﻿namespace NotepadBasedCalculator.Core
{
    internal sealed class VariableService : IVariableService
    {
        private readonly List<IReadOnlyDictionary<string, IData?>> _variablePerLineBackup = new();

        private Dictionary<string, IData?> _variables = new();

        public IData? GetVariableValue(string variableName)
        {
            Guard.IsNotNullOrWhiteSpace(variableName);
            lock (_variables)
            {
                if (_variables.TryGetValue(variableName, out IData? value))
                {
                    return value;
                }

                return null;
            }
        }

        public void SetVariableValue(string variableName, IData? value)
        {
            Guard.IsNotNullOrWhiteSpace(variableName);
            lock (_variables)
            {
                _variables[variableName] = value;
            }
        }

        internal IDisposable BeginRecordVariableSnapshot(int lineNumber, CancellationToken cancellationToken)
        {
            // Restore variables from the previous line.
            lock (_variables)
            {
                Guard.IsGreaterThanOrEqualTo(lineNumber, 0);
                if (lineNumber - 1 >= 0 && lineNumber - 1 < _variablePerLineBackup.Count)
                {
                    IReadOnlyDictionary<string, IData?> variablesFromPreviousLine = _variablePerLineBackup[lineNumber - 1];
                    _variables = variablesFromPreviousLine.ToDictionary(kv => kv.Key, kv => kv.Value);
                }
                else
                {
                    _variables = new();
                }

                return new VariableRecorder(this, lineNumber, cancellationToken);
            }
        }

        private void TakeVariableSnapshot(int lineNumber)
        {
            lock (_variables)
            {
                var snapshot = new Dictionary<string, IData?>(_variables);
                if (_variablePerLineBackup.Count > lineNumber)
                {
                    _variablePerLineBackup[lineNumber] = snapshot;
                }
                else
                {
                    _variablePerLineBackup.Add(snapshot);
                }
            }
        }

        internal IReadOnlyList<string> GetVariableNames()
        {
            lock (_variables)
            {
                return _variables.Keys.ToList();
            }
        }

        internal IReadOnlyDictionary<string, IData?> CreateBackup()
        {
            lock (_variables)
            {
                return new Dictionary<string, IData?>(_variables);
            }
        }

        internal void RestoreBackup(IReadOnlyDictionary<string, IData?>? backup)
        {
            lock (_variables)
            {
                if (backup is null)
                {
                    _variables = new();
                }
                else
                {
                    _variables = backup.ToDictionary(kv => kv.Key, kv => kv.Value);
                }
            }
        }

        private class VariableRecorder : IDisposable
        {
            private readonly VariableService _variableService;
            private readonly int _lineNumber;
            private readonly CancellationToken _cancellationToken;

            internal VariableRecorder(VariableService variableService, int lineNumber, CancellationToken cancellationToken)
            {
                Guard.IsNotNull(variableService);
                Guard.IsGreaterThanOrEqualTo(lineNumber, 0);
                _variableService = variableService;
                _lineNumber = lineNumber;
                _cancellationToken = cancellationToken;
            }

            public void Dispose()
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    _variableService.TakeVariableSnapshot(_lineNumber);
                }
            }
        }
    }
}
