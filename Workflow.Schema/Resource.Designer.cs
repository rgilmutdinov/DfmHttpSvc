﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Workflow.Schema {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Workflow.Schema.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot; attributeFormDefault=&quot;unqualified&quot;&gt;
        ///	&lt;xs:include schemaLocation=&quot;wf_common.xsd&quot;/&gt;
        ///        &lt;xs:element name=&quot;attachmentStampTemplate&quot; type=&quot;AttachmentStampTemplate&quot;/&gt;
        ///        &lt;xs:complexType name=&quot;TextObj&quot;&gt;
        ///		&lt;xs:sequence&gt;
        ///			&lt;xs:element name=&quot;name&quot; type=&quot;xs:string&quot;/&gt;
        ///			&lt;xs:element name=&quot;data&quot; type=&quot;xs:string&quot;/&gt;
        ///		&lt;/xs:sequence&gt;
        ///	&lt;/xs:complexType&gt;
        ///        &lt;xs:complexType nam [rest of string was truncated]&quot;;.
        /// </summary>
        public static string attachmentprint_configuration {
            get {
                return ResourceManager.GetString("attachmentprint_configuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;!-- edited with XMLSpy v2012 sp1 (http://www.altova.com) by Leonid (Noname) --&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot; attributeFormDefault=&quot;unqualified&quot;&gt;
        ///	&lt;xs:include schemaLocation=&quot;wf_common.xsd&quot;/&gt;
        ///	&lt;xs:complexType name=&quot;HyperArea&quot;&gt;
        ///		&lt;xs:sequence&gt;
        ///			&lt;xs:element name=&quot;label&quot; type=&quot;LocalizedText&quot; minOccurs=&quot;0&quot; maxOccurs=&quot;unbounded&quot;/&gt;
        ///			&lt;xs:element name=&quot;description&quot; type=&quot;LocalizedText&quot; minOccurs=&quot;0&quot; maxOccurs= [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ha_configuration {
            get {
                return ResourceManager.GetString("ha_configuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;!-- edited with XMLSpy v2012 sp1 (http://www.altova.com) by Leonid (Noname) --&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot; attributeFormDefault=&quot;unqualified&quot;&gt;
        ///    &lt;xs:include schemaLocation=&quot;wf_common.xsd&quot;/&gt;
        ///    &lt;xs:complexType name=&quot;Filter&quot;&gt;
        ///        &lt;xs:sequence&gt;
        ///            &lt;xs:element name=&quot;conditions&quot; type=&quot;MetadataCondition&quot;/&gt;
        ///            &lt;xs:element name=&quot;onsign&quot; minOccurs=&quot;0&quot;&gt;
        ///                &lt;xs:complexType&gt;
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string hv_configuration {
            get {
                return ResourceManager.GetString("hv_configuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot; attributeFormDefault=&quot;unqualified&quot;&gt;
        ///	&lt;xs:complexType name=&quot;SerializableFieldInfo&quot;&gt;
        ///		&lt;xs:sequence&gt;
        ///			&lt;xs:element name=&quot;size&quot; type=&quot;xs:int&quot;/&gt;
        ///			&lt;xs:element name=&quot;caption&quot; type=&quot;xs:string&quot;/&gt;
        ///			&lt;xs:element name=&quot;precision&quot; type=&quot;xs:int&quot;/&gt;
        ///			&lt;xs:element name=&quot;dateFormat&quot; type=&quot;xs:int&quot;/&gt;
        ///			&lt;xs:element name=&quot;autofill&quot; type=&quot;xs:string&quot;/&gt;
        ///			&lt;xs:element name=&quot;updatable&quot; type=&quot;xs [rest of string was truncated]&quot;;.
        /// </summary>
        public static string hv_serialization {
            get {
                return ResourceManager.GetString("hv_serialization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot; attributeFormDefault=&quot;unqualified&quot;&gt;
        ///	&lt;xs:include schemaLocation=&quot;wf_common.xsd&quot;/&gt;
        ///	&lt;xs:element name=&quot;pdfPrintTemplate&quot; type=&quot;PdfPrintTemplate&quot;/&gt;
        ///	&lt;xs:complexType name=&quot;GrObj&quot;&gt;
        ///		&lt;xs:sequence&gt;
        ///			&lt;xs:element name=&quot;name&quot; type=&quot;xs:string&quot;/&gt;
        ///			&lt;xs:element name=&quot;type&quot; type=&quot;xs:string&quot;/&gt;
        ///			&lt;xs:element name=&quot;data&quot; type=&quot;xs:string&quot;/&gt;
        ///		&lt;/xs:sequence&gt;
        ///	&lt;/xs:complexType&gt;
        ///	&lt;xs:compl [rest of string was truncated]&quot;;.
        /// </summary>
        public static string pdfprint_configuration {
            get {
                return ResourceManager.GetString("pdfprint_configuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot; attributeFormDefault=&quot;unqualified&quot;&gt;
        ///    &lt;xs:element name=&quot;questionnaire&quot; type=&quot;Questionnaire&quot;/&gt;
        ///    &lt;xs:element name=&quot;questioningMode&quot; type=&quot;QuestioningMode&quot;/&gt;
        ///    &lt;xs:element name=&quot;modeAllQuestions&quot; type=&quot;ModeAllQuestions&quot; substitutionGroup=&quot;questioningMode&quot;/&gt;
        ///    &lt;xs:element name=&quot;modeSubsetOfQuestions&quot; type=&quot;ModeSubsetOfQuestions&quot; substitutionGroup=&quot;questioningMode&quot;/&gt;
        ///    &lt;xs [rest of string was truncated]&quot;;.
        /// </summary>
        public static string qhv_configuration {
            get {
                return ResourceManager.GetString("qhv_configuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;xs:schema xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot; elementFormDefault=&quot;qualified&quot;
        ///	attributeFormDefault=&quot;qualified&quot;&gt;
        ///	&lt;xs:complexType name=&quot;LocalizedText&quot;&gt;
        ///		&lt;xs:simpleContent&gt;
        ///			&lt;xs:extension base=&quot;xs:string&quot;&gt;
        ///				&lt;xs:attribute name=&quot;lang&quot; type=&quot;xs:string&quot; use=&quot;optional&quot;/&gt;
        ///			&lt;/xs:extension&gt;
        ///		&lt;/xs:simpleContent&gt;
        ///	&lt;/xs:complexType&gt;
        ///	&lt;xs:complexType name=&quot;Field&quot;&gt;
        ///		&lt;xs:simpleContent&gt;
        ///			&lt;xs:extension base=&quot;xs:string&quot;/&gt;
        ///		&lt;/xs:simpleContent&gt;
        ///	&lt;/xs:com [rest of string was truncated]&quot;;.
        /// </summary>
        public static string wf_common {
            get {
                return ResourceManager.GetString("wf_common", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        public static byte[] wf_common_episode {
            get {
                object obj = ResourceManager.GetObject("wf_common_episode", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
