namespace TRSAppUpdated.Helpers
{
    public static class CacheKeys
    {
        public const string Countries = "LOOKUP_COUNTRIES";
        public const string Cities = "LOOKUP_CITIES";
        public const string Categories = "LOOKUP_CATEGORIES";
        public const string Venues = "LOOKUP_VENUES";
        public const string Auditoriums = "LOOKUP_AUDITORIUMS";
        public const string PaymentModes = "LOOKUP_PAYMENT_MODES";
        public const string ReservationStatuses = "LOOKUP_RESERVATION_STATUSES";
        public const string EventStatuses = "LOOKUP_EVENT_STATUSES";
        public const string NoteTypes = "LOOKUP_NOTE_TYPES";
        public const string Roles = "LOOKUP_ROLES";
        public const string DashboardMetrics = "DASHBOARD_METRICS";
        public const string EventsList = "LOOKUP_EVENTS_ALL";
        public const string ShowsList = "LOOKUP_SHOWS_ALL";

        // View Tickets Cache
        public const string ViewTicketsPrefix = "VIEW_TICKETS_";
        public static string ViewTickets(int showId, int eventId) => $"{ViewTicketsPrefix}{showId}_{eventId}";

        // Event Details Cache
        public const string EventDetailsPrefix = "EVENT_DETAILS_";
        public static string EventDetails(int eventId) => $"{EventDetailsPrefix}{eventId}";

        // Reservation Details Cache
        public const string ReservationDetailsPrefix = "RESERVATION_DETAILS_";
        public static string ReservationDetails(int reservationId) => $"{ReservationDetailsPrefix}{reservationId}";
    }
}
