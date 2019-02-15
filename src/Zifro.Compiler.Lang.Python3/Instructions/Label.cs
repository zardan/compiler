﻿using Zifro.Compiler.Core.Entities;
using Zifro.Compiler.Lang.Python3.Interfaces;

namespace Zifro.Compiler.Lang.Python3.Instructions
{
    public class Label : IOpCode
    {
        public Label(SourceReference source)
        {
            Source = source;
            OpCodeIndex = -1;
        }

        public SourceReference Source { get; }

        public int OpCodeIndex { get; internal set; }

        public void Execute(PyProcessor processor)
        {
            // do nothing
        }

        public override string ToString()
        {
            if (OpCodeIndex != -1)
                return $"label@{OpCodeIndex}";
            return "label";
        }
    }
}