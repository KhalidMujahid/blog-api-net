namespace blog_api.Models.Requests;

public record AuthResponse(string Token, UserProfileResponse User);
