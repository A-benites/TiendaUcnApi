using Mapster;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context; // Para transacciones y roles
    private readonly int _verificationCodeExpirationTimeInMinutes;
    private readonly IConfiguration _configuration;
    public UserService(
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository,
        IEmailService emailService,
        AppDbContext context,
        IConfiguration configuration
    )
    {
        _userRepository = userRepository;
        _verificationCodeRepository = verificationCodeRepository;
        _emailService = emailService;
        _context = context;
        _configuration = configuration;
        _verificationCodeExpirationTimeInMinutes = _configuration.GetValue<int>("VerificationCode:ExpirationTimeInMinutes");
    }

    public async Task RegisterAsync(RegisterDTO registerDto)
    {
        // 1. Validar que el email y el RUT no existan usando los métodos del repositorio.
        if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
        {
            throw new ApplicationException("El correo electrónico ya está en uso.");
        }

        if (await _userRepository.ExistsByRutAsync(registerDto.Rut))
        {
            throw new ApplicationException("El RUT ya se encuentra registrado.");
        }

        // 2. Mapear DTO a la entidad User usando Mapster.
        // La configuración de mapeo se define en UserMappers.cs y se registra en Program.cs.
        var newUser = registerDto.Adapt<User>();

        // 3. Usar el repositorio para crear el usuario.
        // Este método ya se encarga de hashear el password y asignar el rol "Cliente".
        var userCreated = await _userRepository.CreateAsync(newUser, registerDto.Password);
        if (!userCreated)
        {
            // Si CreateAsync devuelve false, es porque Identity tuvo un problema (ej: password no cumple requisitos).
            // Los detalles ya fueron logueados por el repositorio.
            throw new ApplicationException(
                "No se pudo crear el usuario. Revisa los requisitos de contraseña o contacta a soporte."
            );
        }

        // 4. Generar y guardar el código de verificación.
        var verificationCode = new VerificationCode
        {
            Code = new Random().Next(100000, 999999).ToString(),
            ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes),
            UserId = newUser.Id, // Asignamos el ID del usuario recién creado.
        };
        await _verificationCodeRepository.AddAsync(verificationCode);
        await _context.SaveChangesAsync(); // Guardamos el código.

        // 5. Enviar el correo de bienvenida con el código.
        await _emailService.SendVerificationCodeAsync(newUser, verificationCode.Code);
    }
}