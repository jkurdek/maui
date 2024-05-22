using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Microsoft.Maui.Controls.BindingSourceGen;

public static class BindingGenerationUtilities
{
    internal static bool IsTypeNullable(ITypeSymbol typeInfo, bool enabledNullable)
    {
        if (!enabledNullable && typeInfo.IsReferenceType)
        {
            return true;
        }

        return IsNullableValueType(typeInfo) || IsNullableReferenceType(typeInfo);
    }

    private static bool IsNullableValueType(ITypeSymbol typeInfo) =>
        typeInfo is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T;

    private static bool IsNullableReferenceType(ITypeSymbol typeInfo) =>
        typeInfo.IsReferenceType && typeInfo.NullableAnnotation == NullableAnnotation.Annotated;

    internal static TypeDescription CreateTypeDescription(ITypeSymbol typeSymbol, bool enabledNullable)
    {
        var isNullable = IsTypeNullable(typeSymbol, enabledNullable);
        return new TypeDescription(
            GlobalName: GetGlobalName(typeSymbol, isNullable, typeSymbol.IsValueType),
            IsNullable: isNullable,
            IsGenericParameter: typeSymbol.Kind == SymbolKind.TypeParameter, //TODO: Add support for generic parameters
            IsValueType: typeSymbol.IsValueType);
    }

    internal static string GetGlobalName(ITypeSymbol typeSymbol, bool isNullable, bool isValueType)
    {
        if (isNullable && isValueType)
        {
            // Strips the "?" from the type name
            return ((INamedTypeSymbol)typeSymbol).TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public static uint ComputeSha256Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        uint hashInt = BitConverter.ToUInt32(bytes, 0);

        return hashInt;
    }
}
