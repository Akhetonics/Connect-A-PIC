using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Blocks
{
    /// <summary>
    /// Defines a BB parameter.
    /// </summary>
    public class LabelParameter
    {
        /// <value>Short parameter description.</value>
        [YamlMember(Alias = "doc")]
        public string Doc
        { get; set; } = "No documentation provided.";
        /// <value>Data type.</value>
        [YamlMember(Alias = "type")]
        public string Type
        { get; set; }
        /// <value>Unit of the parameter.</value>
        [YamlMember(Alias = "unit")]
        public string Unit
        { get; set; }
        /// <value>Minimum value.</value>
        [YamlMember(Alias = "min")]
        public object Min
        { get; set; }
        /// <value>Maximum value.</value>
        [YamlMember(Alias = "max")]
        public object Max
        { get; set; }
        /// <value>Default value.</value>
        [YamlMember(Alias = "value")]
        public object Value
        { get; set; }
        /// <value>Alias for the parameter.</value>
        [YamlMember(Alias = "alias")]
        public string Alias
        { get; set; }
    }
}
