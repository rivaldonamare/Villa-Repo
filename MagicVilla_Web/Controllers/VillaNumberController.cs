using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.ViewModel;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace MagicVilla_Web.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _service;
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaNumberController(IVillaNumberService service, IMapper mapper, IVillaService villaService)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _villaService = villaService ?? throw new ArgumentNullException(nameof(villaService));
    }

    public async Task<IActionResult> IndexVillaNumber()
    {
        List<VillaNumberDTO> list = new();

        var response = await _service.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
        }

        return View(list);
    }

    #region Create Villa Number
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateVillaNumber()
    {
        CreateVillaNumberViewModel villaNumberViewModel = new();
        var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            var villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));

            villaNumberViewModel.VillaList = villaList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }
        return View(villaNumberViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateVillaNumber(CreateVillaNumberViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _service.CreateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa Number Created Successfully";
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                if (response.ErrorMessage.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessage.FirstOrDefault());
                }
            }
        }
     
        var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        if (resp != null && resp.IsSuccess)
        {
            var villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result));

            model.VillaList = villaList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }
        return View(model);
    }
    #endregion

    #region Update Villa
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateVillaNumber(int villaNo)
    {
        UpdateVillaNumberViewModel villaNumberViewModel = new();
        var response = await _service.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));

        if (response != null && response.IsSuccess)
        {
            VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
            villaNumberViewModel.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
        }

        response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            var villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));

            villaNumberViewModel.VillaList = villaList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(villaNumberViewModel);
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateVillaNumber(UpdateVillaNumberViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _service.UpdateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa Number Update Successfully";
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                if (response.ErrorMessage.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessage.FirstOrDefault());
                }
            }
        }

        var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        if (resp != null && resp.IsSuccess)
        {
            var villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result));

            model.VillaList = villaList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }
        return View(model);
    }
    #endregion

    #region Delete Villa
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteVillaNumber(int villaNo)
    {
        DeleteVillaNumberViewModel villaNumberViewModel = new();
        var response = await _service.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));

        if (response != null && response.IsSuccess)
        {
            VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
            villaNumberViewModel.VillaNumber = _mapper.Map<VillaNumberDTO>(model);
        }

        response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            var villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));

            villaNumberViewModel.VillaList = villaList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(villaNumberViewModel);
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteVillaNumber(DeleteVillaNumberViewModel model)
    {
        var response = await _service.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo, HttpContext.Session.GetString(SD.SessionToken));

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Villa Number Delete Successfully.";
            return RedirectToAction(nameof(IndexVillaNumber));
        }

        return View(model);
    }
    #endregion
}
