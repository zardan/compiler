﻿using System;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zifro.Compiler.Core.Entities;
using Zifro.Compiler.Core.Exceptions;
using Zifro.Compiler.Lang.Python3.Grammar;
using Zifro.Compiler.Lang.Python3.Syntax;
using Zifro.Compiler.Lang.Python3.Syntax.Statements;

namespace Zifro.Compiler.Lang.Python3.Tests.SyntaxConstructor
{
    [TestClass]
    public class VisitTests
    {
        // ReSharper disable InconsistentNaming
        protected Mock<Grammar.SyntaxConstructor> ctorMock;
        protected Grammar.SyntaxConstructor ctor;
        protected Mock<SyntaxNode> nodeMock;
        protected SyntaxNode node;
        // ReSharper restore InconsistentNaming

        protected static Mock<T> GetMockRule<T>() where T : ParserRuleContext
        {
            return new Mock<T>(MockBehavior.Strict, ParserRuleContext.EmptyContext, 0);
        }

        protected void ActAndAssertVisit<TContext>(Mock<TContext> contextMock,
            Expression<Func<Grammar.SyntaxConstructor, SyntaxNode>> action, IRuleNode shouldVisit)
            where TContext : ParserRuleContext
        {
            // Arrange
            ctorMock.Setup(action)
                .Returns(node)
                .Verifiable();

            // Act
            SyntaxNode result = action.Compile().Invoke(ctor);

            // Assert
            ctorMock.Verify(o => o.VisitChildren(shouldVisit), $"Did not visit children of type <{shouldVisit.GetType().Name}>.");
            Assert.AreSame(node, result);
            contextMock.Verify();
            ctorMock.Verify();
        }

        protected Statement GetStatementMock()
        {
            return new Mock<Statement>(MockBehavior.Strict, SourceReference.ClrSource, string.Empty).Object;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ctorMock = new Mock<Grammar.SyntaxConstructor>
            {
                CallBase = true
            };
            ctor = ctorMock.Object;

            nodeMock = new Mock<SyntaxNode>(SourceReference.ClrSource, string.Empty);
            node = nodeMock.Object;
        }

        [TestMethod]
        public void FileStmtVisit_FilledStatementList_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.File_inputContext>();
            contextMock.SetupForSourceReference();
            contextMock.SetupGet(o => o.ChildCount)
                .Returns(3).Verifiable();

            var stmtMock = GetMockRule<Python3Parser.StmtContext>();
            contextMock.Setup(o => o.GetChild(It.IsInRange(0, 2, Range.Inclusive)))
                .Returns(stmtMock.Object).Verifiable();

            ctorMock.Setup(o => o.VisitStmt(stmtMock.Object)).Returns(GetStatementMock());

            // Act
            SyntaxNode result = ctor.VisitFile_input(contextMock.Object);

            // Assert
            ctorMock.Verify(o => o.VisitChildren(It.IsAny<IRuleNode>()), Times.Never);

            Assert.IsInstanceOfType(result, typeof(StatementList));
            var list = (StatementList) result;
            Assert.AreEqual(3, list.Statements.Count);

            contextMock.VerifyGet(o => o.ChildCount, Times.AtLeastOnce);
            contextMock.Verify(o => o.GetChild(0), Times.Once);
            contextMock.Verify(o => o.GetChild(1), Times.Once);
            contextMock.Verify(o => o.GetChild(2), Times.Once);

            ctorMock.Verify(o => o.VisitStmt(stmtMock.Object), Times.Exactly(3));

            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void FileStmtVisit_EmptyStatementList_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.File_inputContext>();
            contextMock.SetupForSourceReference();
            contextMock.SetupGet(o => o.ChildCount).Returns(0).Verifiable();

            // Act
            SyntaxNode result = ctor.VisitFile_input(contextMock.Object);

            // Assert
            ctorMock.Verify(o => o.VisitChildren(It.IsAny<IRuleNode>()), Times.Never);

            Assert.IsInstanceOfType(result, typeof(StatementList));
            var list = (StatementList)result;
            Assert.AreEqual(0, list.Statements.Count);

            contextMock.VerifyGet(o => o.ChildCount, Times.Once);

            ctorMock.Verify(o => o.VisitFile_input(contextMock.Object), Times.Once);

            contextMock.Verify();
            ctorMock.Verify();
        }


        [TestMethod]
        public void StmtVisit_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.StmtContext>();

            // TODO: Replace with statement node
            var stmtNodeMock = new Mock<SyntaxNode>();
            var smallStmtMock = GetMockRule<Python3Parser.Small_stmtContext>();
            ctorMock.Setup(o => o.VisitSmall_stmt(smallStmtMock.Object)).Returns(stmtNodeMock.Object);

            // Act
            ctor.VisitStmt(contextMock.Object);
            // TODO: This is all faulty, fix pls
            // Assert
            ctorMock.Verify(o => o.VisitChildren(It.IsAny<IRuleNode>()), Times.Never);
            ctorMock.VerifyNoOtherCalls();

            // TODO: Verify result is statement list

            contextMock.VerifyGet(o => o.ChildCount, Times.Once);
            contextMock.VerifyNoOtherCalls();
        }

        #region Stmt

        [TestMethod]
        public void StmtVisit_SimpleStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.StmtContext>();
            var simpleStmtMock = GetMockRule<Python3Parser.Simple_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Simple_stmtContext>(0))
                .Returns(simpleStmtMock.Object);

            // Act + Assert
            ActAndAssertVisit(
                contextMock: contextMock,
                action: o => o.VisitStmt(contextMock.Object),
                shouldVisit: simpleStmtMock.Object);
        }

        [TestMethod]
        public void StmtVisit_CompoundStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.StmtContext>();
            var compoundStmtMock = GetMockRule<Python3Parser.Compound_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Compound_stmtContext>(0))
                .Returns(compoundStmtMock.Object);

            // Act + Assert
            ActAndAssertVisit(
                contextMock: contextMock,
                action: o => o.VisitStmt(contextMock.Object),
                shouldVisit: compoundStmtMock.Object);
        }

        [TestMethod]
        public void StmtVisit_None_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.StmtContext>();

            Action action = delegate { ctor.VisitStmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxException>(action);
        }

        #endregion

        #region SmallStmt

        [TestMethod]
        public void SmallStmtVisit_ExprStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Expr_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Expr_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // Act + Assert
            ActAndAssertVisit(
                contextMock: contextMock,
                action: o => o.VisitSmall_stmt(contextMock.Object),
                shouldVisit: innerContextMock.Object);
        }

        [TestMethod]
        public void SmallStmtVisit_DelStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Del_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Del_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_PassStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Pass_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Pass_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_FlowStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Flow_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Flow_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_ImportStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Import_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Import_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_GlobalStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Global_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Global_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_NonLocalStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Nonlocal_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Nonlocal_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_AssertStmt_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();
            var innerContextMock = GetMockRule<Python3Parser.Assert_stmtContext>();

            contextMock.Setup(o => o.GetRuleContext<Python3Parser.Assert_stmtContext>(0))
                .Returns(innerContextMock.Object);

            // TODO: Add this functionality
            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxNotYetImplementedException>(action);
        }

        [TestMethod]
        public void SmallStmtVisit_None_Test()
        {
            // Arrange
            var contextMock = GetMockRule<Python3Parser.Small_stmtContext>();

            Action action = delegate { ctor.VisitSmall_stmt(contextMock.Object); };

            // Act + Assert
            Assert.ThrowsException<SyntaxException>(action);
        }

        #endregion
    }
}