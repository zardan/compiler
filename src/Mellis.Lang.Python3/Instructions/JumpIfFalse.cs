﻿using Mellis.Core.Entities;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3.VM;

namespace Mellis.Lang.Python3.Instructions
{
    public class JumpIfFalse : Jump
    {
        public JumpIfFalse(SourceReference source, int target = -1)
            : base(source, target)
        {
        }

        public override void Execute(PyProcessor processor)
        {
            IScriptType value = processor.PopValue();

            if (!value.IsTruthy())
            {
                processor.JumpToInstruction(Target);
            }
        }

        public override string ToString()
        {
            return $"jmpifn->@{Target}";
        }
    }
}