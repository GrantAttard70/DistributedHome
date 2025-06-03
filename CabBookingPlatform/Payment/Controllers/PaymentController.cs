using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentDbContext _context;
    private readonly ICabFareService _cabFareService;
    private readonly IPricingCalculator _pricingCalculator;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        PaymentDbContext context,
        ICabFareService cabFareService,
        IPricingCalculator pricingCalculator,
        ILogger<PaymentController> logger)
    {
        _context = context;
        _cabFareService = cabFareService;
        _pricingCalculator = pricingCalculator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto dto)
    {
        if (dto.Passengers < 1 || dto.Passengers > 8)
            return BadRequest("Passengers must be between 1 and 8.");

        try
        {
            var cabFare = await _cabFareService.GetBaseFareAsync();
            decimal discountMultiplier = dto.Discount > 0 ? dto.Discount : 1m;

            var totalPrice = _pricingCalculator.CalculateTotalPrice(
                cabFare,
                dto.CabType,
                dto.TripDateTime,
                dto.Passengers,
                discountMultiplier);

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                CabType = dto.CabType,
                Passengers = dto.Passengers,
                TripDateTime = dto.TripDateTime,
                TotalPrice = totalPrice,
                Discount = discountMultiplier,
                PaymentStatus = "Paid"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var audit = new PaymentAudit
            {
                PaymentId = payment.Id,
                Action = "PAYMENT_CREATED",
                Details = $"Payment created for booking {payment.BookingId}, Amount: {payment.TotalPrice}",
                Amount = payment.TotalPrice,
                UserId = "system"
            };
            _context.PaymentAudits.Add(audit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for booking {BookingId}", dto.BookingId);
            return StatusCode(500, "An error occurred while processing the payment.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return NotFound();
        return Ok(payment);
    }

}
