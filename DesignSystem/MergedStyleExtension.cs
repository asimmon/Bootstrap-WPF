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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var mergedStyle = new Style();

            foreach (var style in _styles)
                mergedStyle.Merge(style);

            return mergedStyle;
        }
    }
}