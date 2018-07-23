using System;
using System.Globalization;
using System.Windows.Data;

namespace DesignSystem
{
    public class ArithmeticConverter : IValueConverter
    {
        public ArithmeticOperation Operation { get; set; }

        public ResultTransformation ResultTransform { get; set; }

        public ArithmeticConverter()
        {
            ResultTransform = ResultTransformation.None;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var leftOperand = TryCastToDouble(value);
            var rightOperand = TryCastToDouble(parameter);

            var result = ApplyOperation(leftOperand, rightOperand);
            return ApplyResultTransform(result);
        }

        private double ApplyOperation(double leftOperand, double rightOperand)
        {
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

        private double ApplyResultTransform(double value)
        {
            switch (ResultTransform)
            {
                case ResultTransformation.None:
                    return value;
                case ResultTransformation.Floor:
                    return Math.Floor(value);
                case ResultTransformation.Ceiling:
                    return Math.Ceiling(value);
                case ResultTransformation.Round:
                    return Math.Round(value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(ResultTransform));
            }
        }

        private static double TryCastToDouble(object value)
        {
            switch (value)
            {
                case string _ when double.TryParse((string)value, out var doubleValue):
                    return doubleValue;
                case double _:
                case float _:
                case int _:
                case decimal _:
                    return (double)value;
            }

            throw new ArgumentException($"Could not cast '{value}' to a double");
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

        public enum ResultTransformation
        {
            None,
            Floor,
            Ceiling,
            Round
        }
    }
}