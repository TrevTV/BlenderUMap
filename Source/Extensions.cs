using System.IO;
using CUE4Parse.UE4.Assets.Exports;

namespace BlenderUMap
{
    internal static class Extensions
    {
		public static DirectoryInfo GetExportDir(this UObject obj)
		{
			var pkgPath = obj?.GetPathName();
			int idx = pkgPath.LastIndexOf('.');
			if (idx >= 0)
				pkgPath = pkgPath.Substring(0, idx);
			if (pkgPath.StartsWith("/"))
				pkgPath = pkgPath.Substring(1);
			var outputDir = new DirectoryInfo(pkgPath).Parent;
			outputDir.Create();
			return outputDir;
		}
	}
}
