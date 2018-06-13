using System;
using DFMServer;

namespace DfmCore
{
    public class FieldInfo : ICloneable
    {
        public FieldInfo() { }

        public FieldInfo(FieldInfo fieldInfo)
        {
            if (null == fieldInfo)
            {
                throw new ArgumentNullException();
            }

            this.Name       = fieldInfo.Name;
            this.Caption    = fieldInfo.Caption;
            this.Type       = fieldInfo.Type;
            this.Length     = fieldInfo.Length;
            this.Precision  = fieldInfo.Precision;
            this.DateFormat = fieldInfo.DateFormat;
            this.IsNullable = fieldInfo.IsNullable;
        }

        public FieldInfo(FieldInfo7 fieldInfo)
        {
            if (null == fieldInfo)
            {
                throw new ArgumentNullException();
            }

            this.Name       = fieldInfo.Name;
            this.Caption    = fieldInfo.Caption;
            this.Type       = fieldInfo.Type;
            this.Length     = fieldInfo.Length;
            this.Precision  = fieldInfo.Precision;
            this.DateFormat = fieldInfo.DateFormat;
            this.IsNullable = fieldInfo.Nullability;
        }

        public string          Name       { get; set; }
        public string          Caption    { get; set; }
        public DFM_FIELD_TYPE  Type       { get; set; }
        public int             Length     { get; set; }
        public int             Precision  { get; set; }
        public DFM_DATE_FORMAT DateFormat { get; set; }
        public bool            IsNullable { get; set; }

        public FieldInfo7 ToDfmFieldInfo()
        {
            FieldInfo7 fi = new FieldInfo7
            {
                Name        = this.Name,
                Caption     = this.Caption,
                Type        = this.Type,
                Length      = this.Length,
                Precision   = this.Precision,
                DateFormat  = this.DateFormat,
                Nullability = this.IsNullable
            };

            return fi;
        }

        public object Clone()
        {
            return new FieldInfo(this);
        }

        public string FormatValue(string value)
        {
            switch (Type)
            {
                case DFM_FIELD_TYPE.DFM_FT_STRING:
                case DFM_FIELD_TYPE.DFM_FT_MEMO:
                case DFM_FIELD_TYPE.DFM_FT_DATE:
                    value = value.Replace("'", "''"); // escape quotes
                    value = $"'{value}'";             // surround with quotes
                    break;
            }

            return value;
        }

        public bool IsString => this.Type == DFM_FIELD_TYPE.DFM_FT_STRING;
    }
}
