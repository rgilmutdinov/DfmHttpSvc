using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;
using WFSchema.HVConfig;
using Workflow.Core;

namespace Workflow.Tests
{
    [TestFixture]
    public class WFSchemaTests
    {
        [Test]
        public void TestWFSchemaDeserialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HyperVolume));

            HyperVolume hv;
            using (TextReader reader = new StringReader(Resource.test_hv))
            {
                hv = (HyperVolume) serializer.Deserialize(reader);
            }

            Assert.NotNull(hv);
            StringAssert.AreEqualIgnoringCase("Test hyper volume", hv.Description);
            Assert.AreEqual(2, hv.Filter.Count);
            Assert.AreEqual(2, hv.Form.Count);

            StringAssert.AreEqualIgnoringCase("Create", hv.Form[0].Name);
            StringAssert.AreEqualIgnoringCase("Modify", hv.Form[1].Name);

            StringAssert.AreEqualIgnoringCase("1", hv.Filter[0].SequenceNumber);
            StringAssert.AreEqualIgnoringCase("2", hv.Filter[1].SequenceNumber);
        }

        [Test]
        public void ValidateWithSchema_ValidHyperVolume_NoExceptionThrown()
        {
            Assert.That(
                () => HyperVolumeConfiguration.ValidateWithSchema(Resource.test_hv),
                Throws.Nothing);
        }

        [Test]
        public void ValidateWithSchema_InvalidHyperVolume_ExceptionThrown()
        {
            Assert.That(
                () => HyperVolumeConfiguration.ValidateWithSchema(Resource.invalid_hv), 
                Throws.TypeOf<XmlSchemaValidationException>());
        }
    }
}
