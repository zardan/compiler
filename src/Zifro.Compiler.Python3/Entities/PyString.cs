﻿using Zifro.Compiler.Core.Interfaces;
using Zifro.Compiler.Lang.Entities;

namespace Zifro.Compiler.Lang.Python3.Entities
{
    public class PyString : StringBase
    {
        public PyString(string value, PyProcessor processor)
        {
            Value = value ?? string.Empty;
            Processor = processor;
        }

        /// <inheritdoc />
        public override IScriptType GetTypeDef()
        {
            throw new System.NotImplementedException();
        }
    }
}