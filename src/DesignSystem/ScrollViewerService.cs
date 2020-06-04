using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using DesignSystem.Properties;

namespace DesignSystem
{
    public static class ScrollViewerService
    {
        public static readonly DependencyProperty ShowVerticalGradientProperty = DependencyProperty.RegisterAttached(
            "ShowVerticalGradient",
            typeof(bool),
            typeof(ScrollViewerService),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnShowVerticalGradientChanged));

        public static bool GetShowVerticalGradient(DependencyObject d)
        {
            return (bool) d.GetValue(ShowVerticalGradientProperty);
        }

        public static void SetShowVerticalGradient(DependencyObject d, bool value)
        {
            d.SetValue(ShowVerticalGradientProperty, value);
        }

        private static void OnShowVerticalGradientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer) d;
            scrollViewer.Loaded -= OnScrollViewerLoaded;

            if (GetShowVerticalGradient(d))
            {
                scrollViewer.Loaded += OnScrollViewerLoaded;
            }
        }

        private static void OnScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = (ScrollViewer) sender;

            var gradient = scrollViewer.FindChild<Rectangle>("PART_Gradient");
            if (gradient == null)
            {
                throw new InvalidOperationException(Resources.ScrollViewerService_OnScrollViewerLoaded_GradientNotFound);
            }

            var maxGradientHeight = gradient.MaxHeight;
            if (double.IsNaN(maxGradientHeight))
            {
                throw new InvalidOperationException(Resources.ScrollViewerService_OnScrollViewerLoaded_GradientMaxHeightNotSet);
            }

            SetGradientHeight(scrollViewer, gradient, maxGradientHeight);
            scrollViewer.ScrollChanged -= OnScrollViewerOnScrollChanged;
            scrollViewer.ScrollChanged += OnScrollViewerOnScrollChanged;
            void OnScrollViewerOnScrollChanged(object o, ScrollChangedEventArgs args) => SetGradientHeight(scrollViewer, gradient, maxGradientHeight);
        }

        private static void SetGradientHeight(ScrollViewer scrollViewer, Rectangle gradient, double maxGradientHeight)
        {
            var maxOffset = Math.Max(0L, scrollViewer.ExtentHeight - scrollViewer.ViewportHeight);
            var heightRatio = 1L - (scrollViewer.VerticalOffset / maxOffset);
            gradient.Height = maxGradientHeight * heightRatio;

            if (gradient.Height > 0.00001)
            {
                if (gradient.Visibility == Visibility.Hidden)
                {
                    gradient.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (gradient.Visibility != Visibility.Hidden)
                {
                    gradient.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}