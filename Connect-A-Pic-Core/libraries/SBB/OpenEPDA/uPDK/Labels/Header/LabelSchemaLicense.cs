using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Header
{
    /// <summary>
    /// License information for using this superschema and the derived schema.
    /// </summary>
    public class LabelSchemaLicense
    {
        /// <value>CC BY-SA 4.0 - Mandatory Creative Commons license condition.</value>
        [YamlMember(Alias = "license")]
        public string License
        { get; set; } = "CC BY-SA 4.0";
        /// <value>openEPDA--uPDK-SBB-v0.4 - Mandatory attribution required under the Creative Commons license.</value>
        [YamlMember(Alias = "attribution")]
        public string Attribution
        { get; set; } = "openEPDA-uPDK-SBB-v0.4";
    }
}
