using Blog.Data;
using Blog.Services;
using Blog.ViewModel;
using Blog.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using SecureIdentity.Password;
using Microsoft.EntityFrameworkCore;
using Blog.ViewModel.Accounts;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{

    [ApiController]
    public class AccountController : ControllerBase
    {
        /*
        private readonly TokenService _tokenService;


        public AccountController(TokenService tokenService) {
            _tokenService = tokenService;
        }
        */

        [HttpPost]
        [Route("/v1/account")]
        public async Task<IActionResult> Post(
            [FromBody] RegisterViewModel model,
            [FromServices] BlogDataContext context,
            [FromServices] EmailService emailservice
            )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<RegisterViewModel>(ModelState.GetErrors()));
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };


            // gerar senha
            var password = PasswordGenerator.Generate(length: 25, includeSpecialChars: true, upperCase: false);
            user.PasswordHash = PasswordHasher.Hash(password);


            try
            {

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                emailservice.Send(
                    user.Name,
                    user.Email,
                    subject: "Bem vindo ao blog",
                    body: $"sua senha é {password}"
                    );

                return Ok(new ResultViewModel<dynamic>(new { user = user.Email, password }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("05x99 - este email já está cadastrado"));

            }

            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna no servidor"));

            }


        }


        [AllowAnonymous] // n/a Authorize
        [HttpPost]
        [Route("/v1/account/login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginViewModel model,
            [FromServices] BlogDataContext context,
            [FromServices] TokenService tokenService)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            }

            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));
            }

            // comparação senha com hash password
            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            {

                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));
            }


            try
            {

                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, errors: null));

            }
            catch
            {

                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna no sistema"));
            }
        }


        [Authorize]
        [HttpPost("v1/account/upload-image")]
        public async Task<IActionResult> UploadImage(
                [FromBody] UploadImageViewModel model,
                [FromServices] BlogDataContext context
            )
        {

            var filename = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"^data:image/\[a-z]+;base64,").Replace(model.base64Image, "");

            var bytes = Convert.FromBase64String(data);

            try {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{filename}", bytes);
                
            }

            catch (Exception ex){

                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna do servidor"));
            }

            // pegando do usuário logado
            var user = await context
                .Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            if (user == null) {

                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));
            
            }

            user.Image = $"https://localhost:0000/images/{filename}";

            try {
                context.Users.Update(user);
                await context.SaveChangesAsync();
                
            }
            catch (Exception ex) {

                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna so servidor"));
            
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso"));

        }




        /*
        [Authorize(Roles="user")]
        [HttpGet]
        [Route("/v1/user")]
        public IActionResult GetUser() {
            return Ok(User.Identity.Name);
        }


        [Authorize(Roles ="author")]
        [HttpGet]
        [Route("/v1/author")]
        public IActionResult GetAuthor()
        {
            return Ok(User.Identity.Name);
        }


        [Authorize (Roles ="admin")]
        [HttpGet]
        [Route("/v1/admin")]
        public IActionResult GetAdmin()
        {
            return Ok(User.Identity.Name);
        }
        */
    }
}
