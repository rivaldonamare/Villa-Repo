using MagicVilla_Utility;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;

    public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
    }

    public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
    {
        return SendAsync<T>(new Models.APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = loginRequestDTO,
            Url = villaUrl + "/api/User/login"
        });
    }

    public Task<T> RegisterAsync<T>(RegistrationRequestDTO userDTO)
    {
        return SendAsync<T>(new Models.APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = userDTO,
            Url = villaUrl + "/api/User/register"
        });
    }
}
