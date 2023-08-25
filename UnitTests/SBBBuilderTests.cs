using AtetDataFormats.OpenEPDA;
using AtetDataFormats.OpenEPDA.Labels;
using AtetDataFormats.OpenEPDA.Labels.Blocks;
using AtetDataFormats.OpenEPDA.Labels.Header;
using AtetDataFormats.OpenEPDA.Labels.Subschemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using static System.Reflection.Metadata.BlobBuilder;

namespace UnitTests
{
    public class SBBBuilderTests
    {
        [Fact]
        public void TestCreateSBB()
        {
            var BasicComponent = new Dictionary<string, object>
            {
                { "header", new LabelHeader
                    {
                        Description = "uPDK example of building blocks",
                        FileVersion = "0.1",
                        OpenEPDA = new LabelOpenEPDA
                        {
                            Version = "openEPDA-uPDK-SBB-v0.4",
                            Link = "https://openEPDA.org"
                        },
                        SchemaLicense = new LabelSchemaLicense
                        {
                            License = "CC BY-SA 4.0",
                            Attribution = "openEPDA-uPDK-SBB-v0.4"
                        },
                    PdkLicense = null
                    }
                },
                { "blocks", new Dictionary<string, LabelBlock>
                {
                    { "SampleCell", new LabelBlock
                        {
                            ID = "sampleID",
                            Version = "1.0",
                            License = "Sample License",
                            CellName = "SampleCell",
                            Doc = "This is a sample block.",
                            BBox = new List<List<string>>
                            {
                                new List<string> { "0", "0" },
                                new List<string> { "10", "0" },
                                new List<string> { "10", "10" },
                                new List<string> { "0", "10" }
                            },
                            BBMetalOutline = new List<List<List<string>>>
                            {
                                new List<List<string>>
                                {
                                    new List<string> { "1", "1" },
                                    new List<string> { "9", "1" },
                                    new List<string> { "9", "9" },
                                    new List<string> { "1", "9" }
                                }
                            },
                            BBWidth = 10.0f,
                            BBLength = 10.0f,
                            PinIn = "InputPin",
                            PinOut = "OutputPin",
                            Pins = new Dictionary<string, LabelPin>
                            {
                                { "pin1", new LabelPin() } // Assuming a default constructor or dummy values for LabelPin
                            },
                            Parameters = new Dictionary<string, LabelParameter>
                            {
                                { "param1", new LabelParameter() } // Assuming a default constructor or dummy values for LabelParameter
                            },
                            KeywordParameters = new List<string> { "keyword1", "keyword2" },
                            CellNameParameters = new List<string> { "cellnameParam1" },
                            Call = "SampleCallFunction",
                            GroupName = "SampleGroup",
                            IPBlock = new LabelIPBlock(), // Assuming a default constructor or dummy values for LabelIPBlock
                            Icon = new LabelIcon() // Assuming a default constructor or dummy values for LabelIcon
                        }
                    }
                }
            } };

            var yamlSerializer = new SerializerBuilder().Build();
            var yaml = yamlSerializer.Serialize(BasicComponent);
            SBB sbb = SBBBuilder.createSBB(yaml);
            Assert.Equal("sampleID", sbb.Blocks.First(b => b.Value.CellName == "SampleCell").Value.ID);
        }
    }
}
