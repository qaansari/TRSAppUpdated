using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReservationService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Reservation";

        public ReservationService(
            AppDbContext context,
            ILogger<ReservationService> logger,
            IWebHostEnvironment webHostEnvironment,
            ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _cacheService = cacheService;
        }

        public ReservationDetailVM GetDetailsByID(int ID)
        {
            return GetDetailsByIDAsync(ID).GetAwaiter().GetResult();
        }

        public async Task<ReservationDetailVM> GetDetailsByIDAsync(int ID)
        {
            try
            {
                var cacheKey = CacheKeys.ReservationDetails(ID);
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var r = await _context.Reservations
                        .Include(x => x.Show).ThenInclude(s => s.Event)
                        .Include(x => x.PaymentMode)
                        .Include(x => x.ReservationStatus)
                        .Include(x => x.ClosedByUser)
                        .Include(x => x.ReservationImages)
                        .Include(x => x.ReservationTickets)
                            .ThenInclude(rt => rt.Ticket)
                            .ThenInclude(t => t.Seat)
                            .ThenInclude(s => s.Column)
                            .ThenInclude(c => c.Row)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == ID);

                    if (r == null)
                    {
                        return new ReservationDetailVM();
                    }

                    var firstTicket = r.ReservationTickets?.FirstOrDefault()?.Ticket;
                    var seatList = r.ReservationTickets?
                        .Where(rt => rt.Ticket?.Seat?.Column?.Row != null)
                        .Select(rt => $"{rt.Ticket.Seat.Column.Row.RowLabel}{rt.Ticket.Seat.SeatNumber}")
                        .ToList() ?? new List<string>();

                    var imageList = r.ReservationImages?
                        .Select(ri => ri.ImagePath)
                        .Where(p => !string.IsNullOrEmpty(p))
                        .ToList() ?? new List<string>();

                    string showDateTimeStr = string.Empty;
                    if (r.Show != null)
                    {
                        showDateTimeStr = r.Show.Date.ToString("dddd, dd MMMM yyyy") + " "
                            + DateTime.Today.Add(r.Show.Time).ToString("hh:mm tt");
                    }

                    return new ReservationDetailVM
                    {
                        ID = r.ID,
                        EventName = r.Show?.Event?.Title ?? string.Empty,
                        TicketNumber = r.TicketNumber,
                        CustomerName = r.CustomerName,
                        CustomerPhone = r.CustomerPhone,
                        ShowDateTime = showDateTimeStr,
                        PaymentModeTitle = r.PaymentMode?.Title ?? string.Empty,
                        ReservationStatusTitle = r.ReservationStatus?.Title ?? string.Empty,
                        EventImagePath = r.Show?.Event?.ImagePath ?? string.Empty,
                        IsFullyPaid = r.IsFullyPaid,
                        TicketCategory = firstTicket?.Category ?? string.Empty,
                        CostPrice = r.CostPrice,
                        Discount = r.Discount,
                        SalePrice = r.SalePrice,
                        AmountPaid = r.AmountPaid,
                        ReservationImages = string.Join(",", imageList),
                        AmountRemaining = r.AmountRemaining,
                        BookingDateTime = r.BookingDateTime,
                        TransactionID = r.TransactionID,
                        CreatedByUserID = r.CreatedByUserID,
                        CreatedDateTime = r.CreatedDateTime,
                        UpdatedDateTime = r.UpdatedDateTime,
                        ClosedByUserName = r.ClosedByUser != null ? r.ClosedByUser.FullName : string.Empty,
                        ClosedDateTime = r.ClosingDateTime.HasValue ? r.ClosingDateTime.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") : string.Empty,
                        SeatLabels = string.Join(",", seatList)
                    };
                }, TimeSpan.FromMinutes(15), TimeSpan.FromHours(1)) ?? new ReservationDetailVM();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at GetDetailsByIDAsync in {Title} Service", _Title);
            }
            return new ReservationDetailVM();
        }

        public async Task<ReservationVM?> GetForEditAsync(int id, int eventId)
        {
            try
            {
                var item = await _context.Reservations
                    .Include(x => x.ReservationTickets)
                    .Include(x => x.Show)
                    .Where(x => !x.IsDeleted && x.ID == id)
                    .Select(x => new ReservationVM
                    {
                        ID = x.ID,
                        TicketNumber = x.TicketNumber,
                        EventID = eventId != 0 ? eventId : (x.Show != null ? x.Show.EventID : 0),
                        CustomerName = x.CustomerName,
                        CustomerPhone = x.CustomerPhone,
                        TicketIDs = x.ReservationTickets.Select(rt => rt.TicketID).ToList(),
                        CostPrice = x.CostPrice,
                        Discount = x.Discount,
                        SalePrice = x.SalePrice,
                        AmountPaid = x.AmountPaid,
                        AmountRemaining = x.AmountRemaining,
                        IsFullyPaid = x.IsFullyPaid,
                        IsApproved = x.IsApproved,
                        ActionByUserID = x.ActionByUserID,
                        ActionDateTime = x.ActionDateTime,
                        ClosedByUserID = x.ClosedByUserID,
                        ClosingDateTime = x.ClosingDateTime,
                        TransactionID = x.TransactionID,
                        BookingDateTime = x.BookingDateTime,
                        ShowID = x.ShowID,
                        PaymentModeID = x.PaymentModeID,
                        ReservationStatusID = x.ReservationStatusID,
                        CreatedByUserID = x.CreatedByUserID,
                        CreatedDateTime = x.CreatedDateTime,
                    })
                    .FirstOrDefaultAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at GetForEditAsync for reservation ID {ID}", id);
                return null;
            }
        }

        public async Task<(bool Success, string Message, int EventId)> SaveReservationAsync(ReservationVM model, int currentUserId, bool isSuperAdmin)
        {
            if (model == null)
            {
                return (false, "Invalid reservation data", 0);
            }

            // Resolve EventID if not set
            if (model.EventID == 0 && model.ShowID > 0)
            {
                model.EventID = await _context.Shows
                    .Where(s => s.ID == model.ShowID)
                    .Select(s => s.EventID)
                    .FirstOrDefaultAsync();
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!AppHelper.IsValidImageFile(model.ImageFile, out var imgError))
                {
                    return (false, imgError ?? "Invalid image file format", model.EventID);
                }
            }

            if (model.TicketIDs == null || model.TicketIDs.Count == 0)
            {
                return (false, "Please select at least one ticket", model.EventID);
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync<(bool Success, string Message, int EventId)>(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var now = DateTime.UtcNow.AddHours(5);

                    // Financial calculations
                    model.CustomerName = model.CustomerName?.Trim() ?? string.Empty;
                    model.CustomerPhone = model.CustomerPhone?.Trim() ?? string.Empty;
                    model.TransactionID = string.IsNullOrWhiteSpace(model.TransactionID) ? null : model.TransactionID.Trim();
                    model.Discount = Math.Max(0, Math.Round(model.CostPrice - model.SalePrice, 2));
                    model.AmountRemaining = Math.Max(0, Math.Round(model.SalePrice - model.AmountPaid, 2));
                    model.IsFullyPaid = model.AmountRemaining == 0;

                    var reservationStatus = await _context.ReservationStatuses
                        .Where(x => !x.IsDeleted && x.ID == model.ReservationStatusID)
                        .Select(x => new { x.ID, x.Title })
                        .FirstOrDefaultAsync();

                    var statusTitle = reservationStatus?.Title?.Trim().ToLower() ?? string.Empty;

                    // Validate ticket conflict with other active reservations
                    var conflictingTicket = await _context.ReservationTickets
                        .Where(rt => rt.ReservationID != model.ID
                                     && !rt.Reservation.IsDeleted
                                     && rt.Reservation.ReservationStatusID != 2 // Cancelled
                                     && model.TicketIDs.Contains(rt.TicketID))
                        .Select(rt => rt.TicketID)
                        .FirstOrDefaultAsync();

                    if (conflictingTicket > 0)
                    {
                        return (false, "One or more selected tickets are already reserved in another active reservation.", model.EventID);
                    }

                    Reservation reservation;

                    if (model.ID != 0)
                    {
                        // Update existing reservation
                        reservation = await _context.Reservations
                            .Include(r => r.ReservationTickets)
                            .AsTracking()
                            .FirstOrDefaultAsync(r => !r.IsDeleted && r.ID == model.ID);

                        if (reservation == null)
                        {
                            return (false, "Reservation not found.", model.EventID);
                        }

                        reservation.CustomerName = model.CustomerName;
                        reservation.CustomerPhone = model.CustomerPhone;
                        reservation.CostPrice = model.CostPrice;
                        reservation.Discount = model.Discount;
                        reservation.SalePrice = model.SalePrice;
                        reservation.AmountPaid = model.AmountPaid;
                        reservation.AmountRemaining = model.AmountRemaining;
                        reservation.IsFullyPaid = model.IsFullyPaid;
                        reservation.TransactionID = model.TransactionID;
                        if (model.BookingDateTime.HasValue)
                        {
                            reservation.BookingDateTime = model.BookingDateTime.Value;
                        }
                        reservation.ShowID = model.ShowID;
                        reservation.PaymentModeID = model.PaymentModeID;
                        reservation.ReservationStatusID = model.ReservationStatusID;
                        reservation.UpdatedByUserID = currentUserId;
                        reservation.UpdatedDateTime = now;

                        if (isSuperAdmin && model.IsApproved.HasValue)
                        {
                            reservation.IsApproved = model.IsApproved;
                            if (reservation.IsApproved.Value)
                            {
                                reservation.ActionByUserID = model.ActionByUserID ?? currentUserId;
                                reservation.ActionDateTime = model.ActionDateTime ?? now;
                            }
                        }

                        if (statusTitle == "closed")
                        {
                            reservation.ClosedByUserID = reservation.ClosedByUserID ?? currentUserId;
                            reservation.ClosingDateTime = reservation.ClosingDateTime ?? now;
                        }

                        // Synchronize ReservationTickets
                        var currentTicketIDs = reservation.ReservationTickets.Select(rt => rt.TicketID).ToHashSet();
                        var newTicketIDs = model.TicketIDs.ToHashSet();

                        var ticketsToRemove = reservation.ReservationTickets.Where(rt => !newTicketIDs.Contains(rt.TicketID)).ToList();
                        foreach (var rt in ticketsToRemove)
                        {
                            _context.ReservationTickets.Remove(rt);
                        }

                        var ticketsToAdd = newTicketIDs.Where(id => !currentTicketIDs.Contains(id)).ToList();
                        foreach (var ticketId in ticketsToAdd)
                        {
                            reservation.ReservationTickets.Add(new ReservationTicket
                            {
                                ReservationID = reservation.ID,
                                TicketID = ticketId,
                                CreatedByUserID = currentUserId,
                                CreatedDateTime = now
                            });
                        }

                        await _context.SaveChangesAsync();

                        // Reset tickets that were removed
                        var removedTicketIDs = ticketsToRemove.Select(rt => rt.TicketID).ToList();
                        if (removedTicketIDs.Count > 0)
                        {
                            await _context.Tickets
                                .Where(t => !t.IsDeleted && removedTicketIDs.Contains(t.ID))
                                .ExecuteUpdateAsync(s => s
                                    .SetProperty(t => t.IsReserved, false)
                                    .SetProperty(t => t.ReservationDateTime, (DateTime?)null)
                                    .SetProperty(t => t.SalePrice, 0)
                                    .SetProperty(t => t.Discount, 0)
                                    .SetProperty(t => t.UpdatedByUserID, currentUserId)
                                    .SetProperty(t => t.UpdatedDateTime, now));
                        }
                    }
                    else
                    {
                        // Add new reservation
                        reservation = new Reservation
                        {
                            TicketNumber = string.IsNullOrWhiteSpace(model.TicketNumber)
                                ? Guid.NewGuid().ToString().Substring(0, 7).ToUpper()
                                : model.TicketNumber.Trim(),
                            CustomerName = model.CustomerName,
                            CustomerPhone = model.CustomerPhone,
                            CostPrice = model.CostPrice,
                            Discount = model.Discount,
                            SalePrice = model.SalePrice,
                            AmountPaid = model.AmountPaid,
                            AmountRemaining = model.AmountRemaining,
                            IsFullyPaid = model.IsFullyPaid,
                            TransactionID = model.TransactionID,
                            BookingDateTime = model.BookingDateTime ?? now,
                            ShowID = model.ShowID,
                            PaymentModeID = model.PaymentModeID,
                            ReservationStatusID = model.ReservationStatusID,
                            CreatedByUserID = currentUserId,
                            CreatedDateTime = now
                        };

                        if (isSuperAdmin && model.IsApproved.HasValue)
                        {
                            reservation.IsApproved = model.IsApproved;
                            if (reservation.IsApproved.Value)
                            {
                                reservation.ActionByUserID = model.ActionByUserID ?? currentUserId;
                                reservation.ActionDateTime = model.ActionDateTime ?? now;
                            }
                        }

                        if (statusTitle == "closed")
                        {
                            reservation.ClosedByUserID = currentUserId;
                            reservation.ClosingDateTime = now;
                        }

                        _context.Reservations.Add(reservation);
                        await _context.SaveChangesAsync();

                        foreach (var ticketId in model.TicketIDs)
                        {
                            _context.ReservationTickets.Add(new ReservationTicket
                            {
                                ReservationID = reservation.ID,
                                TicketID = ticketId,
                                CreatedByUserID = currentUserId,
                                CreatedDateTime = now
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Update ticket statuses
                    double costPerTicket = model.TicketIDs.Count > 0 ? (double)reservation.CostPrice / model.TicketIDs.Count : 0.0;
                    double salePricePerTicket = model.TicketIDs.Count > 0 ? (double)reservation.SalePrice / model.TicketIDs.Count : 0.0;
                    double discountPerTicket = model.TicketIDs.Count > 0 ? (double)reservation.Discount / model.TicketIDs.Count : 0.0;

                    if (statusTitle == "cancelled")
                    {
                        await _context.Tickets
                            .Where(t => !t.IsDeleted && model.TicketIDs.Contains(t.ID))
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(t => t.IsReserved, false)
                                .SetProperty(t => t.IsCancelled, true)
                                .SetProperty(t => t.CancellationDateTime, now)
                                .SetProperty(t => t.ReservationDateTime, (DateTime?)null)
                                .SetProperty(t => t.UpdatedByUserID, currentUserId)
                                .SetProperty(t => t.UpdatedDateTime, now));
                    }
                    else if (statusTitle == "refunded")
                    {
                        await _context.Tickets
                            .Where(t => !t.IsDeleted && model.TicketIDs.Contains(t.ID))
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(t => t.IsRefunded, true)
                                .SetProperty(t => t.RefundDateTime, now)
                                .SetProperty(t => t.CostPrice, costPerTicket)
                                .SetProperty(t => t.SalePrice, salePricePerTicket)
                                .SetProperty(t => t.Discount, discountPerTicket)
                                .SetProperty(t => t.UpdatedByUserID, currentUserId)
                                .SetProperty(t => t.UpdatedDateTime, now));
                    }
                    else
                    {
                        // "Arrival Pending", "Closed", or other active status
                        await _context.Tickets
                            .Where(t => !t.IsDeleted && model.TicketIDs.Contains(t.ID))
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(t => t.IsReserved, true)
                                .SetProperty(t => t.ReservationDateTime, now)
                                .SetProperty(t => t.CostPrice, costPerTicket)
                                .SetProperty(t => t.SalePrice, salePricePerTicket)
                                .SetProperty(t => t.Discount, discountPerTicket)
                                .SetProperty(t => t.UpdatedByUserID, currentUserId)
                                .SetProperty(t => t.UpdatedDateTime, now));
                    }

                    // Handle payment slip image upload if provided
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var eventTitle = await _context.Events
                            .Where(e => !e.IsDeleted && e.ID == model.EventID)
                            .Select(e => e.Title)
                            .FirstOrDefaultAsync();

                        var eventFolder = !string.IsNullOrEmpty(eventTitle) ? eventTitle.Replace(" ", "") : "default";
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "reservations", eventFolder);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var baseFileName = $"payment_slip_{reservation.TicketNumber}";
                        if (!string.IsNullOrEmpty(statusTitle))
                        {
                            baseFileName = $"payment_slip_{reservation.TicketNumber}_{statusTitle.Replace(" ", "")}";
                        }
                        var ext = Path.GetExtension(model.ImageFile.FileName);
                        var fileName = baseFileName + ext;
                        var filePath = Path.Combine(folderPath, fileName);
                        int counter = 1;
                        while (File.Exists(filePath))
                        {
                            fileName = $"{baseFileName}_{counter}{ext}";
                            filePath = Path.Combine(folderPath, fileName);
                            counter++;
                        }

                        var compressedBytes = await AppHelper.CompressImageAsync(model.ImageFile);
                        await File.WriteAllBytesAsync(filePath, compressedBytes);

                        _context.ReservationImages.Add(new ReservationImage
                        {
                            ReservationID = reservation.ID,
                            ImagePath = fileName,
                            CreatedByUserID = currentUserId,
                            CreatedDateTime = now
                        });
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    _cacheService.Remove(CacheKeys.ReservationDetails(reservation.ID));
                    _cacheService.Remove(CacheKeys.EventDetails(model.EventID));
                    _cacheService.Remove(CacheKeys.ViewTickets(model.ShowID, model.EventID));
                    _cacheService.RemoveByPrefix(CacheKeys.ViewTicketsPrefix);
                    _cacheService.Remove(CacheKeys.DashboardMetrics);

                    var successMsg = model.ID != 0 ? "Reservation Successfully Updated" : "Reservation Successfully Added";
                    return (true, successMsg, model.EventID);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occurred while saving reservation ID: {ID}, EventID: {EventID}", model.ID, model.EventID);
                    return (false, "An error occurred while saving the reservation: " + ex.Message, model.EventID);
                }
            });
        }

        public async Task<(bool Success, string Message)> CloseReservationAsync(int id, int eventId, int currentUserId)
        {
            try
            {
                var reservation = await _context.Reservations
                    .AsTracking()
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.ID == id);

                if (reservation == null)
                {
                    return (false, "No Reservation Found");
                }

                if (reservation.ReservationStatusID == 3)
                {
                    return (true, "Reservation is already Closed");
                }

                var now = AppHelper.NowPakistan;
                reservation.ReservationStatusID = 3; // Closed
                reservation.ClosedByUserID = currentUserId;
                reservation.ClosingDateTime = now;
                reservation.UpdatedByUserID = currentUserId;
                reservation.UpdatedDateTime = now;

                await _context.SaveChangesAsync();
                _cacheService.Remove(CacheKeys.ReservationDetails(id));
                _cacheService.Remove(CacheKeys.EventDetails(eventId));
                _cacheService.Remove(CacheKeys.DashboardMetrics);
                return (true, "Reservation Successfully Closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at CloseReservationAsync for ID {ID}", id);
                return (false, "An error occurred while closing the reservation");
            }
        }
    }
}
