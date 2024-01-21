using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers;
[Route("api/[Controller]")]
[ApiController]
public class VillaNumberAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IVillaNumberRepository _villaRepo;
    protected APIResponse _apiResponse;
    private readonly IVillaRepository _villaRepository;

    public VillaNumberAPIController(IMapper mapper, IVillaNumberRepository villaRepo, IVillaRepository villaRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _villaRepo = villaRepo ?? throw new ArgumentNullException(nameof(villaRepo));
        this._apiResponse = new();
        _villaRepository = villaRepository ?? throw new ArgumentNullException(nameof(villaRepository));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        try
        {
            IEnumerable<VillaNumber> villasList = await _villaRepo.GetAllAsync(includeProperties:"Villa");
            _apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(villasList);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessage = new List<string> { ex.Message.ToString() };
        }
        return _apiResponse;
    }

    [HttpGet("{villaNo:int}", Name = "GetVillaNumberByNo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVillaNumberById(int villaNo)
    {
        try
        {
            if (villaNo == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }

            var villaNumber = await _villaRepo.GetAsync(x => x.VillaNo == villaNo);

            if (villaNumber == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessage = new List<string> { ex.Message.ToString() };
        }
        return _apiResponse;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
    {
        try
        {
            if (await _villaRepo.GetAsync(x => x.VillaNo == createDTO.VillaNo) != null)
            {
                ModelState.AddModelError("ErrorMessage", "Villa Number already Exists!");
                return BadRequest(ModelState);
            }

            if (await _villaRepository.GetAsync(x => x.Id == createDTO.VillaId) == null)
            {
                ModelState.AddModelError("ErrorMessage", "Villa Id not found");
                return BadRequest(ModelState);  
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(createDTO);
            model.CreatedDate = DateTime.Now;
            await _villaRepo.CreateAsync(model);

            _apiResponse.Result = _mapper.Map<VillaNumberCreateDTO>(model);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            return CreatedAtRoute("GetVillaById", new { Id = model.VillaNo }, _apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessage = new List<string> { ex.Message.ToString() };
        }
        return _apiResponse;
    }

    [HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
    {
        try
        {
            if (villaNo == 0)
            {
                return BadRequest();
            }

            var villa = await _villaRepo.GetAsync(x => x.VillaNo == villaNo);

            if (villa == null)
            {
                return NotFound();
            }
            await _villaRepo.RemoveAsync(villa);

            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessage = new List<string> { ex.Message.ToString() };
        }
        return _apiResponse;
    }

    [HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || villaNo != updateDTO.VillaNo)
            {
                return BadRequest();
            }

            if (await _villaRepository.GetAsync(x => x.Id == updateDTO.VillaId) == null)
            {
                ModelState.AddModelError("ErrorMessage", "Villa Id not found");
                return BadRequest(ModelState);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

            await _villaRepo.UpdateAsync(model);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessage = new List<string> { ex.Message.ToString() };
        }
        return _apiResponse;
    }
}
