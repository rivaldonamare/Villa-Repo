using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private string secretKey;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task<bool> IsUniqueUser(string username)
    {
        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == username);
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
       /* if (loginRequestDTO == null || string.IsNullOrWhiteSpace(loginRequestDTO.UserName) || string.IsNullOrWhiteSpace(loginRequestDTO.Password))
        {
            return new LoginResponseDTO
            {
                Token = "",
                User = null
            };
        }*/

        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (user == null || isValid == false)
        {
            return new LoginResponseDTO
            {
                Token = "",
                User = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new(ClaimTypes.Name, user.UserName.ToString()),
            new(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.Now.AddDays(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var loginResponseDTO = new LoginResponseDTO
        {
            Token = tokenHandler.WriteToken(token),
            User = _mapper.Map<UserDTO>(user)
        };

        return loginResponseDTO;
    }


    public async Task<UserDTO> Register(RegistrationRequestDTO reqistrationRequestDTO)
    {
        ApplicationUser user = new()
        {
            UserName = reqistrationRequestDTO.UserName,
            Email = reqistrationRequestDTO.Name,
            NormalizedEmail = reqistrationRequestDTO.Name.ToUpperInvariant(),
            Name = reqistrationRequestDTO.Name
        };

        try
        {
            var result = await _userManager.CreateAsync(user, reqistrationRequestDTO.Password);
            if (result.Succeeded)
            {
                if(!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                     await _roleManager.CreateAsync(new IdentityRole("admin"));
                     await _roleManager.CreateAsync(new IdentityRole("customer"));
                }
                await _userManager.AddToRoleAsync(user, "admin");
                var userToReturn = _context.ApplicationUsers.FirstOrDefault(x => x.UserName == reqistrationRequestDTO.UserName);
                return _mapper.Map<UserDTO>(userToReturn);

            }
        }
        catch (Exception ex)
        {
            
        }

        return new UserDTO();
    }
}
