using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Blocks
{
    /// <summary>
    /// Defines an icon for the block.
    /// </summary>
    public class LabelIcon
    {
        /// <value>Name of the function that returns a cell with the icon.</value>
        [YamlMember(Alias = "function")]
        public string Function
        { get; set; }
        /// <value>Parameters for which default will be overridden.</value>
        [YamlMember(Alias = "parameters")]
        public LabelParameters Parameters
        { get; set; }
    }
}
