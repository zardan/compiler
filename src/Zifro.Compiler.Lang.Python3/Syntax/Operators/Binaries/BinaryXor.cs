﻿using Zifro.Compiler.Lang.Python3.Instructions;

namespace Zifro.Compiler.Lang.Python3.Syntax.Operators.Binaries
{
    public class BinaryXor : BinaryOperator
    {
        public BinaryXor(
            ExpressionNode leftOperand,
            ExpressionNode rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        protected override BaseBinaryOp GetOp()
        {
            return new DivOp(Source);
        }
    }
}