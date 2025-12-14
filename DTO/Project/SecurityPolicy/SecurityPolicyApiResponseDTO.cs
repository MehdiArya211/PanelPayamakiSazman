namespace DTO.Project.SecurityPolicy
{
    public class SecurityPolicyApiResponseDTO
    {
        public SecurityPolicyDataDTO Data { get; set; }
    }

    public class SecurityPolicyDataDTO
    {
        public PasswordPolicyDTO PasswordPolicy { get; set; }
        public LockoutPolicyDTO LockoutPolicy { get; set; }
        public TokenPolicyDTO TokenPolicy { get; set; }
        public ReAuthenticationPolicyDTO ReAuthenticationPolicy { get; set; }
    }

    public class PasswordPolicyDTO
    {
        public int MinLength { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireSpecial { get; set; }
        public int MinimumDifferentCharacters { get; set; }
        public bool RotationEnabled { get; set; }
        public int RotationDays { get; set; }
        public int PasswordHistoryLimit { get; set; }
    }

    public class LockoutPolicyDTO
    {
        public int MaxFailedAttempts { get; set; }
        public int LockoutWindowMinutes { get; set; }
        public int LockoutDurationMinutes { get; set; }
    }

    public class TokenPolicyDTO
    {
        public int AccessTokenMinutes { get; set; }
        public int RefreshTokenDays { get; set; }
    }

    public class ReAuthenticationPolicyDTO
    {
        public int FreshnessMinutes { get; set; }
    }
}
