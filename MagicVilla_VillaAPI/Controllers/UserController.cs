using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    protected APIResponse _response;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        this._response = new();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        var loginResponse = await _userRepository.Login(model);

        if(loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessage.Add("Username or Password is incorrect");
            return BadRequest(_response);
        }

        _response.StatusCode = System.Net.HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
    {
        bool ifUserNameunique = await _userRepository.IsUniqueUser(model.UserName);

        if (!ifUserNameunique)
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessage.Add("Username already exists");
            return BadRequest(_response);
        }

        var user = await _userRepository.Register(model);

        if (user == null)
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessage.Add("Error while Registering");
            return BadRequest(_response);
        }

        _response.StatusCode = System.Net.HttpStatusCode.OK;
        _response.IsSuccess = true;
        return Ok(_response);

    }
}
