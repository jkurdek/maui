using Microsoft.Maui.Controls.BindingSourceGen;
using Xunit;

namespace BindingSourceGen.UnitTests;
public class IntegrationTests
{
    [Fact]
    public void GenerateSimpleBinding()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (string s) => s.Length);
        """;

        var result = SourceGenHelpers.Run(source);
        AssertExtensions.AssertNoDiagnostics(result);
        AssertExtensions.CodeIsEqual(
            $$"""
            //------------------------------------------------------------------------------
            // <auto-generated>
            //     This code was generated by a .NET MAUI source generator.
            //
            //     Changes to this file may cause incorrect behavior and will be lost if
            //     the code is regenerated.
            // </auto-generated>
            //------------------------------------------------------------------------------
            #nullable enable

            namespace System.Runtime.CompilerServices
            {
                using System;
                using System.CodeDom.Compiler;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                file sealed class InterceptsLocationAttribute : Attribute
                {
                    public InterceptsLocationAttribute(string filePath, int line, int column)
                    {
                        FilePath = filePath;
                        Line = line;
                        Column = column;
                    }
            
                    public string FilePath { get; }
                    public int Line { get; }
                    public int Column { get; }
                }
            }

            namespace Microsoft.Maui.Controls.Generated
            {
                using System;
                using System.CodeDom.Compiler;
                using System.Runtime.CompilerServices;
                using Microsoft.Maui.Controls.Internals;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                file static class GeneratedBindableObjectExtensions
                {
            
                    {{BindingCodeWriter.GeneratedCodeAttribute}}
                    [InterceptsLocationAttribute(@"Path\To\Program.cs", 3, 7)]
                    public static void SetBinding1(
                        this BindableObject bindableObject,
                        BindableProperty bindableProperty,
                        Func<string, int> getter,
                        BindingMode mode = BindingMode.Default,
                        IValueConverter? converter = null,
                        object? converterParameter = null,
                        string? stringFormat = null,
                        object? source = null,
                        object? fallbackValue = null,
                        object? targetNullValue = null)
                    {
                        Action<string, int>? setter = null;
                        if (ShouldUseSetter(mode, bindableProperty))
                        {
                            throw new InvalidOperationException("Cannot set value on the source object.");
                        }

                        var binding = new TypedBinding<string, int>(
                            getter: source => (getter(source), true),
                            setter,
                            handlers: new Tuple<Func<string, object?>, string>[]
                            {
                                new(static source => source, "Length"),
                            })
                        {
                            Mode = mode,
                            Converter = converter,
                            ConverterParameter = converterParameter,
                            StringFormat = stringFormat,
                            Source = source,
                            FallbackValue = fallbackValue,
                            TargetNullValue = targetNullValue
                        };
                        bindableObject.SetBinding(bindableProperty, binding);
                    }

                    private static bool ShouldUseSetter(BindingMode mode, BindableProperty bindableProperty)
                        => mode == BindingMode.OneWayToSource
                            || mode == BindingMode.TwoWay
                            || (mode == BindingMode.Default
                                && (bindableProperty.DefaultBindingMode == BindingMode.OneWayToSource
                                    || bindableProperty.DefaultBindingMode == BindingMode.TwoWay));
                }
            }
            """,
            result.GeneratedCode);
    }

    [Fact]
    public void CorrectlyFormatsBindingWithCasts()
    {
        var source = """
            using Microsoft.Maui.Controls;
            using MyNamespace;
            var label = new Label();
            label.SetBinding(Label.TextProperty, static (MySourceClass s) => (((s.A as X)?.B as Y)?.C as Z)?.D);

            namespace MyNamespace
            {
                public class MySourceClass
                {
                    public object? A { get; set; }
                }

                public class X
                {
                    public object? B { get; set; }
                }

                public class Y
                {
                    public object C { get; set; } = null!;
                }

                public class Z
                {
                    public MyPropertyClass D { get; set; } = null!;
                }

                public class MyPropertyClass
                {
                }
            }
            """;

        var result = SourceGenHelpers.Run(source);
        
        AssertExtensions.AssertNoDiagnostics(result);
        AssertExtensions.CodeIsEqual(
            $$"""
            //------------------------------------------------------------------------------
            // <auto-generated>
            //     This code was generated by a .NET MAUI source generator.
            //
            //     Changes to this file may cause incorrect behavior and will be lost if
            //     the code is regenerated.
            // </auto-generated>
            //------------------------------------------------------------------------------
            #nullable enable

            namespace System.Runtime.CompilerServices
            {
                using System;
                using System.CodeDom.Compiler;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                file sealed class InterceptsLocationAttribute : Attribute
                {
                    public InterceptsLocationAttribute(string filePath, int line, int column)
                    {
                        FilePath = filePath;
                        Line = line;
                        Column = column;
                    }
            
                    public string FilePath { get; }
                    public int Line { get; }
                    public int Column { get; }
                }
            }

            namespace Microsoft.Maui.Controls.Generated
            {
                using System;
                using System.CodeDom.Compiler;
                using System.Runtime.CompilerServices;
                using Microsoft.Maui.Controls.Internals;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                file static class GeneratedBindableObjectExtensions
                {

                    {{BindingCodeWriter.GeneratedCodeAttribute}}
                    [InterceptsLocationAttribute(@"Path\To\Program.cs", 4, 7)]
                    public static void SetBinding1(
                        this BindableObject bindableObject,
                        BindableProperty bindableProperty,
                        Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass?> getter,
                        BindingMode mode = BindingMode.Default,
                        IValueConverter? converter = null,
                        object? converterParameter = null,
                        string? stringFormat = null,
                        object? source = null,
                        object? fallbackValue = null,
                        object? targetNullValue = null)
                    {
                        Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass?>? setter = null;
                        if (ShouldUseSetter(mode, bindableProperty))
                        {
                            setter = static (source, value) =>
                            {
                                if (value is null)
                                {
                                    return;
                                }
                                if (source.A is global::MyNamespace.X p0
                                    && p0.B is global::MyNamespace.Y p1
                                    && p1.C is global::MyNamespace.Z p2)
                                {
                                    p2.D = value;
                                }
                            };
                        }

                        var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass?>(
                            getter: source => (getter(source), true),
                            setter,
                            handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                            {
                                new(static source => source, "A"),
                                new(static source => (source.A as global::MyNamespace.X), "B"),
                                new(static source => ((source.A as global::MyNamespace.X)?.B as global::MyNamespace.Y), "C"),
                                new(static source => (((source.A as global::MyNamespace.X)?.B as global::MyNamespace.Y)?.C as global::MyNamespace.Z), "D"),
                            })
                        {
                            Mode = mode,
                            Converter = converter,
                            ConverterParameter = converterParameter,
                            StringFormat = stringFormat,
                            Source = source,
                            FallbackValue = fallbackValue,
                            TargetNullValue = targetNullValue
                        };

                        bindableObject.SetBinding(bindableProperty, binding);
                    }

                    private static bool ShouldUseSetter(BindingMode mode, BindableProperty bindableProperty)
                        => mode == BindingMode.OneWayToSource
                            || mode == BindingMode.TwoWay
                            || (mode == BindingMode.Default
                                && (bindableProperty.DefaultBindingMode == BindingMode.OneWayToSource
                                    || bindableProperty.DefaultBindingMode == BindingMode.TwoWay));
                }
            }
            """,
            result.GeneratedCode);
    }
}