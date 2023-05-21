using System;
namespace UES
{
    class Utils
    {
        static public string GetParameterTypeName(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return "string";
                case TypeCode.Boolean:
                    return "bool";
                case TypeCode.Byte:
                    return "byte";
                case TypeCode.Char:
                    return "char";
                case TypeCode.Decimal:
                    return "decimal";
                case TypeCode.Double:
                    return "double";
                case TypeCode.Int16:
                    return "short";
                case TypeCode.Int32:
                    return "int";
                case TypeCode.Int64:
                    return "long";
                case TypeCode.Object:
                    if (type == typeof(string))
                    {
                        return "string";
                    }
                    else if (type.IsArray)
                    {
                        return $"{GetParameterTypeName(type.GetElementType())}[]";
                    }
                    else
                    {
                        return type.Name;
                    }
                case TypeCode.SByte:
                    return "sbyte";
                case TypeCode.Single:
                    return "float";
                case TypeCode.UInt16:
                    return "ushort";
                case TypeCode.UInt32:
                    return "uint";
                case TypeCode.UInt64:
                    return "ulong";
                default:
                    throw new NotSupportedException($"Type {type.Name} is not supported.");
            }
        }
    }
}