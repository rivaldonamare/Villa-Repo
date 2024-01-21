using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModel;

public class CreateVillaNumberViewModel
{
    public CreateVillaNumberViewModel()
    {
        VillaNumber = new VillaNumberCreateDTO();
    }

    public VillaNumberCreateDTO VillaNumber { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }

}
