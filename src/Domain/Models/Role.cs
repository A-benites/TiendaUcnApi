using Microsoft.AspNetCore.Identity;

namespace TiendaUcnApi.src.Domain.Models;

public class Role : IdentityRole<int>
{
    public Role() : base() { }
}