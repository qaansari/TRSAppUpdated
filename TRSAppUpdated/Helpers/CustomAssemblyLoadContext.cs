using System.Reflection;
using System.Runtime.Loader;

namespace TRSAppUpdated.Helpers
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return base.LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null!;
        }
    }
}
