﻿using ErrorHandler;
using System.Collections;

namespace Compiler {

	public class ElseStatementParser{
		
		
		public static Logic parseElseStatement(Logic[] logicOrder, int lineNumber, Scope currentScope){
			
			if (logicOrder [logicOrder.Length - 1].currentType != WordTypes.indentOperator) 
				ErrorMessage.sendErrorMessage (lineNumber, ErrorType.ElseStatements, ElseErrorType.missingIndentOperator.ToString(), null);
			
			if (logicOrder.Length != 2) 
				ErrorMessage.sendErrorMessage (lineNumber, ErrorType.ElseStatements, ElseErrorType.unknownFormat.ToString(), null);
			
			
			return (logicOrder[0] as ElseStatement);
		}
		
	}
}