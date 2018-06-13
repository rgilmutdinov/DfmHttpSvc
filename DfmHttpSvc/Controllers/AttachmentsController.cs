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
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("{attachmentName}", Name = Routes.GetAttachment)]
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
        public async Task<IActionResult> AddAttachment(string volume, ulong documentId, IFormFile file, [FromForm] string attachmentName = "")
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
        [HttpDelete("{attachmentName}")]
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
    }
}