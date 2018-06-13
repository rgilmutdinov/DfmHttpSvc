using System.Collections.Generic;
using DfmCore.Collections;

namespace DfmHttpCore.Entities
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
