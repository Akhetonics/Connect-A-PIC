using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Subschemas
{
    /// <summary>
    /// Describes one or more DRC-Rules.
    /// </summary>
    public class LabelDRCRules
    {
        /// <value>Angle DRC rule for instantiation w.r.t the mask.</value>
        [YamlMember(Alias = "angle")]
        public LabelValuesAndDomains Angle
        { get; set; }
        /// <value>Angle DRC rule for instantiation w.r.t the mask with mirroring status.</value>
        [YamlMember(Alias = "angle_mirror")]
        public LabelAngleMirror AngleMirror
        { get; set; }
    }
}
