using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace DesignSystem
{
    [ContentProperty("Styles")]
    [MarkupExtensionReturnType(typeof(Style))]
    public class MergedStyleExtension : MarkupExtension
    {
        private readonly IList<Style> _styles = new List<Style>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IList Styles
        {
            get { return (IList)this._styles; }
        }

        public Style Style1 { get; set; }
        public Style Style2 { get; set; }
        public Style Style3 { get; set; }
        public Style Style4 { get; set; }
        public Style Style5 { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            AddStyleIfNotNull(Style1);
            AddStyleIfNotNull(Style2);
            AddStyleIfNotNull(Style3);
            AddStyleIfNotNull(Style4);
            AddStyleIfNotNull(Style5);

            return MergeStyles();
        }

        private void AddStyleIfNotNull(Style style)
        {
            if (style != null)
                _styles.Add(style);
        }

        private object MergeStyles()
        {
            var mergedStyle = new Style();

            foreach (var style in _styles)
                mergedStyle.Merge(style);

            return mergedStyle;
        }
    }
}