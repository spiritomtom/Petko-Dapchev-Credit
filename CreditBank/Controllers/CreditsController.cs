using CreditBank.Contracts;
using CreditBank.Database;
using Microsoft.AspNetCore.Mvc;

namespace CreditBank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditsController : ControllerBase
    {
        private readonly CreditDbContext _context;

        public CreditsController(CreditDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("apply")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult ApplyForCredit([FromBody] CreditRequestContract request)
        {
            var credit = _context.AddCreditRequest(request);
            return CreatedAtAction(nameof(GetCredit), new { creditRequestId = credit.Id }, request);
        }

        [HttpGet]
        [Route("{creditRequestId}")]
        [ProducesResponseType(typeof(CreditRequestContract), StatusCodes.Status200OK)]
        public ActionResult<CreditRequestContract> GetCredit([FromRoute] Guid creditRequestId)
        {
            var creditRequest = _context.CreditRequests.Find(creditRequestId);
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
    }
}