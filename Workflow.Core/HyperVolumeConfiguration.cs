using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using WFSchema.HVConfig;
using Workflow.Schema;

namespace Workflow.Core
{
    public class HyperVolumeConfiguration
    {
        public HyperVolume HyperVolume { get; }
        public HyperVolumeInfo HyperVolumeInfo { get; }
        public byte[] Hash { get;  }

        public static void ValidateWithSchema(string xmlConfig)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);

            string[] xsds = { Resource.wf_common, Resource.hv_configuration };
            foreach (string xsd in xsds)
            {
                using (TextReader schemaReader = new StringReader(xsd))
                {
                    XmlSchema schema = XmlSchema.Read(schemaReader, null);
                    xmlDoc.Schemas.Add(schema);
                }
            }
            
            xmlDoc.Validate(null);
        }

        public HyperVolumeConfiguration(string xmlConfig, HyperVolumeInfo hyperVolumeInfo)
        {
            this.HyperVolumeInfo = hyperVolumeInfo;
            this.Hash = Sha256(xmlConfig);

            XmlSerializer serializer = new XmlSerializer(typeof(HyperVolume));

            using (TextReader reader = new StringReader(xmlConfig))
            {
                HyperVolume = (HyperVolume) serializer.Deserialize(reader);
            }
        }

        static byte[] Sha256(string text)
        {
            SHA256Managed crypt = new SHA256Managed();
            return crypt.ComputeHash(Encoding.UTF8.GetBytes(text));
        }
    }
}
