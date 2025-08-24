using CreditBank.Contracts;
using CreditBank.Database;
using Microsoft.AspNetCore.Mvc;

namespace CreditBank.Controllers;

[ApiController]
[Route("[controller]")]
public class CreditsController(CreditDbContext context) : ControllerBase
{
    //Gets all credit requests, with optional filtering by status and type
    // Returns 404 if no credit requests are found

    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(IEnumerable<CreditRequestContract>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<CreditRequestContract>> GetAllAvailableCredits([FromQuery] CreditStatusEnum? status, [FromQuery] CreditTypeEnum? type)
    {
        var creditRequestsQuery = context.CreditRequests.AsQueryable();

        if (status.HasValue)
        {
            creditRequestsQuery = creditRequestsQuery.Where(cr => cr.Status == status.Value);
        }

        if (type.HasValue)
        {
            creditRequestsQuery = creditRequestsQuery.Where(cr => cr.CreditType == type.Value);
        }

        var creditRequests = creditRequestsQuery.ToList();

        if (!creditRequests.Any())
        {
            return NotFound(new { Message = "No credit requests found" });
        }

        var creditRequestContracts = creditRequests.Select(creditRequest => new CreditRequestContract
        {
            FullName = creditRequest.FullName,
            Email = creditRequest.Email,
            CreditAmount = creditRequest.CreditAmount,
            TypeEnum = creditRequest.CreditType,
            MonthlyIncome = creditRequest.MonthlyIncome
        }).ToList();

        return Ok(creditRequestContracts);
    }

    [HttpGet]
    [Route("{creditRequestId}")]
    [ProducesResponseType(typeof(CreditRequestContract), StatusCodes.Status200OK)]
    public ActionResult<CreditRequestContract> GetCredit([FromRoute] Guid creditRequestId)
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
            TypeEnum = creditRequest.CreditType,
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
        return CreatedAtAction(nameof(GetCredit), request);
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