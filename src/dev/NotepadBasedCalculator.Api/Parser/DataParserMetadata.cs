﻿namespace NotepadBasedCalculator.Api
{
    public sealed class DataParserMetadata
    {
        public string[] CultureCodes { get; set; }

        public DataParserMetadata(IDictionary<string, object> metadata)
        {
            if (metadata.TryGetValue(nameof(CultureAttribute.CultureCode), out object value))
            {
                if (value is string culture)
                {
                    CultureCodes = new[] { culture };
                }
                else if (value is string[] cultures)
                {
                    CultureCodes = cultures;
                }
                else
                {
                    ThrowHelper.ThrowInvalidDataException($"Unable to understand MEF's '{nameof(CultureAttribute.CultureCode)}' information.");
                }
            }
            else
            {
                CultureCodes = new[] { SupportedCultures.Any };
            }
        }
    }
}