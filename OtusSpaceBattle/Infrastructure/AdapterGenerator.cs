using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.IO;
using OtusSpaceBattle.Interfaces;

namespace OtusSpaceBattle.Infrastructure
{
    public static class AdapterGenerator
    {
        private static readonly Dictionary<Type, string> TypeAliases = new()
        {
            { typeof(void), "void" },
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(bool), "bool" },
            { typeof(string), "string" },
            { typeof(object), "object" },
            { typeof(long), "long" },
            { typeof(short), "short" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
            { typeof(sbyte), "sbyte" },
        };

        private static string GetTypeName(Type type)
        {
            if (TypeAliases.TryGetValue(type, out var alias))
                return alias;

            if (type.IsArray)
                return GetTypeName(type.GetElementType()!) + "[]";

            if (type.IsGenericType)
            {
                var genericTypeName = type.Name.Split('`')[0];
                var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
                return $"{genericTypeName}<{genericArgs}>";
            }

            if (type.IsNested)
                return GetTypeName(type.DeclaringType!) + "." + type.Name;
            return type.Name;
        }

        public static string GenerateAdapterCode(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("Type must be an interface");

            var ns = interfaceType.Namespace?.Replace("Interfaces", "Adapters") ?? "OtusSpaceBattle.Adapters";
            var className = interfaceType.Name.TrimStart('I') + "Adapter";
            var sb = new StringBuilder();
            sb.AppendLine($"using OtusSpaceBattle.Infrastructure;");
            sb.AppendLine($"using OtusSpaceBattle.Interfaces;");
            sb.AppendLine($"using System.Numerics;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className} : {interfaceType.FullName}");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IUObject gameObject;");
            sb.AppendLine();
            sb.AppendLine($"        public {className}(IUObject gameObject)");
            sb.AppendLine("        {");
            sb.AppendLine("            this.gameObject = gameObject;");
            sb.AppendLine("        }");
            sb.AppendLine();

            foreach (var method in interfaceType.GetMethods())
            {
                var returnType = method.ReturnType;
                var methodName = method.Name;
                var parameters = method.GetParameters();
                var paramList = string.Join(", ", parameters.Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));
                var paramNames = string.Join(", ", parameters.Select(p => p.Name));

                string iocKey;
                if (methodName.StartsWith("Get"))
                    iocKey = $"{interfaceType.FullName}:{methodName.Substring(3).ToLower()}.get";
                else if (methodName.StartsWith("Set"))
                    iocKey = $"{interfaceType.FullName}:{methodName.Substring(3).ToLower()}.set";
                else
                    iocKey = $"{interfaceType.FullName}:{methodName.ToLower()}";

                sb.AppendLine($"        public {GetTypeName(returnType)} {methodName}({paramList})");
                sb.AppendLine("        {");
                if (returnType == typeof(void))
                {
                    sb.AppendLine($"            IoC.Resolve<ICommand>(\"{iocKey}\", gameObject{(paramNames.Length > 0 ? ", " + paramNames : string.Empty)}).Execute();");
                }
                else
                {
                    sb.AppendLine($"            return IoC.Resolve<{GetTypeName(returnType)}>(\"{iocKey}\", gameObject{(paramNames.Length > 0 ? ", " + paramNames : string.Empty)});");
                }
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static void GenerateAndSaveAllAdapters(string outputDir, Assembly assembly)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var interfaceTypes = assembly.GetTypes()
                .Where(t => t.IsInterface)
                .ToList();

            foreach (var iface in interfaceTypes)
            {
                var code = GenerateAdapterCode(iface);
                var fileName = iface.Name.TrimStart('I') + "Adapter.cs";
                var filePath = Path.Combine(outputDir, fileName);
                File.WriteAllText(filePath, code);
            }
        }
    }
}
