using System;

namespace ArmatSoftware.Code.Engine.Compiler.Utils
{
    public static class Names
    {
        public static string GenerateUniqueAssemblyName()
        {
            return $"Assembly{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
        }

        public static string GenerateFullTypeName<TSubject>(ICompilerConfiguration<TSubject> configuration) where TSubject: class
        {
            return $"{configuration.GetNamespace()}.{configuration.GetClassName()}";
        }
    }
}