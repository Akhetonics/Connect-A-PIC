using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AtetDataFormats.OpenEPDA
{
    /// <summary>
    /// Creates individual SBBs from a YAML file.
    /// Implementation of the openEPDA uPDK (tm) Blocks.
    /// Version 0.4 (latest, draft) - https://openepda.org/updk/0.4/format_specification.html
    /// </summary>
    public static class SBBBuilder
    {
        /// <summary>
        /// Creates a Standard Black Block (SBB) from YAML.
        /// </summary>
        /// <returns>
        /// The SBB or null on failure.
        /// </returns>
        public static SBB createSBB(string yamlData)
        {
            try
            {
                IDeserializer deserializer = new DeserializerBuilder()
                .Build();
                return deserializer.Deserialize<SBB>(yamlData);
            } catch (Exception ex)
            {
                return null;
            }
        }
    }
}
