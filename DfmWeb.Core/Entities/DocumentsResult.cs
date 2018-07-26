using System.Collections.Generic;
using DfmServer.Managed.Collections;

namespace DfmWeb.Core.Entities
{
    public class DocumentsResult
    {
        public static DocumentsResult Empty = new DocumentsResult();

        private DocumentsResult() : this(Lists.Empty<Document>(), 0)
        {
        }

        public DocumentsResult(List<Document> documents, int totalDocuments)
        {
            this.Documents = documents;
            this.TotalDocuments = totalDocuments;
        }

        public List<Document> Documents { get; }
        public int TotalDocuments { get; }
    }
}
