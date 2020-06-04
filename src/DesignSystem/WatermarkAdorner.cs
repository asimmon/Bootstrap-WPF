using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace DesignSystem
{
    internal class WatermarkAdorner : Adorner
    {
        public static readonly DependencyProperty IsOverflowingAdornedElementProperty = DependencyProperty.Register(
            "IsOverflowingAdornedElement",
            typeof(bool),
            typeof(WatermarkAdorner),
            new PropertyMetadata(default(bool)));

        private readonly ContentPresenter _contentPresenter;

        public WatermarkAdorner(UIElement adornedElement, object watermark, DataTemplate watermarkTemplate)
            : base(adornedElement)
        {
            IsHitTestVisible = false;

            var template = watermarkTemplate ??
                           Control.TryFindResource("WatermarkPresenter_" + adornedElement.GetType().Name) as DataTemplate ??
                           Control.TryFindResource("WatermarkPresenter") as DataTemplate;

            if (template != null)
            {
                _contentPresenter = (ContentPresenter)template.LoadContent();
            }
            else
            {
                _contentPresenter = new ContentPresenter();
            }

            // set the content
            _contentPresenter.Content = watermark;

            if (Control is ItemsControl && !(Control is ComboBox))
            {
                _contentPresenter.VerticalAlignment = VerticalAlignment.Center;
                _contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
            }

            var binding = new Binding(IsVisibleProperty.Name);
            binding.Source = adornedElement;
            binding.Converter = new BooleanToVisibilityConverter();
            SetBinding(VisibilityProperty, binding);
        }

        public bool IsOverflowingAdornedElement
        {
            get { return (bool)GetValue(IsOverflowingAdornedElementProperty); }
            set { SetValue(IsOverflowingAdornedElementProperty, value); }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        private Control Control
        {
            get { return (Control) AdornedElement; }
        }

        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="FrameworkElement"/> derived class.
        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// Returns a specified child <see cref="Visual"/> for the parent <see cref="ContainerVisual"/>.
        protected override Visual GetVisualChild(int index)
        {
            return _contentPresenter;
        }

        /// Implements any custom measuring behavior for the adorner.
        protected override Size MeasureOverride(Size constraint)
        {
            // First, we calculate the actual desired size of the watermark content within the adorner
            _contentPresenter.Measure(constraint);

            var desiredSize = _contentPresenter.DesiredSize;
            var actualSize = Control.RenderSize;

            // Here's the secret to getting the adorner to cover the whole control
            _contentPresenter.Measure(Control.RenderSize);

            // To check if the watermark overflows the control, we compare its width.
            IsOverflowingAdornedElement = desiredSize.Width > actualSize.Width;
            return actualSize;
        }
    }
}