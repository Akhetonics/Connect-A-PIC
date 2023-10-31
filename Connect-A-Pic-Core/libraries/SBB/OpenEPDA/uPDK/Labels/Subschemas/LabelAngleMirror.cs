using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Subschemas
{
    /// <summary>
    /// angle DRC rule for instantiation w.r.t the mask with mirroring status.
    /// </summary>
    public class LabelAngleMirror
    {
        /// <value>Group rules that apply for a flip=true state.</value>
        [YamlMember(Alias = "flip")]
        public LabelValuesAndDomains Flip
        { get; set; }
        /// <value>Group rules that apply for a flip=false state.</value>
        [YamlMember(Alias = "noflip")]
        public LabelValuesAndDomains NoFlip
        { get; set; }
    }
}
