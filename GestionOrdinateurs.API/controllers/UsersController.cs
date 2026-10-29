using GestionOrdinateurs.API.contexts;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _context;

    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // GET /api/Users
    [HttpGet]
    public ActionResult<IReadOnlyList<User>> GetAllUsers()
    {
        IReadOnlyList<User> users = _context.Users
            .AsNoTracking()
            .ToList();

        return Ok(users);
    }

    // GET /api/Users/{login}
    [HttpGet("{login}")]
    public ActionResult<User> GetUserByLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return BadRequest("Le login est obligatoire.");
        }

        User? user = _context.Users
            .AsNoTracking()
            .FirstOrDefault(user => user.Login == login);

        return user is null
            ? NotFound($"Aucun utilisateur ne possède le login '{login}'.")
            : Ok(user);
    }
}
