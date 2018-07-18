using System;
using System.Globalization;
using System.Windows.Data;

namespace DesignSystem
{
    public class ArithmeticConverter : IValueConverter
    {
        public ArithmeticOperation Operation { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var leftOperand = TryCastToDouble(value);
            var rightOperand = TryCastToDouble(parameter);

            switch (Operation)
            {
                case ArithmeticOperation.Addition:
                    return leftOperand + rightOperand;
                case ArithmeticOperation.Substraction:
                    return leftOperand - rightOperand;
                case ArithmeticOperation.Multiplication:
                    return leftOperand * rightOperand;
                case ArithmeticOperation.Division:
                    return leftOperand / rightOperand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Operation));
            }
        }

        private double TryCastToDouble(object value)
        {
            if (value is string && double.TryParse((string)value, out var doubleValue))
                return doubleValue;

            if (value is int || value is float || value is double)
                return (double)value;

            throw new ArgumentException($"Could not cast {value} as a double");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public enum ArithmeticOperation
        {
            Addition,
            Substraction,
            Multiplication,
            Division
        }
    }
}