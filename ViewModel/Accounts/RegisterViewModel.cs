using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "o password é obrigatório")]
        //public string Password { get; set; }

    }
}
