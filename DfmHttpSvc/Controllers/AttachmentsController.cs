using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using DfmCore;
using DfmCore.Extensions;
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
    [Route("api/volumes/{volume}/documents/{documentId}/[controller]")]
    public class AttachmentsController : ApiController
    {
        public AttachmentsController(SessionManager sessionManager) : base(sessionManager)
        {

        }

        /// <summary>
        /// Retrieves attachment list of a document with the specified id
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <returns>The list of attachments of a document</returns>
        /// <response code="200">Returns the list of attachments of a document</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(List<AttachmentInfo>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet(Name = Routes.GetAttachments)]
        public IActionResult GetAttachments(string volume, ulong documentId)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            DocIdentity identity = new DocIdentity(documentId);
            List<AttachmentInfo> attachments = session.GetAttachments(volume, identity);

            return Ok(attachments);
        }

        /// <summary>
        /// Retrieves an archive that contains attachments of a document with the specified id
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="range">Attachments selection</param>
        /// <returns>The requested archive file</returns>
        /// <response code="200">Returns the requested archive</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpPost("download")]
        [DeleteFile]
        public IActionResult GetAttachmentsArchive(string volume, ulong documentId, [FromBody] AttachmentsRange range)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            AttachmentsSelection selection = new AttachmentsSelection(
                documentId,
                range.Attachments,
                range.ExcludeMode
            );

            if (!selection.IsValid())
            {
                return BadRequest("Selection parameter is not valid");
            }

            return GetSelection(session, volume, selection);
        }

        /// <summary>
        /// Generates temporary token to download an attachment
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="attachmentName">Attachment name</param>
        /// <returns>Temporary token</returns>
        /// <response code="200">Returns temporary token for download an attachment</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("attachment/{attachmentName}/token")]
        public IActionResult GetAttachmentToken(string volume, ulong documentId, string attachmentName)
        {
            if (!TryGetSession(User, out Session _))
            {
                return Unauthorized();
            }

            DocIdentity identity = new DocIdentity(documentId);

            string sessionId = GetSessionId(User);
            AttachmentsSelection selection = new AttachmentsSelection(identity.CompositeId, attachmentName);

            DownloadTicket ticket = SessionManager
                .CreateDownloadTicket(sessionId, volume, selection);

            return Ok(new { ticket.Token });
        }

        /// <summary>
        /// Generates temporary token to download an archive with a bunch of document's attachments
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "excludeMode": "true",
        ///         "attachmentNames": ['attachment1', 'attachment2']
        ///     }
        ///
        /// </remarks>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="range">Attachments selection</param>
        /// <returns>Temporary token</returns>
        /// <response code="200">Returns temporary token for download an archive with a bunch of documents</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpPost("download/token")]
        public IActionResult GetAttachmentsArchiveToken(string volume, ulong documentId, [FromBody] AttachmentsRange range)
        {
            if (!TryGetSession(User, out Session _))
            {
                return Unauthorized();
            }

            AttachmentsSelection selection = new AttachmentsSelection(
                documentId,
                range.Attachments,
                range.ExcludeMode
            );
            
            if (!selection.IsValid())
            {
                return BadRequest("Selection parameter is not valid");
            }

            string sessionId = GetSessionId(User);
            DownloadTicket ticket = SessionManager.CreateDownloadTicket(sessionId, volume, selection);

            return Ok(new { ticket.Token });
        }

        /// <summary>
        /// Retrieves the attachment (file) from document with the specified id
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="attachmentName">attachment name</param>
        /// <returns>The requested attachment (file)</returns>
        /// <response code="200">Returns the requested attachment file</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(PhysicalFileResult), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("attachment/{attachmentName}", Name = Routes.GetAttachment)]
        [DeleteFile]
        public IActionResult GetAttachment(string volume, ulong documentId, string attachmentName)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            DocIdentity identity = new DocIdentity(documentId);
            string filePath = session.ExtractAttachment(volume, identity, attachmentName);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            return PhysicalFile(filePath, contentType);
        }

        /// <summary>
        /// Adds new attachment (file) to the document
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="file">Attachment file</param>
        /// <param name="attachmentName">(optional) The attachment name; if empty attachment file name is used.</param>
        /// <response code="201">Attachment was added successfully</response>
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
        public async Task<IActionResult> AddAttachment(string volume, ulong documentId, [FromForm] IFormFile file, [FromForm] string attachmentName = "")
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

            DocIdentity docIdentity = new DocIdentity(documentId);
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                session.NewAttachmentFromFile(volume, docIdentity, filePath, attachmentName);
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }

            if (attachmentName.IsNullOrEmpty())
            {
                attachmentName = Path.GetFileNameWithoutExtension(filePath);
            }

            return CreatedAtRoute(
                Routes.GetAttachment,
                new { volume, documentId, attachmentName },
                attachmentName
            );
        }

        /// <summary>
        /// Deletes the attachment (file) from document with the specified id
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="attachmentName">attachment name</param>
        /// <response code="204">Attachment was deleted successfully</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="404">Attachment with requested name not found</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpDelete("attachment/{attachmentName}")]
        public IActionResult DeleteAttachment(string volume, ulong documentId, string attachmentName)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            DocIdentity identity = new DocIdentity(documentId);
            session.DeleteAttachment(volume, identity, attachmentName);
            return NoContent();
        }

        /// <summary>
        /// Deletes a list of attachments of a document specified with selection
        /// </summary>
        /// <param name="volume">Volume name</param>
        /// <param name="documentId">Document id</param>
        /// <param name="range">Attachments selection</param>
        /// <response code="204">Attachments were deleted successfully</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="403">Documents selection is null or empty</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpDelete]
        public IActionResult DeleteAttachments(string volume, ulong documentId, [FromBody] AttachmentsRange range)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            if (!session.IsVolumeExist(volume))
            {
                return NotFound(Resources.ErrorVolumeNotFound);
            }

            AttachmentsSelection attSelection = new AttachmentsSelection(
                documentId,
                range.Attachments,
                range.ExcludeMode
            );

            if (!attSelection.IsValid())
            {
                return BadRequest("Selection parameter is not valid");
            }

            session.DeleteSelection(volume, attSelection);

            return Ok();
        }
    }
}