using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using System.Configuration;

namespace PluginLib {
    public class AssemblyResourceVirtualFile : System.Web.Hosting.VirtualFile {
        private string path;

        public AssemblyResourceVirtualFile(string virtualPath)
            : base(virtualPath) {
            path = VirtualPathUtility.ToAppRelative(virtualPath);
        }

        public override Stream Open() {
            string[] parts = path.Split('/');
            string assemblyName = parts[2];
            string resourceName = parts[3];

            assemblyName = Path.Combine(HttpRuntime.BinDirectory, assemblyName);
            if (!File.Exists(assemblyName))
            {
                if (ConfigurationManager.AppSettings["EmptyViewVirtualPath"] != null)
                {
                    string emptyViewPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["EmptyViewVirtualPath"]);
                    return new FileStream(emptyViewPath, FileMode.Open);
                }
            }
            byte[] assemblyBytes = File.ReadAllBytes(assemblyName);
            Assembly assembly = Assembly.Load(assemblyBytes);
            if (assembly != null)
            {
               return assembly.GetManifestResourceStream(resourceName);
            }

            return null;
        }
    } 
}