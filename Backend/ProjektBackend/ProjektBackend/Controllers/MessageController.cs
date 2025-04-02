using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System;
using System.Security.Claims;

#pragma warning disable

namespace ProjektBackend.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ProjektContext _context;

        public MessageController(ProjektContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("getMessages")]
        public async Task<ActionResult> GetMessages()
        {
            try
            {

                var messages = await _context.Messages.ToListAsync();

                if (messages != null && messages.Count > 0)
                {
                    return StatusCode(200, messages);
                }
                return StatusCode(404, "No messages found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching messages.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpGet("getMessage")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                    {
                        return StatusCode(401, "User ID not found in token.");
                    }
                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var messages = await _context.Messages
                    .Where(x => x.ReceiverId == targetUserId)
                    .ToListAsync();

                if (messages != null && messages.Any())
                {
                    return StatusCode(200, messages);
                }
                return StatusCode(404, "No messages can be found for this user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the messages.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPost("postMessage")]
        public async Task<ActionResult> SendMessage(CreateMessageDto createMessageDto, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                    {
                        return StatusCode(401, "User ID not found in token.");
                    }
                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var newMessage = new Message
                {
                    SenderId = targetUserId,
                    ReceiverId = createMessageDto.ReceiverId,
                    Content = createMessageDto.Content,
                    SentAt = DateTime.Now
                };

                if (newMessage != null)
                {
                    _context.Add(newMessage);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Message sent successfully.");
                }
                return StatusCode(400, "Invalid data.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create message. It may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the message.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpDelete("deleteMessage")]
        public async Task<ActionResult> DeleteMessage(int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                    {
                        return StatusCode(401, "User ID not found in token.");
                    }
                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var message = await _context.Messages.FirstOrDefaultAsync(x => x.ReceiverId == targetUserId);

                if (message != null)
                {
                    _context.Remove(message);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Message successfully deleted.");
                }
                return StatusCode(404, "No Message can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete message due to database constraints.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the message.");
            }
        }
    }
}
