﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TecnologyShop.Data;
using TecnologyShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wkhtmltopdf.NetCore; // recien añadido
using Rotativa.AspNetCore; // recien añadido

namespace TecnologyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        readonly IGeneratePdf _generatePdf; // añadido recientemente
        public UserController(ApplicationDbContext db, IGeneratePdf generatePdf /*añadido recientemente*/)
        {
            _db = db;
            _generatePdf = generatePdf; // añadido recientemente
        }
        //INDEX - GET
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());
        }
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser = await _db.ApplicationUser.Where(m => m.Id == id).FirstOrDefaultAsync();
            applicationUser.LockoutEnd = DateTime.Now.AddYears(100);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser = await _db.ApplicationUser.Where(m => m.Id == id).FirstOrDefaultAsync();
            applicationUser.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return View();
            }
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int?id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edited(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return View();
            }
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetPDF(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //return View(await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());

            //if (catalogueVM.CatalogueItem == null)
            //{
            //    return NotFound();
            //}

            return new ViewAsPdf("ReportProduct", claim)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait, //para modificar la pagina
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

        }

    }
}