using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using DfmHttpCore;
using DfmHttpCore.Entities;
using DfmHttpSvc.Attributes;
using DfmHttpSvc.Controllers.Base;
using DfmHttpSvc.Dto;
using DfmHttpSvc.Properties;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    [Route("api/volumes/{volume}/[controller]")]
    public class DocumentsController : ApiController
    {
        public DocumentsController(SessionManager sessionManager) : base(sessionManager)
        {

        }

        /// <summary>
        /// Retrieves a page of documents in the volume.
        /// </summary>
        /// <param name="volumeQuery">Volume query</param>
        /// <param name="start">Start position (optional, default value is 0)</param>
        /// <param name="count">Documents count  (optional, default value is 50)</param>
        /// <returns>List of volume's documents</returns>
        /// <response code="200">Return the list of volume's documents</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(DocumentsResult), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet]
        public IActionResult GetDocuments(VolumeQuery volumeQuery, [FromQuery] int? start = 0, [FromQuery] int? count = 50)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            if (volumeQuery == null)
            {
                return BadRequest(Resources.ErrorEmptyRequest);
            }

            VolumeState volumeState = new VolumeState(
                volumeQuery.VolumeName,
                volumeQuery.FilterQuery,
                volumeQuery.SortOrder
            );

            DocumentsResult docsResult = session.GetDocuments(volumeState, start ?? 0, count ?? 50);

            return Ok(docsResult);
        }

        /// <summary>
        /// Updates fields of a document with the specified id.
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="updateFields">List of fields to update</param>
        /// <returns>Updated document</returns>
        /// <response code="200">Return updated document</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="404">Document with requested id not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(Document), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpPatch]
        [Route("{documentId}")]
        public IActionResult UpdateDocument(string volume, ulong documentId, [FromBody] List<Field> updateFields)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            if (!session.IsVolumeExist(volume))
            {
                return NotFound(Resources.ErrorVolumeNotFound);
            }

            DocIdentity docId = new DocIdentity(documentId);

            if (!session.IsDocumentExist(volume, docId))
            {
                return NotFound(Resources.ErrorDocumentNotFound);
            }

            Document updatedDocument = session
                .UpdateDocumentFields(volume, docId, updateFields);

            return Ok(updatedDocument);
        }

        /// <summary>
        /// Generates temporary token to download a document
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <returns>Temporary token</returns>
        /// <response code="200">Returns temporary token for download a document</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("{documentId}/token")]
        public IActionResult GetDocumentDownloadToken(string volume, ulong documentId)
        {
            if (!TryGetSession(User, out Session _))
            {
                return Unauthorized();
            }

            DocIdentity identity = new DocIdentity(documentId);

            string sessionId = GetSessionId(User);
            DownloadTicket ticket = SessionManager.CreateDownloadTicket(sessionId, volume, identity);

            return Ok(new
            {
                ticket.Token
            });
        }

        /// <summary>
        /// Downloads document by temporary token
        /// </summary>
        /// <param name="token">temporary token</param>
        /// <returns>A document (file), associated with the temporary token</returns>
        /// <response code="200">A document (file)</response>
        /// <response code="404">Token is not valid or expired</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>        
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet("/api/downloads/{token}")]
        public IActionResult DownloadByToken(string token)
        {
            DownloadTicket ticket = SessionManager.GetDownloadTicket(token);
            if (ticket == null)
            {
                return NotFound("Token is not valid or expired.");
            }

            if (!TryGetSession(ticket.SessionId, out Session session))
            {
                return Unauthorized();
            }

            return DownloadDocument(
                session, ticket.VolumeName, ticket.DocumentId);
        }

        /// <summary>
        /// Retrieves a document (file) with the specified id
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <returns>The requested document file</returns>
        /// <response code="200">Returns the requested document file</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("{documentId}", Name = Routes.GetDocument)]
        [DeleteFile]
        public IActionResult GetDocument(string volume, ulong documentId)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            return DownloadDocument(session, volume, documentId);
        }

        /// <summary>
        /// Adds new document (file) to the volume
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="fields">Document fields (metadata)</param>
        /// <param name="file">Document file</param>
        /// <response code="201">Document was added successfully</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Consumes("multipart/form-data")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDocument(string volume, [JsonFromForm] List<Field> fields, IFormFile file)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            if (!session.IsVolumeExist(volume))
            {
                return NotFound(Resources.ErrorVolumeNotFound);
            }

            if (file == null)
            {
                return BadRequest(Resources.ErrorFileIsMissing);
            }

            if (file.Length <= 0)
            {
                return BadRequest(Resources.ErrorFileIsEmpty);
            }

            string filePath = GetFilePath(session, file);

            DocIdentity docIdentity;
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                docIdentity = session.NewDocumentFromFile(volume, filePath, fields);
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }

            return CreatedAtRoute(
                Routes.GetDocument,
                new { volume, documentId = docIdentity.CompositeId },
                docIdentity.CompositeId.ToString()
            );
        }

        /// <summary>
        /// Deletes the document with specified id
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <response code="204">Document was deleted successfully</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="404">Document with requested id not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpDelete]
        [Route("{documentId}")]
        public IActionResult DeleteDocument(string volume, ulong documentId)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            if (!session.IsVolumeExist(volume))
            {
                return NotFound(Resources.ErrorVolumeNotFound);
            }

            DocIdentity docId = new DocIdentity(documentId);

            if (!session.IsDocumentExist(volume, docId))
            {
                return NotFound(Resources.ErrorDocumentNotFound);
            }

            session.DeleteDocument(volume, docId);

            return NoContent();
        }

        /// <summary>
        /// Deletes specified list of documents
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documents">List of document id</param>
        /// <returns>The list of deleted documents</returns>
        /// <response code="204">Documents was deleted successfully</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="403">List of documents is null or empty</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpDelete]
        public IActionResult DeleteDocuments(string volume, List<ulong> documents)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            if (!session.IsVolumeExist(volume))
            {
                return NotFound(Resources.ErrorVolumeNotFound);
            }

            if (documents == null || documents.Count == 0)
            {
                return BadRequest();
            }

            List<string> deleted = new List<string>();

            foreach (ulong document in documents)
            {
                DocIdentity docId = new DocIdentity(document);

                if (session.IsDocumentExist(volume, docId))
                {
                    session.DeleteDocument(volume, docId);
                    deleted.Add(document.ToString());
                }
            }
            return Ok(deleted);
        }


        private IActionResult DownloadDocument(Session session, string volume, ulong documentId)
        {
            DocIdentity identity = new DocIdentity(documentId);

            string filePath = session.ExtractDocument(volume, identity);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = Path.GetFileName(filePath),
                Inline   = true // true = browser to try to show the file inline; false = prompt the user for downloading
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());

            return PhysicalFile(filePath, contentType);
        }
    }
}
