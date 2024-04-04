namespace CAP_Core.CodeExporter
{
    public class PythonResources
    {
        public static string CreateHeader(string PDKName, string StandardInputCellName)
        {
            return Resources.NazcaHeader
                .Replace("CAPICPDK", PDKName)
                .Replace("grating", StandardInputCellName);
        }

        public static string CreateFooter(string? layoutName = null, string? gdsFileName = null)
        {
            layoutName ??= Resources.NazcaLayoutName;
            gdsFileName ??= Resources.PDKFileName;
            return Resources.NazcaFooter
                .Replace("Akhetonics_ConnectAPIC", layoutName)
                .Replace("ReplaceThisFileName.gds", gdsFileName);
        }

    }
}
