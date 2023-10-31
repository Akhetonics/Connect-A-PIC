using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Blocks
{
    /// <summary>
    /// Parameters for which default will be overridden.
    /// </summary>
    public class LabelParameters
    {
        /// <value>Buffer in the x-direction in um.</value>
        [YamlMember(Alias = "bufx")]
        public float BufX
        { get; set; }
        /// <value>Buffer in the y-direction in um.</value>
        [YamlMember(Alias = "bufy")]
        public float BufY
        { get; set; }
        /// <value>Icon length in the x-direction in um.</value>
        [YamlMember(Alias = "length")]
        public float Length
        { get; set; }
        /// <value>Icon length in the y-direction in um.</value>
        [YamlMember(Alias = "width")]
        public float Width
        { get; set; }
    }
}
