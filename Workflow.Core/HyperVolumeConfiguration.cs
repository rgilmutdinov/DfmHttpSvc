using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DfmServer.Managed.Extensions;
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

            XmlSerializer serializer = new XmlSerializer(typeof(HyperVolume));

            using (TextReader reader = new StringReader(xmlConfig))
            {
                HyperVolume hyperVolume = (HyperVolume) serializer.Deserialize(reader);

                ValidateFilters(hyperVolume);
                ValidateForms(hyperVolume);
            }
        }

        private static void ValidateForms(HyperVolume hyperVolume)
        {
            int massiveForms = hyperVolume.Form.Count(f => f.Role == FormRole.MassiveUpdate);
            if (massiveForms > 1)
            {
                throw new HyperVolumeConfigurationException("Hyper volume should not contain more than one massive update form.");
            }

            List<GestPayButton> gestPayButtons = new List<GestPayButton>();
            foreach (Form form1 in hyperVolume.Form)
            {
                int nameCount = 0;

                foreach (Form form2 in hyperVolume.Form)
                {
                    if (form2.Name.Equals(form1.Name) && ++nameCount > 1)
                    {
                        throw new HyperVolumeConfigurationException($"Form with name \"{form1.Name}\" already exists.");
                    }
                }

                FormFakeDocument fakeDocument = form1.FakeDocument;
                bool isFakeDocument = fakeDocument != null && fakeDocument.Enabled == true;
                if (form1.Role == FormRole.MassiveUpdate)
                {
                    if (form1.LocalAppButton.Any())
                    {
                        throw new HyperVolumeConfigurationException("Massive update form should not contain Local App button.");
                    }

                    if (form1.GestPayButton.Any())
                    {
                        throw new HyperVolumeConfigurationException("Massive update form should not contain Payment button.");
                    }

                    if (form1.GSignButton.Any())
                    {
                        throw new HyperVolumeConfigurationException("Massive update form should not contain GSign button.");
                    }

                    if (form1.PreviewPanel.Any())
                    {
                        throw new HyperVolumeConfigurationException("Massive update form should not contain Preview panel.");
                    }

                    if (form1.ScanButton.Any())
                    {
                        throw new HyperVolumeConfigurationException("Massive update form should not contain Scan button.");
                    }

                    if (form1.QuestionnaireButton.Any())
                    {
                        throw new HyperVolumeConfigurationException("Massive update form should not contain Questionnaire button.");
                    }
                }

                foreach (LocalAppButton appButton in form1.LocalAppButton)
                {
                    if (appButton.Enabled == true)
                    {
                        throw new HyperVolumeConfigurationException("Fake document creation form should not contain any Local App Button enabled");
                    }
                }

                foreach (Button button1 in form1.Button)
                {
                    int idCount = 0;
                    foreach (Button button2 in form1.Button)
                    { 
                        if (button1.CommandId.Equals(button2.CommandId) && ++idCount > 1)
                        {
                            throw new HyperVolumeConfigurationException($"Button with ID \"{button1.CommandId}\" already exists.");
                        }
                    }

                    if (form1.Role == FormRole.NewRecord)
                    {
                        foreach (PDFPrintAction printAction in button1.PdfPrintAction)
                        {
                            if (printAction.Enabled && !isFakeDocument)
                            {
                                throw new HyperVolumeConfigurationException(
                                    "Non fake document creation form should not contain PDF print action enabled.");
                            }
                        }

                        foreach (StampedPrintAction printAction in button1.StampedPrintAction)
                        {
                            if (printAction.Enabled)
                            {
                                throw new HyperVolumeConfigurationException("Document creation form should not contain document print action enabled.");
                            }
                        }

                    }
                }

                foreach (GestPayButton gpButton in form1.GestPayButton)
                {
                    if (form1.Role == FormRole.NewRecord)
                    {
                        throw new HyperVolumeConfigurationException("Form for new document creation should not contain Payment buttons");
                    }

                    foreach (GestPayButton paymentButton in gestPayButtons)
                    {
                        if (!gpButton.Amount.Equals(paymentButton.Amount, StringComparison.OrdinalIgnoreCase) ||
                            !gpButton.MerchantId.Equals(paymentButton.MerchantId, StringComparison.OrdinalIgnoreCase) ||
                            !gpButton.Currency.Equals(paymentButton.Currency, StringComparison.OrdinalIgnoreCase) ||
                            !gpButton.TransactionStatus.Equals(paymentButton.TransactionStatus, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new HyperVolumeConfigurationException("HyperVolume should not contain different definitions of Payment buttons");
                        }
                    }
                    gestPayButtons.Add(gpButton);
                }

                foreach (GSignButton gsButton in form1.GSignButton)
                {
                    if (form1.Role == FormRole.NewRecord && gsButton.Enabled == true)
                    {
                        throw new HyperVolumeConfigurationException("Form for document creation should not contain GSign button.");
                    }
                }

                foreach (ScanButton scanButton in form1.ScanButton)
                {
                    if (form1.Role == FormRole.UpdateRecord)
                    {
                        throw new HyperVolumeConfigurationException("Update record form should not contain Scan button.");
                    }

                    if (isFakeDocument && scanButton.Enabled == true)
                    {
                        throw new HyperVolumeConfigurationException("Fake document creation form should not contain any Scan buttons enabled.");
                    }
                }

                foreach (PreviewPanel previewPanel in form1.PreviewPanel)
                {
                    if (form1.Role == FormRole.NewRecord && previewPanel.Enabled == true)
                    {
                        throw new HyperVolumeConfigurationException("Record creation form can not contain Preview panel.");
                    }
                }
            }
        }

        private static void ValidateFilters(HyperVolume hyperVolume)
        {
            foreach (Filter filter in hyperVolume.Filter)
            {
                Collection<FilterFilterForm> filterForms = filter.FilterForm;
                if (filterForms != null)
                {
                    foreach (FilterFilterForm filterForm in filterForms)
                    {
                        String formName = filterForm.FormName;
                        if (!formName.IsNullOrEmpty())
                        {
                            if (!FormExists(formName, hyperVolume))
                            {
                                throw new HyperVolumeConfigurationException($"Filter refers to unknown form \"{formName}\".");
                            }
                        }
                    }
                }
            }
        }

        private static bool FormExists(string formName, HyperVolume hyperVolume)
        {
            foreach (Form form in hyperVolume.Form)
            {
                if (formName.Equals(form.Name))
                {
                    return true;
                }
            }

            return false;
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
