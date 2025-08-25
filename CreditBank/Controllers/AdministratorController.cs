using CreditBank.Contracts;
using CreditBank.Database;
using Microsoft.AspNetCore.Mvc;

namespace CreditBank.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministratorController(CreditDbContext context) : ControllerBase
{
    [HttpPatch]
    [Route("{userId}/approve/{creditRequestId}")]
    public async Task<ActionResult> ApproveCreditRequestAsync([FromRoute] Guid userId, [FromRoute] Guid creditRequestId)
    {
        if (creditRequestId == Guid.Empty)
        {
            return BadRequest(new { Message = "Invalid credit request ID." });
        }
        try
        {
            var user = await context.Users.FindAsync(userId).ConfigureAwait(false);

            if (user == null || user.Role != UserRoles.Administrator)
            {
                return Unauthorized(new { Message = "User is not authorized to approve credit requests." });
            }

            var creditRequest = await context.CreditRequests.FindAsync(creditRequestId).ConfigureAwait(false);
            if (creditRequest == null)
            {
                return NotFound($"Could not find a credit request with Id: {creditRequestId}");
            }

            if (creditRequest.Status != CreditStatusEnum.PendingReview)
            {
                return BadRequest(new { Message = $"Credit request {creditRequestId} is not pending review." });
            }

            if (creditRequest.CreditAmount > creditRequest.MonthlyIncome * 20)
            {
                creditRequest.Status = CreditStatusEnum.Canceled;
                creditRequest.AdministratorGuid = userId;
                creditRequest.DateOfApproval = DateTime.UtcNow;
                context.CreditRequests.Update(creditRequest);
                await context.SaveChangesAsync().ConfigureAwait(false);
                return BadRequest(new { Message = $"Credit request {creditRequestId} was denied due to insufficient monthly income." });
            }

            creditRequest.Status = CreditStatusEnum.Ongoing;
            creditRequest.AdministratorGuid = userId;
            creditRequest.DateOfApproval = DateTime.UtcNow;

            context.CreditRequests.Update(creditRequest);
            await context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new { Message = $"Credit request {creditRequestId} was successfully approved." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = $"Credit request {creditRequestId} could not be approved.", Error = ex.Message });
        }
    }
}