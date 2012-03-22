﻿using System;
using Whathecode.System.Arithmetic;


namespace Whathecode.System.Windows.Data
{
	/// <summary>
	///   Convert one value to another by doing a calculation.
	/// </summary>
	/// <author>Steven Jeuris</author>
	public class CalculateValueConverter : AbstractGenericValueConverter<double, double>
	{
		/// <summary>
		///   The calculation to perform.
		/// </summary>
		public MathOperator Operation { get; set; }

		/// <summary>
		///   The second value to be used in the calculation. For the first value the original value is used. E.g. Original / Value
		/// </summary>
		public double Value { get; set; }


		public CalculateValueConverter()
		{
			Operation = MathOperator.Addition;
			Value = 0;
		}

		public CalculateValueConverter( MathOperator operation, double value )
		{
			Operation = operation;
			Value = value;
		}


		public override double Convert( double value )
		{
			switch ( Operation )
			{
				case MathOperator.Addition:
					return value + Value;
				case MathOperator.Division:
					return value / Value;
				case MathOperator.Multiplication:
					return value * Value;
				case MathOperator.Subtraction:
					return value - Value;
				default:
					throw CreateNotSupportedException();
			}
		}

		public override double ConvertBack( double value )
		{
			switch ( Operation )
			{
				case MathOperator.Addition:
					return value - Value;
				case MathOperator.Division:
					return value * Value;
				case MathOperator.Multiplication:
					return value / Value;
				case MathOperator.Subtraction:
					return value + Value;
				default:
					throw CreateNotSupportedException();
			}
		}

		Exception CreateNotSupportedException()
		{
			return new NotSupportedException( Operation + " is not supported by " + typeof( CalculateValueConverter ) + "." );;
		}
	}
}