using Microsoft.AspNetCore.Authorization;

namespace AuthenticationApp.Authorization
{
    public class AgeGreaterThan25Requirement:IAuthorizationRequirement
    {
    }
}
