using Blog.Data;
using Blog.Models;
using Blog.ViewModel;
using Blog.ViewModel.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {


        [HttpGet]
        [Route("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25
            )
        {


            int count = await context.Posts.AsNoTracking().CountAsync();

            var posts = await context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.Author)
                .Select(post =>
                    new ListPostsViewModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        LasUpdateDate = post.LastUpdateDate,
                        Category = post.Category.Name,
                        Author = $"{post.Author.Name} {post.Author.Email}"

                    }
                )
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LasUpdateDate)
                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts

            }));

        }




        [HttpGet]
        [Route("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsPost(
            [FromServices] BlogDataContext context,
            [FromRoute] int id
            )
        {

            try
            {
                var post = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                {
                    return NotFound(new ResultViewModel<string>("Conteúdo não encontrado"));
                }

                return Ok(new ResultViewModel<Post>(post));

            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Post>("05x04 Falha interna no servidor"));
            }

        }

        [HttpGet]
        [Route("v1/posts/category/{category}")]
        public async Task<IActionResult> GetBtCategoryAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] string category,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25
            )
        {
            try
            {

                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Where(x => x.Category.Slug == category)
                    .Select(item =>
                        new ListPostsViewModel
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Slug = item.Slug,
                            LasUpdateDate = item.LastUpdateDate,
                            Category = item.Category.Name,
                            Author = $"{item.Author.Name} {item.Author.Email}"
                        }
                    )
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(item => item.LasUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts

                }));

            }
            catch {

                return StatusCode(500, new ResultViewModel<string>("05x12 falha interna no servidor"));
            }


        }


    }
}
