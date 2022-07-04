using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts
{
    public class UploadImageViewModel
    {
        [Required(ErrorMessage ="Imagem inválida")]
        public string base64Image { get; set; }
    }
}
