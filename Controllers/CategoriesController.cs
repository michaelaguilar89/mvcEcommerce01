using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCEcommerce.Data;
using MVCEcommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Services;
using Mono.TextTemplating;
namespace MVCEcommerce.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly CategoryService _category;
        private readonly UserManager<IdentityUser> _userManager;
        public CategoriesController(CategoryService category, UserManager<IdentityUser> userManager)
        {
            _category = category;
            _userManager = userManager;
        }


        // Método para obtener el UserId
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categorie = await _category.GetCategories();
            return View(categorie);
        }

        

        // GET: Categories/Add or Edit
        public async Task<IActionResult> AddOrEdit(int? id)
        {
            if (id==null || id==0)
            {
                return View(new CategoryDto());//return new categorie
            }
            else
            {
                var categorie = await _category.Details(id);
                if (categorie==null)
                {
                    ViewBag.ErrorMessage = "Error : Product not found!";

                }

                return View(categorie);//edit categorie
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddorEdit(CategoryDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                    var secret = GetCurrentUserId();
                    var exist = await _category.Exist(dto.Title);
                     // Verificar si el título ya existe antes de agregar la categoría
                   if (exist==1)
                    {
                        ModelState.AddModelError("Title", "Category title is already taken.");
                        return View(dto); // Regresar a la vista con el mensaje de error
                    }
                   // save or update category
                    var state = await _category.AddOrEdit(dto, secret);
                    if (state=="1" || state=="2")
                    {
                        return RedirectToAction("Index");
                    }
                   
                    ViewBag.ErrorMessage = "Error : "+state;
                    return View(dto);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on AddOrEdit: " + e.Message);
            }

            return View(dto); // Retornar el dto en caso de errores
        }
        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id >0)
            {
                var category = await _category.DetailsWithUser(id);
                if (category != null)
                {
                    return View(category);
                    
                }
                ViewBag.ErrorMessage = "Error : Product not Found!";
                return View();

            }

            return NotFound();
          
        }

     
        // POST: Categories/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id > 0) {

                var state = await _category.Delete(id);
                if (state=="1")
                {
                    return RedirectToAction(nameof(Index));
                }
                Console.WriteLine(state);
                TempData["ErrorMessage"] = "Error: " + state; // Usar TempData para mantener el mensaje de error.
                return RedirectToAction(nameof(Delete), new { id });
            

            }
            // Si hay un error, vuelve a mostrar la vista de eliminación pero redirige a 'Delete'
            TempData["ErrorMessage"] = "Error: Category not found";
            return RedirectToAction(nameof(Delete), new { id });
        }

        
    }
}
