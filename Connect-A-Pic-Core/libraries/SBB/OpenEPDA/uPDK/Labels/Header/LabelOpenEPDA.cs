using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Header
{
    /// <summary>
    /// Label for openEPDA related information.
    /// </summary>
    public class LabelOpenEPDA
    {
        /// <value>openEPDA version of this scheme.</value>
        [YamlMember(Alias = "version")]
        public string Version
        { get; set; } = "openEPDA-uPDK-SBB-v0.4";
        /// <value>Link to openEPDA site.</value>
        [YamlMember(Alias = "link")]
        public string Link
        { get; set; } = "https://www.openEPDA.org";
    }
}
