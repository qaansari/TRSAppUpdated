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
                        page.Size(210, 140, Unit.Millimetre);
                        page.Margin(5, Unit.Millimetre);
                        page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(9.5f).FontColor("#0F0F0F"));

                        page.Content().Border(1).BorderColor("#D6DADE").CornerRadius(10).Background("#FFFFFF").Column(card =>
                        {
                            // 1. Header Bar (cloned from .ticket-header)
                            card.Item().Background("#1AA48B").PaddingVertical(10).AlignCenter().Text(text =>
                            {
                                text.Span("E-Ticket")
                                    .FontSize(17)
                                    .SemiBold()
                                    .FontColor("#FFFFFF")
                                    .LetterSpacing(0.06f);
                            });

                            // 2. Ticket Body (cloned from .ticket-body)
                            card.Item().PaddingHorizontal(20).PaddingTop(8).PaddingBottom(6).Column(body =>
                            {
                                // Centered Logo (.logo_img_section)
                                if (logoBytes != null)
                                {
                                    body.Item().AlignCenter().Height(40).Image(logoBytes).FitArea();
                                }

                                // Centered E-Ticket heading (.ticket_heading)
                                body.Item().PaddingTop(3).AlignCenter().Text(text =>
                                {
                                    text.Span("E-TICKET")
                                        .FontSize(15)
                                        .Bold()
                                        .FontColor("#1AA48B")
                                        .LetterSpacing(0.08f);
                                });

                                // Ticket Number Box (.ticket-number)
                                body.Item().PaddingTop(5).PaddingBottom(8)
                                    .Background("#F8F9FA")
                                    .Border(1).BorderColor("#CCCCCC")
                                    .CornerRadius(8)
                                    .PaddingVertical(7)
                                    .AlignCenter()
                                    .Text(text =>
                                    {
                                        text.Span(reservation.TicketNumber ?? string.Empty)
                                            .FontSize(18)
                                            .Bold()
                                            .FontColor("#1AA48B")
                                            .LetterSpacing(0.12f);
                                    });

                                // Ticket Content (.ticket-content)
                                body.Item().Row(contentRow =>
                                {
                                    var leftCol = eventImageBytes != null ? contentRow.RelativeItem(6) : contentRow.RelativeItem(10);
                                    leftCol.Column(info =>
                                    {
                                        // Event Name
                                        info.Item().BorderBottom(0.75f).BorderColor("#EEEEEE").PaddingVertical(3).Row(r =>
                                        {
                                            r.ConstantItem(105).Text("Event Name:").SemiBold().FontColor("#130716").FontSize(9.5f);
                                            r.RelativeItem().Text((reservation.EventName ?? string.Empty).ToUpper()).Bold().FontColor("#0A0303").FontSize(9.5f);
                                        });

                                        // Show Date
                                        info.Item().BorderBottom(0.75f).BorderColor("#EEEEEE").PaddingVertical(3).Row(r =>
                                        {
                                            r.ConstantItem(105).Text("Show Date:").SemiBold().FontColor("#130716").FontSize(9.5f);
                                            r.RelativeItem().Text((reservation.ShowDateTime ?? string.Empty).ToUpper()).Bold().FontColor("#0A0303").FontSize(9.5f);
                                        });

                                        // Customer Name
                                        info.Item().BorderBottom(0.75f).BorderColor("#EEEEEE").PaddingVertical(3).Row(r =>
                                        {
                                            r.ConstantItem(105).Text("Customer Name:").SemiBold().FontColor("#130716").FontSize(9.5f);
                                            r.RelativeItem().Text((reservation.CustomerName ?? string.Empty).ToUpper()).Bold().FontColor("#0A0303").FontSize(9.5f);
                                        });

                                        // Phone Number
                                        info.Item().BorderBottom(0.75f).BorderColor("#EEEEEE").PaddingVertical(3).Row(r =>
                                        {
                                            r.ConstantItem(105).Text("Phone Number:").SemiBold().FontColor("#130716").FontSize(9.5f);
                                            r.RelativeItem().Text((reservation.CustomerPhone ?? string.Empty).ToUpper()).Bold().FontColor("#0A0303").FontSize(9.5f);
                                        });

                                        // Reserved Seats
                                        info.Item().PaddingTop(3.5f).Row(r =>
                                        {
                                            r.ConstantItem(105).Text($"Reserved Seats: ({seatLabelsList.Count})").SemiBold().FontColor("#130716").FontSize(9.5f);
                                            r.RelativeItem().Inlined(inlined =>
                                            {
                                                inlined.Spacing(4);
                                                if (!string.IsNullOrEmpty(reservation.TicketCategory))
                                                {
                                                    inlined.Item().Text($"{reservation.TicketCategory.ToUpper()} -").SemiBold().FontColor("#0A0303").FontSize(9.5f);
                                                }

                                                if (isGeneralEvent)
                                                {
                                                    inlined.Item().Background("#58FFD9").CornerRadius(5).PaddingVertical(2.5f).PaddingHorizontal(7)
                                                        .Text("First Come First Sit").FontSize(8.5f).Bold().FontColor("#100C0C");
                                                }
                                                else if (seatLabelsList.Count > 0)
                                                {
                                                    foreach (var seat in seatLabelsList)
                                                    {
                                                        inlined.Item().Background("#58FFD9").CornerRadius(5).PaddingVertical(2.5f).PaddingHorizontal(7)
                                                            .Text(seat).FontSize(8.5f).Bold().FontColor("#100C0C");
                                                    }
                                                }
                                                else
                                                {
                                                    inlined.Item().Background("#58FFD9").CornerRadius(5).PaddingVertical(2.5f).PaddingHorizontal(7)
                                                        .Text("None").FontSize(8.5f).Bold().FontColor("#100C0C");
                                                }
                                            });
                                        });
                                    });

                                    // Event Poster Image Column (.image-info)
                                    if (eventImageBytes != null)
                                    {
                                        contentRow.RelativeItem(4).AlignRight().PaddingLeft(14)
                                            .Height(115)
                                            .Border(1).BorderColor("#EAEAEA")
                                            .CornerRadius(8)
                                            .Image(eventImageBytes)
                                            .FitArea();
                                    }
                                });
                            });

                            // 3. Footer Bar (cloned from .footer)
                            card.Item().BorderTop(0.75f).BorderColor("#EEEEEE").Background("#F9F9F9").PaddingVertical(9).AlignCenter().Text(text =>
                            {
                                text.Span("Thank you for your reservation. Please present this ticket at the venue.")
                                    .FontSize(8.5f)
                                    .FontColor("#999999");
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
