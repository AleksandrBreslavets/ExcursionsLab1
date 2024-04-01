using System.ComponentModel.DataAnnotations;

namespace ExcursionsInfrastructure.ViewModel
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage="Введіть назву ролі.")]
        [Display(Name = "Роль")]
        public string Name{ get; set;}
    }
}
