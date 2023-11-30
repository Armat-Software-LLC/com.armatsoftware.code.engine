using System;

namespace ArmatSoftware.Code.Engine.Compiler.Utils
{
    public static class Names
    {
        public static string GenerateUniqueAssemblyName()
        {
            return $"Assembly{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
        }

        public static string GenerateFullTypeName<S>(ICompilerConfiguration<S> configuration) where S : class
        {
            return $"{configuration.GetNamespace()}.{configuration.GetClassName()}";
        }
    }
}