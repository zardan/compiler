﻿using System;
using ErrorHandler;
using System.Collections.Generic;
using Runtime;

namespace Compiler
{
	public class NumberSumParser{

		public static Variable validNumberSum(Logic[] logicOrder, int lineNumber){
			handleFirstAndLastWords(logicOrder, lineNumber);

			logicOrder = calcPow (logicOrder, lineNumber);
			logicOrder = calcModulo (logicOrder, lineNumber);

			var calcString = LogicToCalcString(logicOrder, out bool corrupt);

			if (!corrupt)
				try{
					double returnResult = SyntaxCheck.globalParser.Evaluate(calcString);
					return new Variable ("calcVarMaten", returnResult, true);
				}
				catch(Exception e){
					if (e is DivideByZeroException)
						ErrorMessage.sendErrorMessage(lineNumber, "Kan inte dividera med noll!");
					ErrorMessage.sendErrorMessage (lineNumber, "Något gick fel i uträkningen: " + e.ToString());
				}


			ErrorMessage.sendErrorMessage (lineNumber, "Något gick fel med nummertolkningen");
			return new Variable ();
		}

		#region pow "**"
		private static Logic[] calcPow(Logic[] logicOrder, int lineNumber){
			List<Logic> postPower = new List<Logic> ();

			for (int i = 0; i < logicOrder.Length; i++) {
				if (logicOrder [i].currentType == WordTypes.mathOperator && logicOrder [i].word == "*" && i < logicOrder.Length - 1) {
					if (logicOrder [i+1].currentType == WordTypes.mathOperator && logicOrder [i+1].word == "*") {
						postPower.Add (new MathOperator ("^"));
						i++;
					}
					else
						postPower.Add (logicOrder[i]);
				}
				else
					postPower.Add (logicOrder[i]);
			}

			return postPower.ToArray ();
		}
		#endregion


		#region Modulo
		private static Logic[] calcModulo(Logic[] logicOrder, int lineNumber){
			List<Logic> postModulo = new List<Logic> ();

			for (int i = 0; i < logicOrder.Length; i++) {
				
				if (logicOrder [i].currentType == WordTypes.mathOperator && logicOrder [i].word == "%") {

					if (i <= 0 || i >= logicOrder.Length - 1)
						ErrorMessage.sendErrorMessage (lineNumber, "Modulo operatorn måste appliceras på två tal");

					if(logicOrder[i-1].currentType != WordTypes.variable && logicOrder[i-1].currentType != WordTypes.number)
						// Will not be reached because there can't be a number and a notNumber at the same row
						ErrorMessage.sendErrorMessage (lineNumber, "Modulo operatorn måste appliceras på två tal");

					double firstValue = getNumberValue (logicOrder [i - 1]);
					double secondValue = getNumberValue (logicOrder [i + 1]);
					double result = firstValue % secondValue;

					postModulo.RemoveAt (postModulo.Count - 1);
					postModulo.Add (new NumberValue (result));
					i++;
				} 
				else
					postModulo.Add (logicOrder [i]);
			}

			return postModulo.ToArray ();
		}

		private static double getNumberValue(Logic l){
			if (l.currentType == WordTypes.variable)
				return (l as Variable).getNumber ();

			return (l as NumberValue).value;
		}
		#endregion

		private static void handleFirstAndLastWords(Logic[] logicOrder, int lineNumber){
			string firstWord = logicOrder [0].word;
			string lastWord = logicOrder [logicOrder.Length - 1].word;

			if (lastWord == "*" || lastWord == "/")
				ErrorMessage.sendErrorMessage (lineNumber, "Matematiska uttryck kan inte sluta med \"" + lastWord + "\"");
			if (firstWord == "*" || firstWord == "/")
				ErrorMessage.sendErrorMessage (lineNumber, "Matematiska uttryck kan inte börja med  \"" + firstWord + "\"");
			
		}

		private static string LogicToCalcString(Logic[] logicOrder, out bool corrupt)
		{
			string calcString = "";
			corrupt = false;

			for (int i = 0; i < logicOrder.Length; i++)
			{
				if (logicOrder[i].currentType == WordTypes.variable)
				{
					if (shouldAddParenthesis(logicOrder, i))
						calcString += "(" + (logicOrder[i] as Variable).getNumber() + ")";
					else
						calcString += (logicOrder[i] as Variable).getNumber().ToString();
				}
				else if (logicOrder[i].currentType == WordTypes.number)
				{
					if (shouldAddParenthesis(logicOrder, i))
					{
						calcString = calcString.Remove(calcString.Length - 1);
						calcString += "(-" + logicOrder[i].word + ")";
					}
					else
						calcString += logicOrder[i].word;
				}
				else if (logicOrder[i].currentType == WordTypes.mathOperator)
					calcString += logicOrder[i].word;
				else
					corrupt = true;
			}
			
			return calcString;
		}

		private static bool shouldAddParenthesis(Logic[] logicOrder, int index)
		{
			if (index > 2 && 
			    logicOrder[index - 1].word == "-" && 
			    logicOrder[index - 2].currentType != WordTypes.number &&
			    logicOrder[index - 2].currentType != WordTypes.variable)
				return true;

			if (logicOrder[index].currentType == WordTypes.variable &&
			    (logicOrder[index] as Variable).variableType == VariableTypes.number &&
				(logicOrder[index] as Variable).getNumber() < 0)
				return true;
			return false;
		}
	}

}

