﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zifro.Compiler.Core.Exceptions;
using Zifro.Compiler.Lang.Python3.Grammar;
using Zifro.Compiler.Lang.Python3.Resources;
using Zifro.Compiler.Lang.Python3.Syntax;
using Zifro.Compiler.Lang.Python3.Syntax.Statements;

namespace Zifro.Compiler.Lang.Python3.Tests.SyntaxConstructor.CompoundTree
{
    [TestClass]
    public class VisitIfStmtTests : BaseVisitClass<Python3Parser.If_stmtContext>
    {
        public override SyntaxNode VisitContext()
        {
            return ctor.VisitIf_stmt(contextMock.Object);
        }

        private void CreateAndSetupTest(out Mock<Python3Parser.TestContext> testMock, out ExpressionNode testExpr)
        {
            testMock = GetMockRule<Python3Parser.TestContext>();
            Mock<Python3Parser.TestContext> refCopy = testMock;
            testExpr = ctorMock.SetupExpressionMock(o => o.VisitTest(refCopy.Object));
        }

        private void CreateAndSetupSuite(out Mock<Python3Parser.SuiteContext> suiteMock, out Statement suiteStmt)
        {
            suiteMock = GetMockRule<Python3Parser.SuiteContext>();
            Mock<Python3Parser.SuiteContext> refCopy = suiteMock;
            suiteStmt = ctorMock.SetupStatementMock(o => o.VisitSuite(refCopy.Object));
        }

        [TestMethod]
        public void Visit_If_Test()
        {
            // Arrange
            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> testMock,
                out ExpressionNode testExpr);
            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> suiteMock, 
                out Statement suiteStmt);

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            /* Expected tree:
             *
             * IF
             * : test: testExpr
             * : suite: suiteStmt
             * : else: null
             */
            Assert.IsInstanceOfType(result, typeof(IfStatement));
            var ifStmt = (IfStatement)result;
            Assert.AreSame(testExpr, ifStmt.Condition, "if condition did not match");
            Assert.That.IsStatementListContaining(ifStmt.ElseSuite, suiteStmt);
            Assert.IsNull(ifStmt.ElseSuite, "ifStmt.ElseSuite was not null");

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Visit_If_Else_Test()
        {
            // Arrange
            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> testMock,
                out ExpressionNode testExpr);
            var suiteMock = GetMockRule<Python3Parser.SuiteContext>();
            var elseMock = GetMockRule<Python3Parser.SuiteContext>();

            var suiteStmt = ctorMock.SetupStatementMock(o => o.VisitSuite(suiteMock.Object));
            var elseStmt = ctorMock.SetupStatementMock(o => o.VisitSuite(elseMock.Object));

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object,
                GetTerminal(Python3Parser.ELSE),
                GetTerminal(Python3Parser.COLON),
                elseMock.Object
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            /* Expected tree:
             *
             * IF
             * : test: testExpr
             * : suite: suiteStmt
             * : else: elseStmt
             */
            Assert.IsInstanceOfType(result, typeof(IfStatement));
            var ifStmt = (IfStatement)result;
            Assert.AreSame(testExpr, ifStmt.Condition, "if condition did not match");
            Assert.That.IsStatementListContaining(ifStmt.IfSuite, suiteStmt);
            Assert.That.IsStatementListContaining(ifStmt.ElseSuite, elseStmt);

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Visit_If_ElIf_Test()
        {
            // Arrange
            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> testMock,
                out ExpressionNode testExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> suiteMock,
                out Statement suiteStmt);

            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> elifTestMock,
                out ExpressionNode elifTestExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> elifSuiteMock,
                out Statement elifStmt);

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object,
                GetTerminal(Python3Parser.ELIF),
                elifTestMock.Object,
                GetTerminal(Python3Parser.COLON),
                elifSuiteMock.Object
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            /* Expected tree:
             *
             * IF
             * : test: testExpr
             * : suite: suiteStmt
             * : else:
             *      IF
             *      : test: elifTestExpr
             *      : suite: elifStmt
             *      : else: null
             */
            Assert.IsInstanceOfType(result, typeof(IfStatement));
            var ifStmt = (IfStatement)result;
            Assert.AreSame(testExpr, ifStmt.Condition, "if condition did not match");
            Assert.That.IsStatementListContaining(ifStmt.IfSuite, suiteStmt);
            Assert.That.IsStatementListWithCount(1, ifStmt.ElseSuite);

            var innerIf = (IfStatement) ifStmt.ElseSuite.Statements[0];
            Assert.AreSame(elifTestExpr, innerIf.Condition);
            Assert.That.IsStatementListContaining(innerIf.IfSuite, elifStmt);
            Assert.IsNull(innerIf.ElseSuite);

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Visit_If_ElIf_Else_Test()
        {
            // Arrange
            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> testMock,
                out ExpressionNode testExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> suiteMock,
                out Statement suiteStmt);

            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> elifTestMock,
                out ExpressionNode elifTestExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> elifSuiteMock,
                out Statement elifStmt);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> elseSuiteMock,
                out Statement elseStmt);

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object,

                GetTerminal(Python3Parser.ELIF),
                elifTestMock.Object,
                GetTerminal(Python3Parser.COLON),
                elifSuiteMock.Object,

                GetTerminal(Python3Parser.ELSE),
                GetTerminal(Python3Parser.COLON),
                elseSuiteMock.Object
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            /* Expected tree:
             *
             * IF
             * : test: testExpr
             * : suite: suiteStmt
             * : else:
             *      IF
             *      : test: elifTestExpr
             *      : suite: elifStmt
             *      : else: elseStmt
             */
            Assert.IsInstanceOfType(result, typeof(IfStatement));
            var ifStmt = (IfStatement)result;
            Assert.AreSame(testExpr, ifStmt.Condition, "if condition did not match");
            Assert.That.IsStatementListContaining(ifStmt.IfSuite, suiteStmt);
            Assert.That.IsStatementListWithCount(1, ifStmt.ElseSuite);

            var innerIf = (IfStatement)ifStmt.ElseSuite.Statements[0];
            Assert.AreSame(elifTestExpr, innerIf.Condition);
            Assert.That.IsStatementListContaining(innerIf.IfSuite, elifStmt);
            Assert.IsNull(innerIf.ElseSuite);

            Assert.That.IsStatementListContaining(ifStmt.ElseSuite, elseStmt);

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Visit_If_2ElIf_Else_Test()
        {
            // Arrange
            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> testMock,
                out ExpressionNode testExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> suiteMock,
                out Statement suiteStmt);

            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> elif1TestMock,
                out ExpressionNode elif1TestExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> elif1SuiteMock,
                out Statement elif1Stmt);

            CreateAndSetupTest(
                out Mock<Python3Parser.TestContext> elif2TestMock,
                out ExpressionNode elif2TestExpr);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> elif2SuiteMock,
                out Statement elif2Stmt);

            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> elseSuiteMock,
                out Statement elseStmt);

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object,

                GetTerminal(Python3Parser.ELIF),
                elif1TestMock.Object,
                GetTerminal(Python3Parser.COLON),
                elif1SuiteMock.Object,

                GetTerminal(Python3Parser.ELIF),
                elif2TestMock.Object,
                GetTerminal(Python3Parser.COLON),
                elif2SuiteMock.Object,

                GetTerminal(Python3Parser.ELSE),
                GetTerminal(Python3Parser.COLON),
                elseSuiteMock.Object
            );

            // Act
            SyntaxNode result = VisitContext();

            // Assert
            /* Expected tree:
             *
             * IF
             * : test: testExpr
             * : suite: suiteStmt
             * : else:
             *      IF
             *      : test: elif1TestExpr
             *      : suite: elif1Stmt
             *      : else:
             *      IF
             *          : test: elif2TestExpr
             *          : suite: elif2Stmt
             *          : else: elseStmt
             */

            Assert.IsInstanceOfType(result, typeof(IfStatement));
            var ifStmt = (IfStatement)result;
            Assert.AreSame(testExpr, ifStmt.Condition, "if condition did not match");
            Assert.That.IsStatementListContaining(ifStmt.IfSuite, suiteStmt);
            Assert.That.IsStatementListWithCount(1, ifStmt.ElseSuite);

            var innerIf1 = (IfStatement)ifStmt.ElseSuite.Statements[0];
            Assert.AreSame(elif1TestExpr, innerIf1.Condition);
            Assert.That.IsStatementListContaining(innerIf1.IfSuite, elif1Stmt);
            Assert.That.IsStatementListWithCount(1, innerIf1.ElseSuite);

            var innerIf2 = (IfStatement)innerIf1.ElseSuite.Statements[0];
            Assert.AreSame(elif2TestExpr, innerIf2.Condition);
            Assert.That.IsStatementListContaining(innerIf2.IfSuite, elif2Stmt);
            Assert.IsNull(innerIf2.ElseSuite);

            Assert.That.IsStatementListContaining(ifStmt.ElseSuite, elseStmt);

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Invalid_If_MissingColon_Test()
        {
            // Arrange
            var testMock = GetMockRule<Python3Parser.TestContext>();
            var suiteMock = GetMockRule<Python3Parser.SuiteContext>();

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetMissingTerminal(Python3Parser.COLON),
                suiteMock.Object
            );

            // Act
            var ex = Assert.ThrowsException<SyntaxException>(VisitContext);

            // Assert
            Assert.That.ErrorFormatArgsEqual(ex,
                nameof(Localized_Python3_Parser.Ex_Syntax_If_MissingColon));

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Invalid_If_Else_MissingColon_Test()
        {
            // Arrange
            var testMock = GetMockRule<Python3Parser.TestContext>();
            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> suiteMock,
                out Statement _);

            var elseSuiteMock = GetMockRule<Python3Parser.SuiteContext>();

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object,
                GetTerminal(Python3Parser.ELSE),
                GetMissingTerminal(Python3Parser.COLON),
                elseSuiteMock.Object
            );

            // Act
            var ex = Assert.ThrowsException<SyntaxException>(VisitContext);

            // Assert
            Assert.That.ErrorFormatArgsEqual(ex,
                nameof(Localized_Python3_Parser.Ex_Syntax_If_Else_MissingColon));

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }

        [TestMethod]
        public void Invalid_If_Elif_MissingColon_Test()
        {
            // Arrange
            var testMock = GetMockRule<Python3Parser.TestContext>();
            CreateAndSetupSuite(
                out Mock<Python3Parser.SuiteContext> suiteMock,
                out Statement _);

            var elifTestMock = GetMockRule<Python3Parser.TestContext>();
            var elifSuiteMock = GetMockRule<Python3Parser.SuiteContext>();

            contextMock.SetupChildren(
                GetTerminal(Python3Parser.IF),
                testMock.Object,
                GetTerminal(Python3Parser.COLON),
                suiteMock.Object,
                GetTerminal(Python3Parser.ELIF),
                elifTestMock.Object,
                GetMissingTerminal(Python3Parser.COLON),
                elifSuiteMock.Object
            );

            // Act
            var ex = Assert.ThrowsException<SyntaxException>(VisitContext);

            // Assert
            Assert.That.ErrorFormatArgsEqual(ex,
                nameof(Localized_Python3_Parser.Ex_Syntax_If_Elif_MissingColon));

            testMock.Verify();
            suiteMock.Verify();
            contextMock.Verify();
            ctorMock.Verify();
        }
    }
}