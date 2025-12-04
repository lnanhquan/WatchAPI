using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories;

namespace WatchAPI.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repo;
        private readonly IWatchRepository _watchRepo;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository repo, ILogger<InvoiceService> logger, IWatchRepository watchRepo)
        {
            _repo = repo;
            _logger = logger;
            _watchRepo = watchRepo;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllAsync()
        {
            try
            {
                var invoices = await _repo.GetAllAsync();
                _logger.LogInformation("Retrieved all invoices.");

                return invoices.Select(i => MapToDTO(i)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving all invoices: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<InvoiceDTO?> GetByIdAsync(Guid id)
        {
            try
            {
                var invoice = await _repo.GetByIdAsync(id);
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice with ID {Id} not found", id);
                    return null;
                }

                _logger.LogInformation("Retrieved invoice with ID {Id}", id);
                return MapToDTO(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving invoice with ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<InvoiceDTO> CreateAsync(InvoiceDTO dto)
        {
            try
            {
                var invoice = new Invoice
                {
                    UserId = dto.UserId,
                    //CreatedAt = DateTime.UtcNow
                };

                invoice.InvoiceDetails = new List<InvoiceDetail>();

                foreach (var d in dto.Details)
                {
                    var watch = await _watchRepo.GetByIdAsync(d.WatchId);
                    if (watch == null)
                        throw new InvalidOperationException($"Watch with ID {d.WatchId} not found.");

                    invoice.InvoiceDetails.Add(new InvoiceDetail
                    {
                        WatchId = d.WatchId,
                        Quantity = d.Quantity,
                        Price = watch.Price
                    });
                }

                var created = await _repo.CreateAsync(invoice);
                _logger.LogInformation("Created invoice {Id} for user {UserId}", created.Id, created.UserId);

                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating invoice for user {UserId}: {Message}", dto.UserId, ex.Message);
                throw;
            }
        }

        public async Task<InvoiceDTO?> UpdateAsync(Guid id, InvoiceDTO dto)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Update failed. Invoice with ID {Id} not found", id);
                    return null;
                }

                existing.UserId = dto.UserId;

                // Tạo dictionary để lọc detail cũ và mới
                var dtoDetails = dto.Details.ToDictionary(x => x.WatchId);

                var toRemove = existing.InvoiceDetails
                    .Where(d => !dtoDetails.ContainsKey(d.WatchId))
                    .ToList();

                foreach (var r in toRemove)
                    existing.InvoiceDetails.Remove(r);

                foreach (var d in dto.Details)
                {
                    var watch = await _watchRepo.GetByIdAsync(d.WatchId);
                    if (watch == null)
                        throw new InvalidOperationException($"Watch with ID {d.WatchId} not found.");

                    var existingDetail = existing.InvoiceDetails
                        .FirstOrDefault(x => x.WatchId == d.WatchId);

                    if (existingDetail != null)
                    {
                        // Update
                        existingDetail.Quantity = d.Quantity;
                        existingDetail.Price = watch.Price;
                    }
                    else
                    {
                        // Add
                        existing.InvoiceDetails.Add(new InvoiceDetail
                        {
                            WatchId = d.WatchId,
                            Quantity = d.Quantity,
                            Price = watch.Price
                        });
                    }
                }

                await _repo.UpdateAsync(existing);
                _logger.LogInformation("Updated invoice {Id} for user {UserId}", id, dto.UserId);

                return MapToDTO(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating invoice {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Delete failed. Invoice with ID {Id} not found", id);
                    return false;
                }

                await _repo.DeleteAsync(id);
                _logger.LogInformation("Deleted invoice {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting invoice {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        private InvoiceDTO MapToDTO(Invoice invoice)
        {
            return new InvoiceDTO
            {
                Id = invoice.Id,
                UserId = invoice.UserId,
                UserEmail = invoice.User?.Email ?? string.Empty,
                CreatedAt = invoice.CreatedAt,
                Details = invoice.InvoiceDetails?.Select(d => new InvoiceDetailDTO
                {
                    Id = d.Id,
                    InvoiceId = d.InvoiceId,
                    WatchId = d.WatchId,
                    WatchName = d.Watch?.Name ?? string.Empty,
                    ImageUrl = d.Watch?.ImageUrl,
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList() ?? new List<InvoiceDetailDTO>()
            };
        }
    }
}
