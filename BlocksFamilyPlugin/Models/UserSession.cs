namespace BlocksFamilyPlugin.Models
{
    /// <summary>
    /// Represents the current user's subscription and authentication state.
    /// In a real product this would be populated from an API login call.
    /// </summary>
    public class UserSession
    {
        public static UserSession Guest => new() { Plan = SubscriptionPlan.Free };

        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = "Guest";
        public string Email { get; set; } = string.Empty;
        public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);
    }

    public enum SubscriptionPlan
    {
        Free,
        Premium
    }
}
