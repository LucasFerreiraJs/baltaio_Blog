using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModel;
using Blog.ViewModel.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{

    [ApiController]
    public class CategoryController : ControllerBase
    {


        [HttpGet]
        [Route("/v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromServices] IMemoryCache cache
            )
        {

   
            try
            {
                //List<Category> categories = await context.Categories.ToListAsync();
                List<Category> categories = cache.GetOrCreate("CategoriesCache", entry =>
                {

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });

                if (categories == null)
                {
                    return NotFound();
                }
                return Ok(new ResultViewModel<List<Category>>(categories));
            }

            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE11 - Falha interna no servidor"));
            }
        }


        private List<Category> GetCategories(BlogDataContext context) {

            return context.Categories.ToList();
        }



        [HttpGet]
        [Route("/v1/categories/{id:int}")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id
            )
        {

            try
            {
                Category category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));
                }

                return Ok(new ResultViewModel<Category>(category));
            }


            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE11 - Falha interna no servidor"));
            }
        }


        [HttpPost]
        [Route("/v1/categories")]
        public async Task<IActionResult> Post(
            [FromServices] BlogDataContext context,
            [FromBody] EditorCategoryViewModel model
            )
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<EditorCategoryViewModel>(ModelState.GetErrors()));
            }

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Posts = null,
                    Name = model.Name,
                    Slug = model.Slug,

                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"/v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }

            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE09 - Não foi pussível incluir a categoria"));
            }

            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE10 - Falha interna no servidor"));
            }

        }


        [HttpPut]
        [Route("/v1/categories/{id:int}")]
        public async Task<IActionResult> Put(
                [FromServices] BlogDataContext context,
                [FromBody] EditorCategoryViewModel model,
                [FromRoute] int id
            )
        {

            try
            {
                Category category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE08 - Não foi pussível alterar a categoria"));
            }

            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE11 - Falha interna no servidor"));
            }

        }


        [HttpDelete]
        [Route("/v1/categories/{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
            )
        {
            try
            {
                Category category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE07 - Não foi pussível deletar a categoria"));
            }

            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE13 - Falha interna no servidor"));
            }
        }


    }
}
