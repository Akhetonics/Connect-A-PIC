using AtetDataFormats.OpenEPDA.Labels.Header;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels
{
    /// <summary>
    /// Contains licensing and usage information for an SBB.
    /// </summary>
    public class LabelHeader
    {
        /// <value>Short info on the purpose of this scheme.</value>
        [YamlMember(Alias = "description")]
        public string Description { get; set; } = "schema to describe a uPDK.";
        /// <value>Version of the SBB file.</value>
        [YamlMember(Alias = "file_version")]
        public string FileVersion { get; set; } = "1.0";
        /// <value>Version of the SBB file.</value>
        [YamlMember(Alias = "openEPDA")]
        public LabelOpenEPDA OpenEPDA { get; set; }
        /// <value>Version of the SBB file.</value>
        [YamlMember(Alias = "schema_license")]
        public LabelSchemaLicense SchemaLicense { get; set; }
        /// <value>License conditions of the content in the YAML.</value>
        [YamlMember(Alias = "pdk_license")]
        public string PdkLicense { get; set; }
    }
}
