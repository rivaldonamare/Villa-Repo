using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModel;

public class UpdateVillaNumberViewModel
{
    public UpdateVillaNumberViewModel()
    {
        VillaNumber = new VillaNumberUpdateDTO();
    }

    public VillaNumberUpdateDTO VillaNumber { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }

}
