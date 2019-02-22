﻿using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Mellis.Core.Exceptions;
using Mellis.Lang.Python3.Extensions;
using Mellis.Lang.Python3.Grammar;
using Mellis.Lang.Python3.Resources;
using Mellis.Lang.Python3.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Mellis.Lang.Python3.Tests.SyntaxConstructor.TestTree
{
    [TestClass]
    public class VisitTrailerTests : BaseVisitClass<
        Python3Parser.TrailerContext>
    {
        public override SyntaxNode VisitContext()
        {
            return ctor.VisitTrailer(contextMock.Object);
        }

        protected Mock<Python3Parser.ArglistContext> GetArglistMock()
        {
            return GetMockRule<Python3Parser.ArglistContext>();
        }

        protected void SetupForArglist(Mock<Python3Parser.ArglistContext> innerMock, params ExpressionNode[] arguments)
        {
            var args = new IParseTree[arguments.Length * 2];

            for (var i = 0; i < arguments.Length; i++)
            {
                ExpressionNode arg = arguments[i];
                var argMock = GetMockRule<Python3Parser.ArgumentContext>();

                ctorMock.Setup(o => o.VisitArgument(argMock.Object))
                    .Returns(arg).Verifiable();

                args[i * 2] = argMock.Object;
                args[i * 2 + 1] = GetTerminal(Python3Parser.COMMA);
            }

            innerMock.SetupChildren(
                args
            );
        }

        [TestMethod]
        public void TestNonClosingParentheses()
        {
            // Arrange
            ITerminalNode opening = GetTerminal(Python3Parser.OPEN_PAREN);
            contextMock.SetupChildren(
                opening
            );

            // Act
            var ex = Assert.ThrowsException<SyntaxException>(VisitContext);

            // Assert
            Assert.That.ErrorSyntaxFormatArgsEqual(ex,
                nameof(Localized_Python3_Parser.Ex_Parenthesis_NoClosing),
                opening,
                ")"
            );
        }

        [TestMethod]
        public void TestFunctionCallIsCreated()
        {
            // Arrange
            contextMock.SetupForSourceReference(startTokenMock, stopTokenMock);
            contextMock.SetupChildren(
                GetTerminal(Python3Parser.OPEN_PAREN),
                GetTerminal(Python3Parser.CLOSE_PAREN)
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            Assert.IsInstanceOfType(result, typeof(FunctionArguments));
            Assert.AreEqual(0, ((FunctionArguments) result).Count);
        }

        [TestMethod]
        public void TestFunctionCallWithArguments()
        {
            // Arrange
            var arg1 = GetExpressionMock();
            var arg2 = GetExpressionMock();
            var arg3 = GetExpressionMock();

            var arglistMock = GetArglistMock();
            SetupForArglist(arglistMock,
                arg1, arg2, arg3
            );

            contextMock.SetupForSourceReference(startTokenMock, stopTokenMock);
            contextMock.SetupChildren(
                GetTerminal(Python3Parser.OPEN_PAREN),
                arglistMock.Object,
                GetTerminal(Python3Parser.CLOSE_PAREN)
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            Assert.IsInstanceOfType(result, typeof(FunctionArguments));
            var args = (FunctionArguments) result;
            Assert.AreSame(arg1, args[0]);
            Assert.AreSame(arg2, args[1]);
            Assert.AreSame(arg3, args[2]);
            Assert.AreEqual(3, args.Count);
        }
    }
}