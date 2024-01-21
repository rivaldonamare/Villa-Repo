using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers;
[Route("api/[Controller]")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IVillaRepository _villaRepo;
    protected APIResponse _apiResponse;

    public VillaAPIController(IMapper mapper, IVillaRepository villaRepository)
    {
        
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _villaRepo = villaRepository ?? throw new ArgumentNullException(nameof(villaRepository));
        this._apiResponse = new();
    }

    //API Starting point
    [HttpGet]
    [ResponseCache(Duration = 30)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<APIResponse>> GetVillas([FromQuery] int? occupancy, [FromQuery] string search, int pageSize = 0, int pageNumber = 1)
    {
        try
        {
            IEnumerable<Villa> villasList;

            if (occupancy > 0)
            {
                villasList = await _villaRepo.GetAllAsync(x => x.Occupancy == occupancy, pageSize:pageSize,
                    pageNumber:pageNumber);
            }
            else
            {
                villasList = await _villaRepo.GetAllAsync(pageSize: pageSize,
                    pageNumber: pageNumber);
            }

            if (!string.IsNullOrEmpty(search))
            {
                villasList = villasList.Where(x => x.Name == search);
            }

            Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
            _apiResponse.Result = _mapper.Map<List<VillaDTO>>(villasList);
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

    [HttpGet("{id:int}", Name = "GetVillaByid")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ResponseCache(CacheProfileName = "Default30")]
    public async Task<ActionResult<APIResponse>> GetVillaById(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }

            var villa = await _villaRepo.GetAsync(x => x.Id == id);

            if (villa == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
        try
        {
            if (await _villaRepo.GetAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessage", "Villa already Exists!");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            await _villaRepo.CreateAsync(model);

            _apiResponse.Result = _mapper.Map<VillaDTO>(model);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.IsSuccess = true;
            return CreatedAtRoute("GetVillaById", new { id = model.Id }, _apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessage = new List<string> { ex.Message.ToString() };
        }
        return _apiResponse;
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _villaRepo.GetAsync(x => x.Id == id);

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

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

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

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patch)
    {
        if (patch == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _villaRepo.GetAsync(x => x.Id == id, tracked: false);

        VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

        if (villa == null)
        {
            return BadRequest();
        }

        patch.ApplyTo(villaDTO, ModelState);

        Villa model = _mapper.Map<Villa>(villaDTO);

        await _villaRepo.UpdateAsync(model);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return NoContent();
    }
}
