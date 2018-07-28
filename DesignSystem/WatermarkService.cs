using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DesignSystem
{
    public static class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
           "Watermark",
           typeof(object),
           typeof(WatermarkService),
           new FrameworkPropertyMetadata(null, OnWatermarkChanged));

        public static readonly DependencyProperty WatermarkTargetProperty = DependencyProperty.RegisterAttached(
           "WatermarkTarget",
           typeof(UIElement),
           typeof(WatermarkService),
           new FrameworkPropertyMetadata(null, OnWatermarkTargetChanged));

        public static readonly DependencyProperty WatermarkDataTemplateProperty = DependencyProperty.RegisterAttached(
           "WatermarkDataTemplate",
           typeof(DataTemplate),
           typeof(WatermarkService),
           new FrameworkPropertyMetadata((object)null, null));

        public static void OnWatermarkedControlChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;

            if (ShouldShowWatermark(control))
            {
                ShowWatermark(control, GetWatermarkTarget(control) ?? control);
            }
            else
            {
                RemoveWatermark(GetWatermarkTarget(control) ?? control);
            }
        }

        public static object GetWatermark(DependencyObject d)
        {
            return d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject d, object value)
        {
            d.SetValue(WatermarkProperty, value);
        }

        public static DataTemplate GetWatermarkDataTemplate(DependencyObject d)
        {
            return (DataTemplate)d.GetValue(WatermarkDataTemplateProperty);
        }

        public static void SetWatermarkDataTemplate(DependencyObject d, DataTemplate value)
        {
            d.SetValue(WatermarkDataTemplateProperty, value);
        }

        public static UIElement GetWatermarkTarget(DependencyObject d)
        {
            return (UIElement)d.GetValue(WatermarkTargetProperty);
        }

        public static void SetWatermarkTarget(DependencyObject d, UIElement value)
        {
            d.SetValue(WatermarkTargetProperty, value);
        }

        public static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.Loaded += OnWatermarkedControlChanged;

            PasswordBox passwordBox;
            TextBoxBase textBox;

            if ((passwordBox = d as PasswordBox) != null)
            {
                passwordBox.PasswordChanged += OnWatermarkedControlChanged;
            }
            else if ((textBox = d as TextBoxBase) != null)
            {
                textBox.TextChanged += OnWatermarkedControlChanged;
            }
        }

        private static void OnWatermarkTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RemoveWatermark((e.OldValue as UIElement) ?? (Control)d);
            OnWatermarkedControlChanged(d, EventArgs.Empty);
        }

        private static void RemoveWatermark(UIElement control)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);
            if (layer != null)
            {
                RemoveAllWatermarksFromLayer(control, layer);
            }
        }

        private static void ShowWatermark(UIElement control, UIElement adornedControl)
        {
            var layer = AdornerLayer.GetAdornerLayer(adornedControl);
            if (layer != null)
            {
                RemoveAllWatermarksFromLayer(adornedControl, layer);

                layer.Add(new WatermarkAdorner(adornedControl, GetWatermark(control), GetWatermarkDataTemplate(control)));
            }
        }

        private static void RemoveAllWatermarksFromLayer(UIElement control, AdornerLayer layer)
        {
            var adorners = layer.GetAdorners(control);
            if (adorners != null && adorners.Length > 0)
            {
                foreach (var adorner in adorners.OfType<WatermarkAdorner>())
                {
                    adorner.Visibility = Visibility.Hidden;
                    layer.Remove(adorner);
                }
            }
        }

        private static bool ShouldShowWatermark(Control c)
        {
            PasswordBox passwordBox;
            TextBox textBox;

            if ((passwordBox = c as PasswordBox) != null)
            {
                return string.IsNullOrEmpty(passwordBox.Password);
            }
            else if ((textBox = c as TextBox) != null)
            {
                return string.IsNullOrEmpty(textBox.Text);
            }
            else
            {
                return false;
            }
        }
    }
}
