using System;
using System.ComponentModel;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls
{
    public static class BindingFactory
    {
        /// <summary>
        /// This factory method was added to simplify creating TypedBindingBase instances from lambda getters.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="getter">An getter method used to retrieve the source property.</param>
        /// <param name="mode">The binding mode. This property is optional. Default is <see cref="F:Microsoft.Maui.Controls.BindingMode.Default" />.</param>
        /// <param name="converter">The converter. This parameter is optional. Default is <see langword="null" />.</param>
        /// <param name="converterParameter">An user-defined parameter to pass to the converter. This parameter is optional. Default is <see langword="null" />.</param>
        /// <param name="stringFormat">A String format. This parameter is optional. Default is <see langword="null" />.</param>
        /// <param name="source">An object used as the source for this binding. This parameter is optional. Default is <see langword="null" />.</param>
        /// <param name="fallbackValue">The value to use instead of the default value for the property, if no specified value exists.</param>
        /// <param name="targetNullValue">The value to supply for a bound property when the target of the binding is <see langword="null" />.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [EditorBrowsable(EditorBrowsableState.Never)] // TODO: remove the attribute once the source generator is enabled by default
        public static TypedBinding<TSource, TProperty> Create<TSource, TProperty>(
            Func<TSource, TProperty> getter,
            BindingMode mode = BindingMode.Default,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? stringFormat = null,
            object? source = null,
            object? fallbackValue = null,
            object? targetNullValue = null)
        {

            throw new InvalidOperationException($"Call to BindingFactory.Create<{typeof(TSource)}, {typeof(TProperty)}> was not intercepted.");
        }
    }
}