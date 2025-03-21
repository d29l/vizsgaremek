using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Connection = ProjektBackend.Models.Connection;

#pragma warning disable CS0168

namespace ProjektBackend.Controllers
{
    [Route("api/connections")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly ProjektContext _context;

        public ConnectionController(ProjektContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchConnections")]
        public async Task<ActionResult<Connection>> FetchConnections()
        {
            try
            {
                var connections = await _context.Connections.ToListAsync();

                if (connections != null && connections.Any())
                {
                    return StatusCode(200, connections);
                }
                return StatusCode(404, "There are currently no Connections.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching connections.");
            }
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpGet("fetchConnections/{ReceiverId}")]
        public async Task<ActionResult<IEnumerable<Connection>>> FetchConnections(int ReceiverId, int UserId)
        {
            try
            {
                if (ReceiverId != UserId)
                {
                    return StatusCode(403, "You dont have access to this connection.");
                }

                var connection = await _context.Connections.Where(x => x.ReceiverId == ReceiverId).ToListAsync();

                if (connection != null && connection.Any())
                {
                    return StatusCode(200, connection);
                }
                return StatusCode(404, "No Connection can be found with this Id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching connections.");
            }
        }

        [Authorize(Policy = "EmployeeSelfOnly")]
        [HttpPost("postNewConnection")]
        public async Task<ActionResult> PostNewConnection(int UserId, int RequesterId, CreateConnectionDto createConnectionDto)
        {
            try
            {
                var newConnection = new Connection
                {
                    RequesterId = RequesterId,
                    ReceiverId = createConnectionDto.ReceiverId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                };
                if (newConnection != null)
                {
                    _context.Add(newConnection);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Request created successfully.");
                }
                return StatusCode(400, "Invalid data.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create connection. It may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the connection.");
            }
        }

        [Authorize(Policy = "EmployerSelfOnly")]
        [HttpPut("editConnection/{ReceiverId}")]
        public async Task<ActionResult> UpdateConnection(int UserId, int ReceiverId, int RequesterId, UpdateConnectionDto updateConnectionDto)
        {
            try
            {
                var existingConnection = await _context.Connections.FirstOrDefaultAsync(x => x.ReceiverId == ReceiverId && x.RequesterId == RequesterId);

                if (existingConnection != null)
                {
                    existingConnection.Status = updateConnectionDto.Status;

                    _context.Update(existingConnection);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, $"Request status has been changed to: {existingConnection.Status}");
                }
                return StatusCode(404, "No Connection can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update connection.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the connection.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpDelete("deleteConnection/{RequesterId}/{ReceiverId}")]
        public async Task<ActionResult> DeleteConnection(int UserId, int ReceiverId, int RequesterId)
        {
            try
            {
                var connection = await _context.Connections.FirstOrDefaultAsync(x => x.ReceiverId == ReceiverId && x.RequesterId == RequesterId);

                if (connection != null)
                {
                    _context.Remove(connection);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Connection successfully deleted.");
                }
                return StatusCode(404, "No Connection can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete connection.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the connection.");
            }
        }
    }
}
