using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class QuestPdfTicketService : IPdfService
    {
        private readonly ILogger<QuestPdfTicketService> _logger;

        public QuestPdfTicketService(ILogger<QuestPdfTicketService> logger)
        {
            _logger = logger;
        }

        public byte[] GenerateTicketPdf(ReservationDetailVM reservation, bool isGeneralEvent, string webRootPath)
        {
            try
            {
                // Prepare seat labels
                var seatLabelsList = !string.IsNullOrEmpty(reservation.SeatLabels)
                    ? reservation.SeatLabels.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
                    : new List<string>();

                // Logo path
                string logoPath = Path.Combine(webRootPath, "assets", "images", "web-logo-light.png");
                if (!File.Exists(logoPath))
                {
                    logoPath = Path.Combine(webRootPath, "assets", "images", "web-logo.png");
                }
                byte[]? logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : null;

                // Event image path
                byte[]? eventImageBytes = null;
                if (!string.IsNullOrEmpty(reservation.EventImagePath))
                {
                    string eventImagePath = Path.Combine(webRootPath, "assets", "images", "events", reservation.EventImagePath);
                    if (File.Exists(eventImagePath))
                    {
                        try
                        {
                            eventImageBytes = File.ReadAllBytes(eventImagePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not read event image: {ImagePath}", eventImagePath);
                        }
                    }
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(210, 80, Unit.Millimetre);
                        page.Margin(0);
                        page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(9).FontColor("#0F0F0F"));

                        page.Content().Column(col =>
                        {
                            // 1. Header Bar
                            col.Item().Background("#1AA48B").PaddingVertical(4).AlignCenter().Text(text =>
                            {
                                text.Span("E-Ticket")
                                    .FontSize(13)
                                    .Bold()
                                    .FontColor("#FFFFFF")
                                    .LetterSpacing(0.08f);
                            });

                            // 2. Ticket Body
                            col.Item().PaddingHorizontal(14).PaddingTop(4).Column(body =>
                            {
                                // Logo and Ticket Number Header Row
                                body.Item().Row(row =>
                                {
                                    if (logoBytes != null)
                                    {
                                        row.RelativeItem(1).AlignLeft().Height(28).Image(logoBytes).FitArea();
                                    }

                                    row.RelativeItem(2).AlignCenter().PaddingVertical(2).PaddingHorizontal(6)
                                        .Border(1).BorderColor("#C8C8C8")
                                        .Background("#F8F9FA")
                                        .AlignCenter()
                                        .Text(text =>
                                        {
                                            text.Span(reservation.TicketNumber ?? string.Empty)
                                                .FontSize(11)
                                                .Bold()
                                                .FontColor("#1AA48B")
                                                .LetterSpacing(0.1f);
                                        });

                                    if (logoBytes != null)
                                    {
                                        row.RelativeItem(1); // Balancer column for visual centering
                                    }
                                });

                                // Main Content (Details + Event Poster)
                                body.Item().PaddingTop(3).Row(contentRow =>
                                {
                                    // Info Column
                                    var infoColumn = eventImageBytes != null ? contentRow.RelativeItem(6) : contentRow.RelativeItem(10);

                                    infoColumn.Column(info =>
                                    {
                                        // Row: Event Name
                                        info.Item().BorderBottom(0.5f).BorderColor("#F0F0F0").PaddingVertical(1.5f).Row(r =>
                                        {
                                            r.ConstantItem(85).Text("Event Name:").SemiBold().FontColor("#282828");
                                            r.RelativeItem().Text((reservation.EventName ?? string.Empty).ToUpper()).Bold();
                                        });

                                        // Row: Show Date
                                        info.Item().BorderBottom(0.5f).BorderColor("#F0F0F0").PaddingVertical(1.5f).Row(r =>
                                        {
                                            r.ConstantItem(85).Text("Show Date:").SemiBold().FontColor("#282828");
                                            r.RelativeItem().Text((reservation.ShowDateTime ?? string.Empty).ToUpper()).Bold();
                                        });

                                        // Row: Customer Name
                                        info.Item().BorderBottom(0.5f).BorderColor("#F0F0F0").PaddingVertical(1.5f).Row(r =>
                                        {
                                            r.ConstantItem(85).Text("Customer Name:").SemiBold().FontColor("#282828");
                                            r.RelativeItem().Text((reservation.CustomerName ?? string.Empty).ToUpper()).Bold();
                                        });

                                        // Row: Phone Number
                                        info.Item().BorderBottom(0.5f).BorderColor("#F0F0F0").PaddingVertical(1.5f).Row(r =>
                                        {
                                            r.ConstantItem(85).Text("Phone Number:").SemiBold().FontColor("#282828");
                                            r.RelativeItem().Text((reservation.CustomerPhone ?? string.Empty).ToUpper()).Bold();
                                        });

                                        // Row: Reserved Seats
                                        info.Item().PaddingVertical(1.5f).Row(r =>
                                        {
                                            r.ConstantItem(85).Text($"Reserved Seats: ({seatLabelsList.Count})").SemiBold().FontColor("#282828");
                                            r.RelativeItem().Row(seatRow =>
                                            {
                                                if (!string.IsNullOrEmpty(reservation.TicketCategory))
                                                {
                                                    seatRow.AutoItem().Text($"{reservation.TicketCategory.ToUpper()} - ").SemiBold();
                                                }

                                                if (isGeneralEvent)
                                                {
                                                    seatRow.AutoItem().Background("#58FFD9").PaddingVertical(1).PaddingHorizontal(4)
                                                        .Text("First Come First Sit").FontSize(8).Bold().FontColor("#100C0C");
                                                }
                                                else if (seatLabelsList.Count > 0)
                                                {
                                                    foreach (var seat in seatLabelsList)
                                                    {
                                                        seatRow.AutoItem().PaddingRight(2).Background("#58FFD9").PaddingVertical(1).PaddingHorizontal(4)
                                                            .Text(seat).FontSize(8).Bold().FontColor("#100C0C");
                                                    }
                                                }
                                                else
                                                {
                                                    seatRow.AutoItem().Background("#58FFD9").PaddingVertical(1).PaddingHorizontal(4)
                                                        .Text("None").FontSize(8).Bold().FontColor("#100C0C");
                                                }
                                            });
                                        });
                                    });

                                    // Event Poster Image Column
                                    if (eventImageBytes != null)
                                    {
                                        contentRow.RelativeItem(4).AlignRight().PaddingLeft(8).Height(95)
                                            .Border(0.5f).BorderColor("#EAEAEA")
                                            .Image(eventImageBytes).FitArea();
                                    }
                                });
                            });

                            // 3. Footer
                            col.Item().PaddingTop(3).BorderTop(0.5f).BorderColor("#EEEEEE").Background("#F9F9F9").PaddingVertical(3).AlignCenter().Text(text =>
                            {
                                text.Span("Thank you for your reservation. Please present this ticket at the venue.")
                                    .FontSize(7.5f)
                                    .FontColor("#787878");
                            });
                        });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate ticket PDF using QuestPDF for ticket: {TicketNumber}", reservation.TicketNumber);
                throw;
            }
        }
    }
}
