#nullable enable
using System;
using System.Text.RegularExpressions;
using OS.Devices;

namespace Core.Scripting
{
	public class ShellTester
	{
		private readonly string[] expressions;
		private readonly ITextConsole console;

		public ShellTester(string[] expressions, ITextConsole console)
		{
			this.expressions = expressions;
			this.console = console;
		}
		
		public bool Test()
		{
			// No arguments means false.
			if (this.expressions.Length < 1)
				return false;
			
			// Too many arguments is an error.
			if (expressions.Length > 4)
			{
				console.WriteText($"-sh: test: too many arguments{Environment.NewLine}");
				return false;
			}

			switch (expressions.Length)
			{
				case 1:
					return TestSingleExpression(expressions[0]) == 0;
				case 2:
					return TestUnaryOperation(expressions[0], expressions[1]);
				case 3:
					return TestBinaryOperation(expressions[0], expressions[1], expressions[2]);
			}
			
			return false;
		}

		private int TestSingleExpression(string expr)
		{
			if (int.TryParse(expr, out int numeric))
				return numeric;

			if (string.IsNullOrEmpty(expr))
				return 1;

			if (expr == "false")
				return 1;

			return expr.Length > 0 ? 0 : 1;
		}

		private bool TestUnaryOperation(string op, string expr)
		{
			switch (op)
			{
				case "-z":
				{
					return expr.Length == 0;
				}
				case "-n":
				{
					return expr.Length > 0;
				}
				case "!":
				{
					int result = TestSingleExpression(expr);
					return result != 0;
				}
				// TODO: File tests
				default:
					return false;
			}
		}

		private bool TestBinaryOperation(string expr1, string op, string expr2)
		{
			switch (op)
			{
				case "-a":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					// If either a or be evaluate to 0, then multiplying them will result in a 0 as well.
					return a * b != 0;
				}
				case "-o":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					// this cursed shit will evaluate to 1 if a or b have a non-zero value
					// because I know what the fuck I'm doing with bitwise operators and don't
					// feel like doing multiple ifs or ternaries
					//
					// if you don't like this, get the fuck outta my codebase <3
					return (a | b) != 0;
				}
				case "=":
					return expr1 == expr2;
				case "!=":
					return expr1 != expr2;
				case "-eq":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					return a == b;
				}
				case "-ge":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					return a >= b;
				}
				case "-le":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					return a <= b;
				}
				case "-gt":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					return a > b;
				}
				case "-lt":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					return a < b;
				}
				case "-ne":
				{
					int a = TestSingleExpression(expr1);
					int b = TestSingleExpression(expr2);

					return a != b;
				}
				case "=~":
				{
					return Regex.IsMatch(expr1, expr2);
				}
				default:
					return false;
			}
		}
	}
}