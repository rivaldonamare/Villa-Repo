using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModel;

public class DeleteVillaNumberViewModel
{
    public DeleteVillaNumberViewModel()
    {
        VillaNumber = new VillaNumberDTO();
    }

    public VillaNumberDTO VillaNumber { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }

}
