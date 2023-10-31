using System.Collections.Generic;
using AtetDataFormats.OpenEPDA.Labels.Subschemas;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels
{
    /// <summary>
    /// Describe zero or more subschemas.
    /// </summary>
    public class LabelSubschemas
    {
        /// <value>Describes one or more DRC-Rules.</value>
        [YamlMember(Alias = "drc-rules")]
        public LabelDRCRules DRCRules
        { get; set; }
        /// <value>Describes one or more values and domains.</value>
        [YamlMember(Alias = "values_and_domains")]
        public LabelValuesAndDomains ValuesAndDomains
        { get; set; }
        /// <value>Describes one or models.</value>
        [YamlMember(Alias = "models")]
        public Dictionary<string, LabelModel> Models
        { get; set; }
        /// <value>Describes ip-block data.</value>
        [YamlMember(Alias = "ip_block")]
        public LabelIPBlock IPBlock
        { get; set; }
    }
}
