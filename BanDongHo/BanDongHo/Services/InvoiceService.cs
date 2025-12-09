using AutoMapper;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories;

namespace WatchAPI.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IUnitOfWork uow, IMapper mapper, ILogger<InvoiceService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllAsync()
        {
            _logger.LogInformation("Admin requested all invoices");
            var invoices = await _uow.Invoices.GetAllWithDetailAsync();
            _logger.LogInformation("Returned {Count} invoice for admin", invoices.Count());
            return _mapper.Map<IEnumerable<InvoiceDTO>>(invoices);
        }

        public async Task<InvoiceDTO?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Admin requested invoice by ID: {InvoiceId}", id);
            var invoice = await _uow.Invoices.GetDetailAsync(id);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found for admin", id);
                return null;
            }

            _logger.LogInformation("Returned invoice with ID {Id} for admin", id);
            return _mapper.Map<InvoiceDTO?>(invoice);
        }


        public async Task<InvoiceDTO> CreateAsync(InvoiceDTO dto, string? user = null)
        {
            await ValidateInvoiceCreateAsync(dto);

            var invoice = new Invoice
            {
                UserId = dto.UserId
            };

            foreach (var d in dto.Details)
            {
                var watch = await _uow.Watches.GetByIdAsync(d.WatchId) ?? throw new InvalidOperationException($"Watch with ID {d.WatchId} not found.");
                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    WatchId = d.WatchId,
                    Quantity = d.Quantity,
                    Price = watch.Price
                });
            }

            invoice.SetCreated(user);

            await _uow.Invoices.CreateAsync(invoice, user);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Created invoice {Id} by user {UserId}", invoice.Id, invoice.UserId);

            return _mapper.Map<InvoiceDTO>(invoice);
        }

        private async Task ValidateInvoiceCreateAsync(InvoiceDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserId))
                throw new ArgumentException("UserId is required.");

            if (dto.Details == null || !dto.Details.Any())
                throw new ArgumentException("Invoice must contain at least one detail.");

            foreach (var d in dto.Details)
            {
                if (d.WatchId == Guid.Empty)
                    throw new ArgumentException("WatchId is required in invoice detail.");

                if (d.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero.");

                var watch = await _uow.Watches.GetByIdAsync(d.WatchId);
                if (watch == null)
                    throw new InvalidOperationException($"Watch with ID {d.WatchId} not found.");

                if (d.Price != 0)
                    throw new InvalidOperationException("Price cannot be provided by the client.");
            }

            if (dto.Id != Guid.Empty)
                throw new ArgumentException("Invoice ID must not be included when creating.");

            if (dto.Details.Any(d => d.Id != Guid.Empty))
                throw new ArgumentException("InvoiceDetail IDs must not be included when creating.");
        }

    }
}
