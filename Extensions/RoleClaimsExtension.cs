using Blog.Models;
using System.Security.Claims;

namespace Blog.Extensions
{
    public static class RoleClaimsExtension
    {

        public static IEnumerable<Claim> GetClaims(this User user)
        {

            var result = new List<Claim> {
                new Claim(ClaimTypes.Name, value: user.Email)
            };

            result.AddRange(
                    user.Roles.Select(role => new Claim(ClaimTypes.Role, value: role.Slug))
                );

            return result;
        }



    }

}

