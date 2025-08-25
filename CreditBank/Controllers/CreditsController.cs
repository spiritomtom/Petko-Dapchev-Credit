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

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<CreditRequestContract>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<CreditRequestContract>> GetAllAvailableCredits([FromQuery] CreditStatusEnum? status, [FromQuery] CreditTypeEnum? type)
        {
            var creditRequestsQuery = _context.CreditRequests.AsQueryable();

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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("{creditId}/payment")]
        public IActionResult MakePayment([FromRoute] Guid creditId, [FromBody] PaymentContract payment)
        {
            _context.MakePayment(creditId, payment);
            return Ok(new { Message = $"Payment was processed successfully for credit: {creditId}" });
        }
    }
}