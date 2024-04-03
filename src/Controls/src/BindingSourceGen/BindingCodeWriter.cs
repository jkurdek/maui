using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.Maui.Controls.BindingSourceGen;

public sealed class BindingCodeWriter
{
	public static string GeneratedCodeAttribute => $"[GeneratedCodeAttribute(\"{typeof(BindingCodeWriter).Assembly.FullName}\", \"{typeof(BindingCodeWriter).Assembly.GetName().Version}\")]";

	public string GenerateCode() => $$"""
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
		
			{{GeneratedCodeAttribute}}
			[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
			file sealed class InterceptsLocationAttribute(string filePath, int line, int column) : Attribute
			{
			}
		}

		namespace Microsoft.Maui.Controls.Generated
		{
			using System;
			using System.CodeDom.Compiler;
			using System.Runtime.CompilerServices;
			using Microsoft.Maui.Controls.Internal;

			{{GeneratedCodeAttribute}}
			file static class GeneratedBindableObjectExtensions
			{
				{{GenerateBindingMethods(indent: 2)}}
			}
		}
		""";

	private readonly List<CodeWriterBinding> _bindings = new();

	public void AddBinding(CodeWriterBinding binding)
	{
		_bindings.Add(binding);
	}

	private string GenerateBindingMethods(int indent)
	{
		using var builder = new BidningInterceptorCodeBuilder(indent);

		for (int i = 0; i < _bindings.Count; i++)
		{
			builder.AppendSetBindingInterceptor(id: i + 1, _bindings[i]);
		}

		return builder.ToString();
	}

	public sealed class BidningInterceptorCodeBuilder : IDisposable
	{
		private StringWriter _stringWriter;
		private IndentedTextWriter _indentedTextWriter;

		public override string ToString()
		{
			_indentedTextWriter.Flush();
			return _stringWriter.ToString();
		}

		public BidningInterceptorCodeBuilder(int indent = 0)
		{
			_stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			_indentedTextWriter = new IndentedTextWriter(_stringWriter, "\t") { Indent = indent };
		}

		public void AppendSetBindingInterceptor(int id, CodeWriterBinding binding)
		{
			AppendBlankLine();

			AppendLine(GeneratedCodeAttribute);
			AppendInterceptorAttribute(binding.Location);
			Append($"public static void SetBinding{id}");
			if (binding.SourceType.IsGenericParameter && binding.PropertyType.IsGenericParameter)
			{
				Append($"<{binding.SourceType}, {binding.PropertyType}>");
			}
			else if (binding.SourceType.IsGenericParameter)
			{
				Append($"<{binding.SourceType}>");
			}
			else if (binding.PropertyType.IsGenericParameter)
			{
				Append($"<{binding.PropertyType}>");
			}
			AppendLine('(');

			AppendLines($$"""
					this BindableObject bindableObject,
					BindableProperty bidnableProperty,
					Func<{{binding.SourceType}}, {{binding.PropertyType}}> getter,
					BindingMode mode = BindingMode.Default,
					IValueConverter? converter = null,
					object? converterParameter = null,
					string? stringFormat = null,
					object? source = null,
					object? fallbackValue = null,
					object? targetNullValue = null)
				{
					var binding = new TypedBinding<{{binding.SourceType}}, {{binding.PropertyType}}>(
						getter: static source => (getter(source), true),
				""");

			Indent();
			Indent();

			Append("setter: ");
			if (binding.GenerateSetter)
			{
				AppendSetterAction(binding.Path);
			}
			else
			{
				Append("null");
			}
			AppendLine(',');

			Append("handlers: ");
			AppendHandlersArray(binding.SourceType, binding.Path);
			AppendLine(")");

			Unindent();
			Unindent();

			AppendLines($$"""
					{
						Mode = mode,
						Converter = converter,
						ConverterParameter = converterParameter,
						StringFormat = stringFormat,
						Source = source,
						FallbackValue = fallbackValue,
						TargetNullValue = targetNullValue
					};
				
					bindableObject.SetBinding(bidnableProperty, binding);
				}
				""");
		}

		private void AppendInterceptorAttribute(SourceCodeLocation location)
		{
			AppendLine($"[InterceptsLocationAttribute(@\"{location.FilePath}\", {location.Line}, {location.Column})]");
		}

		private void AppendSetterAction(IPathPart[] path)
		{
			AppendLine("static (source, value) => ");
			AppendLine('{');
			Indent();

			bool anyPartIsNullable = false;
			foreach (var part in path)
			{
				anyPartIsNullable |= part.IsNullable;
			}

			if (anyPartIsNullable)
			{
				Append("if (source");
				AppendPathAccess(path, path.Length - 1);
				AppendLine(" is null)");
				AppendLines(
					"""
					{
					    return;
					}

					""");
			}

			Append("source");
			foreach (var part in path)
			{
				Append(part.PartGetter(withNullableAnnotation: false));
			}

			AppendLine(" = value;");

			Unindent();
			Append('}');
		}

		private void AppendHandlersArray(TypeName sourceType, IPathPart[] path)
		{
			AppendLine($"new Tuple<Func<{sourceType}, object?>, string>[]");
			AppendLine('{');

			Indent();
			for (int i = 0; i < path.Length; i++)
			{
				Append("new(static source => source");
				AppendPathAccess(path, depth: i);
				AppendLine($", \"{path[i].PropertyName}\"),");
			}
			Unindent();

			Append('}');
		}

		private void AppendPathAccess(IPathPart[] path, int depth)
		{
			Debug.Assert(depth >= 0, "Depth must be greater than 0");
			Debug.Assert(depth <= path.Length, "Depth must be less than path length");

			if (depth == 0)
			{
				return;
			}

			for (int i = 0; i < depth - 1; i++)
			{
				Append(path[i].PartGetter(withNullableAnnotation: true));
			}

			Append(path[depth - 1].PartGetter(withNullableAnnotation: false));
		}

		public void Dispose()
		{
			_indentedTextWriter.Dispose();
			_stringWriter.Dispose();
		}

		private void AppendBlankLine() => _indentedTextWriter.WriteLine();
		private void AppendLine(string line) => _indentedTextWriter.WriteLine(line);
		private void AppendLine(char character) => _indentedTextWriter.WriteLine(character);
		private void Append(string str) => _indentedTextWriter.Write(str);
		private void Append(char character) => _indentedTextWriter.Write(character);
		private void AppendLines(string lines)
		{
			foreach (var line in lines.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
			{
				AppendLine(line.TrimEnd('\r'));
			}
		}

		private void Indent() => _indentedTextWriter.Indent++;
		private void Unindent() => _indentedTextWriter.Indent--;
	}
}