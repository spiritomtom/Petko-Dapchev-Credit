using CreditBank.Contracts;
using CreditBank.Contracts.Enums;
using CreditBank.Database;
using Microsoft.AspNetCore.Mvc;

namespace CreditBank.Controllers;

[ApiController]
[Route("[controller]")]
public class CreditsController(CreditDbContext context) : ControllerBase
{

    [HttpGet]
    [Route("{creditRequestId}")]
    [ProducesResponseType(typeof(CreditRequestContract), StatusCodes.Status200OK)]
    public ActionResult<CreditRequestContract> GetAvailableCredits([FromRoute] Guid creditRequestId)
    {
        var creditRequest = context.CreditRequests.Find(creditRequestId);
        if (creditRequest == null)
        {
            return NotFound(new { Message = "Credit request not found" });
        }
        var creditRequestContract = new CreditRequestContract
        {
            FullName = creditRequest.FullName,
            Email = creditRequest.Email,
            CreditAmount = creditRequest.CreditAmount,
            TypeEnum = creditRequest.TypeEnum,
            MonthlyIncome = creditRequest.MonthlyIncome
        };

        return Ok(creditRequestContract);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [HttpPost]
    [Route("apply")]
    public IActionResult ApplyForCredit([FromBody] CreditRequestContract request)
    {
        context.AddCreditRequest(request);
        return CreatedAtAction(nameof(GetAvailableCredits), request);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch]
    [Route("{creditRequestId}/payment")]
    public IActionResult MakePayment([FromRoute] Guid creditId, [FromBody] PaymentContract payment)
    {
        var credit = context.Credits.Find(creditId);
        if (credit == null)
        {
            return NotFound(new { Message = $"Credit with id: {creditId} was not found" });
        }

        if(credit.Status is CreditStatusEnum.Finished or CreditStatusEnum.Canceled)
        {
            return BadRequest(new { Message = $"Credit: {creditId} is currently not active." });
        }

        credit.Amount -= payment.Amount;
        if (credit.Amount <= 0)
        {
            credit.Status = CreditStatusEnum.Finished;
            credit.EndDate = DateTime.UtcNow;
        }

        context.Credits.Update(credit);
        return Ok(new { Message = $"Payment was processed successfully for credit: {creditId}" });
    }
}