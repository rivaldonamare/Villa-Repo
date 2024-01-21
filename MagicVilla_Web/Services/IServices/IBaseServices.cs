using MagicVilla_Web.Models;

namespace MagicVilla_Web.Services.IServices;

public interface IBaseServices
{
    APIResponse ResponseModel { get; set; }
    
    Task<T> SendAsync<T>(APIRequest request);
}
