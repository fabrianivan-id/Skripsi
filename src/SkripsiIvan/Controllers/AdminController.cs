using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkripsiIvan.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace SkripsiIvan.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private readonly SkripsiIvanContext _context;
        private readonly IHostingEnvironment _he;
        public AdminController(SkripsiIvanContext context)
        {
            _context = context;
        }



        #region Users
        public async Task<IActionResult> Index()
        {
            return View(new AdminViewModels() { Users = await _context.Admin.ToListAsync() });
        }
        [Route("users/UserTable")]
        public async Task<IActionResult> UserTable()
        {
            return View(await _context.Admin.ToListAsync());
        }

        [HttpPost]
        [Route("users/Buat")]
        [ValidateAntiForgeryToken]
        public IActionResult Buat(Admin user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Admin.Any(x => x.UsernameAdmin == user.UsernameAdmin.Trim().ToLower()))
                {
                    return Json(new { error = true, errorMsg = "Username sudah dimasukkan!" });
                }
                else
                {
                    user.UsernameAdmin = user.UsernameAdmin.Trim().ToLower();
                    user.PasswordAdmin = HashPassword(user.PasswordAdmin);
                    _context.Admin.Add(user);
                    _context.SaveChanges();
                    return Json(new { error = false });
                }
            }
            return Json(new { error = true, errorMsg = "Harap diisi semua kolom!" });
        }

        [Route("users/GetEdit")]
        public IActionResult GetEdit(int id)
        {
            return Json(_context.Admin.SingleOrDefault(x => x.IdAdmin == id));
        }

        [HttpPost]
        [Route("Users/Reset")]
        public async Task<IActionResult> Reset(int id, string pass)
        {
            var user = _context.Admin.Single(x => x.IdAdmin == id);
            try
            {
                user.PasswordAdmin = HashPassword(pass.Trim());
                _context.Admin.Update(user);
                await _context.SaveChangesAsync();
                return Json(new { error = false });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }
        [HttpPost]
        [Route("users/Ubah")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ubah(UsersEditViewModels model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Admin.Single(x => x.IdAdmin == model.IdUsers);

                if (model.Username != user.UsernameAdmin && _context.Admin.Any(x => x.UsernameAdmin == model.Username.Trim().ToUpper()))
                {
                    return Json(new { error = true, errorMsg = "Username sudah dimasukkan!" });
                }
                try
                {

                    user.UsernameAdmin = model.Username.Trim().ToUpper();
                    user.Name = model.Name;
                    _context.Admin.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new { error = false });
                }
                catch (Exception ex)
                {
                    return Json(new { error = true, errorMsg = ex.Message });
                }
            }
            return Json(new { error = true, errorMsg = "Harap isi semua kolom!" });
        }

        [HttpPost]
        [Route("users/Delete")]
        public IActionResult Delete(int id)
        {
            var user = _context.Admin.SingleOrDefault(m => m.IdAdmin == id);
            _context.Admin.Remove(user);
            _context.SaveChanges();
            return Json(new { error = false });
        }
        #endregion                

        #region Kategori

        public async Task<IActionResult> Kategori()
        {
            return View(new KategoriViewModels() { Kategoris = await _context.Kategori.ToListAsync() });
        }

        public async Task<IActionResult> KategoriTable()
        {
            return View(await _context.Kategori.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateK(Kategori model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Kategori.Any(x => x.NamaKategori == model.NamaKategori))
                {
                    return Json(new { error = true, errorMsg = "Kategori sudah dimasukkan!" });
                }
                else
                {
                    _context.Kategori.Add(model);
                    _context.SaveChanges();
                    return Json(new { error = false });
                }
            }
            return Json(new { error = true, errorMsg = "Harap isi semua kolom!" });
        }

        public IActionResult GetEditK(int id)
        {
            return Json(_context.Kategori.SingleOrDefault(x => x.KategoriPenyakit == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditK(KategoriViewModels model)
        {
            if (ModelState.IsValid)
            {
                var mod = _context.Kategori.Single(x => x.KategoriPenyakit == model.KategoriPenyakit);

                if (model.NamaKategori != mod.NamaKategori && _context.Kategori.Any(x => x.NamaKategori == model.NamaKategori))
                {
                    return Json(new { error = true, errorMsg = "Kategori sudah dimasukkan!" });
                }
                try
                {
                    mod.NamaKategori = model.NamaKategori;
                    _context.Kategori.Update(mod);
                    await _context.SaveChangesAsync();
                    return Json(new { error = false });
                }
                catch (Exception ex)
                {
                    return Json(new { error = true, errorMsg = ex.Message });
                }
            }
            return Json(new { error = true, errorMsg = "Harap isi semua kolom!" });
        }

        [HttpPost]
        public IActionResult DeleteK(int id)
        {
            var mod = _context.Kategori.SingleOrDefault(m => m.KategoriPenyakit == id);
            _context.Kategori.Remove(mod);
            _context.SaveChanges();
            return Json(new { error = false });
        }
        #endregion

        #region Penyakit
        public async Task<IActionResult> Penyakit()
        {
            return View(new PenyakitViewModels() { Penyakits = await _context.Penyakit.Include(x => x.Kategori).ToListAsync() });
        }

        public async Task<IActionResult> PenyakitTable()
        {
            return View(await _context.Penyakit.Include(x => x.Kategori).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateP(Penyakit model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Penyakit.Any(x => x.NamaPenyakit == model.NamaPenyakit))
                {
                    return Json(new { error = true, errorMsg = "Penyakit sudah ada!" });
                }
                else
                {
                    _context.Penyakit.Add(model);
                    _context.SaveChanges();
                    return Json(new { error = false });
                }
            }
            return Json(new { error = true, errorMsg = "Harap isi semua kolom!" });
        }

        public IActionResult GetEditP(int id)
        {
            return Json(_context.Penyakit.SingleOrDefault(x => x.IdPenyakit == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditP(PenyakitViewModels model)
        {
            if (ModelState.IsValid)
            {
                var mod = _context.Penyakit.Single(x => x.IdPenyakit == model.IdPenyakit);

                if (model.NamaPenyakit != mod.NamaPenyakit && _context.Penyakit.Any(x => x.NamaPenyakit == model.NamaPenyakit))
                {
                    return Json(new { error = true, errorMsg = "Penyakit sudah ada!" });
                }
                try
                {
                    mod.KategoriId = model.KategoriId;
                    mod.NamaPenyakit = model.NamaPenyakit;
                    mod.Solusi = model.Solusi;
                    _context.Penyakit.Update(mod);
                    await _context.SaveChangesAsync();
                    return Json(new { error = false });
                }
                catch (Exception ex)
                {
                    return Json(new { error = true, errorMsg = ex.Message });
                }
            }
            return Json(new { error = true, errorMsg = "Harap isi semua kolom!" });
        }

        [HttpPost]
        public IActionResult DeleteP(int id)
        {
            var mod = _context.Penyakit.SingleOrDefault(m => m.IdPenyakit == id);
            _context.Penyakit.Remove(mod);
            _context.SaveChanges();
            return Json(new { error = false });
        }
        #endregion


        #region Gejala
        public async Task<IActionResult> Gejala()
        {
            return View(new GejalaViewModels() { Gejalas = await _context.Gejala.Include(x => x.Kategori).ToListAsync() });
        }
        
        public async Task<IActionResult> GejalaTable()
        {
            return View(await _context.Gejala.Include(x => x.Kategori).ToListAsync());
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateG(Gejala model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Gejala.Any(x => x.NamaGejala == model.NamaGejala)) //&& GambarGejala != null)
                {
                    return Json(new { error = true, errorMsg = "Gejala sudah dimasukkan!" });
                }
                if (model.FileGambar != null || model.FileGambar.Length > 0)
                {
                    var fileName = Path.GetFileName(model.FileGambar.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", fileName);
                
                   
                    if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                    model.FileGambar.CopyTo(new FileStream(filePath, FileMode.Create));
                
                    model.GambarGejala = fileName;
                    model.Min = MinMax(model.Interval.Value).FirstOrDefault().Key;
                    model.Max = MinMax(model.Interval.Value).FirstOrDefault().Value;
                    _context.Gejala.Add(model);
                    _context.SaveChanges();
                    return Json(new { error = false });
               }
                else
                {
                    return Json(new { error = true, errorMsg = "Gambar belum dipilih!" });
                }
            }
            else
            {
                return Json(new { error = true, errorMsg = "Harap semua kolom diisi!" });
            }
            ///return Json(new { error = false });
        }
        public IActionResult GetEditG(int id)
        {
            return Json(_context.Gejala.SingleOrDefault(x => x.IdGejala == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditG(Gejala model)
        {
            if (ModelState.IsValid)
            {
                var mod = _context.Gejala.Single(x => x.IdGejala == model.IdGejala);

                if (model.NamaGejala != mod.NamaGejala && _context.Gejala.Any(x => x.NamaGejala == model.NamaGejala))
                {
                    return Json(new { error = true, errorMsg = "Gejala sudah dimasukkan!" });
                }
                try
                {
                    mod.KategoriId = model.KategoriId;
                    mod.NamaGejala = model.NamaGejala;
                    mod.Interval = model.Interval;
                    mod.GambarGejala = model.GambarGejala;
                    mod.Min = MinMax(model.Interval.Value).FirstOrDefault().Key;
                    mod.Max = MinMax(model.Interval.Value).FirstOrDefault().Value;
                    _context.Gejala.Update(mod);
                    await _context.SaveChangesAsync();
                    return Json(new { error = false });
                }
                catch (Exception ex)
                {
                    return Json(new { error = true, errorMsg = ex.Message });
                }
            }
            else
            {
                return Json(new { error = true, errorMsg = "Harap isi semua kolom!" });
            }
        }

        public Dictionary<double, double> MinMax(double i)
        {

            var data = new Dictionary<double, double>();
            if (i >= 0 && i <= 0.4)
            {
                data.Add(0, 0.4);
            }
            else if (i >= 0.3 && i <= 0.7)
            {
                data.Add(0.3, 0.7);
            }
            else if (i >= 0.6 && i <= 1)
            {
                data.Add(0.6, 1);
            }
            return data;
        }

        [HttpPost]
        public IActionResult DeleteG(int id)
        {
            if (!_context.Gejala.Any(x => x.ParentId == id))
            {
                var mod = _context.Gejala.SingleOrDefault(m => m.IdGejala == id);
                _context.Gejala.Remove(mod);
                _context.SaveChanges();
                return Json(new { error = false });
            }
            return Json(new { error = true, errorMsg = "Gejala harus dihapus terlebih dahulu sebelum menghapus kategori gejala" });
        }
        #endregion

        #region RekamMedik
        public async Task<IActionResult> RekamMedik()
        {
            var model = await _context.RekamMedik.Include(x => x.IdGejalaNavigation).Include(x => x.IdPenyakitNavigation).Include(x => x.Kategori)
                .Select(x => new RekamMedikString
                {
                    Kode = x.Kode,
                    Penyakit = x.IdPenyakitNavigation.NamaPenyakit,
                    Gejala = x.IdGejalaNavigation.NamaGejala,
                    Kategori = x.Kategori.NamaKategori
                })
                .GroupBy(x => x.Kode)
                .Select(x => new RekamMedikString
                {
                    Kode = x.Key,
                    Penyakit = x.FirstOrDefault().Penyakit,
                    Gejala = String.Join(", ", x.Select(y => y.Gejala).ToArray()),
                    Kategori = x.FirstOrDefault().Kategori
                }).ToListAsync();
            var kode = _context.RekamMedik.GroupBy(x => x.Kode).Count() + 1;
            while (true) { if (_context.RekamMedik.Any(x => x.Kode == kode.ToString())) { kode++; } else { break; } }
            return View(new RekamMedikViewModels() { RekamMediks = model, Kode = kode.ToString() });
        }

        public async Task<IActionResult> RekamMedikTable()
        {
            var model = await _context.RekamMedik.Include(x => x.IdGejalaNavigation).Include(x => x.IdPenyakitNavigation).Include(x => x.Kategori)
                .Select(x => new RekamMedikString
                {
                    Kode = x.Kode,
                    Penyakit = x.IdPenyakitNavigation.NamaPenyakit,
                    Gejala = x.IdGejalaNavigation.NamaGejala,
                    Kategori = x.Kategori.NamaKategori
                })
                .GroupBy(x => x.Kode)
                .Select(x => new RekamMedikString
                {
                    Kode = x.Key,
                    Penyakit = x.FirstOrDefault().Penyakit,
                    Gejala = String.Join(", ", x.Select(y => y.Gejala).ToArray()),
                    Kategori = x.FirstOrDefault().Kategori
                }).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateR(RekamMedikViewModels model)
        {
            if (ModelState.IsValid)
            {
                
                    foreach (var item in model.ListGejala.Split(','))
                    {
                    var kode = _context.RekamMedik.GroupBy(x => x.Kode).Count() + 1;
                    while (true) { if (_context.RekamMedik.Any(x => x.Kode == kode.ToString())) { kode++; } else { break; } }
                        var mod = new RekamMedik()
                        {
                            Kode = kode.ToString(),
                            KategoriId = model.KategoriId,
                            IdPenyakit = model.IdPenyakit,
                            IdGejala = Convert.ToInt32(item)
                        };
                        _context.RekamMedik.Add(mod);
                    }
                    _context.SaveChanges();
                    return Json(new { error = false });
                
            }
            return Json(new { error = true, errorMsg = "Harap diisi semua kolom!" });
        }

        public IActionResult GetEditR(int id)
        {
            return Json(_context.RekamMedik.SingleOrDefault(x => x.IdRekammedik == id));
        }

        [HttpPost]
        public IActionResult DeleteR(string id)
        {
            var mod = _context.RekamMedik.Where(m => m.Kode == id);
            _context.RekamMedik.RemoveRange(mod);
            _context.SaveChanges();
            return Json(new { error = false });
        }
        #endregion

        #region GET
        [AllowAnonymous]
        public IActionResult GetKategori()
        {
            var a = _context.Kategori.Select(x => string.Format("<option value=\"{0}\" >{1}</option>", x.KategoriPenyakit, x.NamaKategori)).ToList();
            return Json(_context.Kategori.Select(x => string.Format("<option value=\"{0}\" >{1}</option>", x.KategoriPenyakit, x.NamaKategori)).ToList());
        }
        [AllowAnonymous]
        public IActionResult GetPenyakit(int id)
        {
            return Json(_context.Penyakit.Where(x => x.KategoriId == id).Select(x => string.Format("<option value=\"{0}\" >{1}</option>", x.IdPenyakit, x.NamaPenyakit)).ToList());
        }
        [AllowAnonymous]
        public IActionResult GetGejala()
        {
            return Json(_context.Gejala.Select(x => string.Format("<li class=\"list-group-item\"><label><input type=\"checkbox\" value=\"{0}\" />{1}</label></li>", x.IdGejala, x.NamaGejala, x.GambarGejala)).ToList());
        }
        [AllowAnonymous]
        public IActionResult GetGejalaSelect(int id, int kat)
        {
            return Json(_context.Gejala.Where(x => x.ParentId == id && x.KategoriId == kat).Select(x => string.Format("<option value=\"{0}\" >{1}</option>", x.IdGejala, x.NamaGejala)).ToList());
        }
        [AllowAnonymous]
        public IActionResult GetGejalaTree(int id, int kat)
        {
            if (id == 0)
                return Json(_context.Gejala.Where(x => x.ParentId == id && x.KategoriId == kat).Select(x => new { id = x.IdGejala, nama = x.NamaGejala, gambar = x.GambarGejala, child = _context.Gejala.Any(y => y.ParentId == x.IdGejala) }).OrderBy(x => x.nama).ToList());
            else
                return Json(_context.Gejala.Where(x => x.ParentId == id && x.KategoriId == kat).Select(x => string.Format("<li class=\"list-group-item\"><label><input type=\"checkbox\" value=\"{0}\" />{1}</label></li>", x.IdGejala, x.NamaGejala)).ToList());
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetGejalaTree1Async(int id, int kat)
        {
            if (id == 0)
                return Json(_context.Gejala.Where(x => x.ParentId == id && x.KategoriId == kat).Select(x => new { id = x.IdGejala, interval = x.Interval.Value, min = x.Min.Value, max = x.Max.Value, gambar = x.GambarGejala, nama = x.NamaGejala, child = _context.Gejala.Any(y => y.ParentId == x.IdGejala) }).OrderBy(x => x.nama).ToList());
            else
                return Ok(await _context.Gejala
                    .Where(x => x.ParentId == id && x.KategoriId == kat)
                    .Select(x => $"<li class=\"list-group-item\"><label><input type=\"checkbox\"  value=\"{x.IdGejala}\" />{x.NamaGejala}</label><div style=\"display:none\"><br /><b>{x.Min.Value}</b> <input type=\"text\" data-id=\"{x.Interval.Value}\" class=\"span2\" value=\"\" data-slider-min=\"{x.Min.Value}\" data-slider-max=\"{x.Max.Value}\" data-slider-step=\"0.01\" data-slider-value=\"{x.Interval.Value}\" /> <b>{x.Max.Value}</b></div></li>")
                    .ToListAsync());
        }
        public string t(double a) {
            switch (a.ToString()){
                case "0":return a.ToString() + "(Ringan)";
                case "0.3": return a.ToString() + "(Agak Parah)";
                case "0.4": return a.ToString() + "(Agak Parah)";
                case "0.6": return a.ToString() + "(Parah)";
                case "0.7": return a.ToString() + "(Parah)";
                case "1": return a.ToString() + "(Sangat Parah)";
                default: return a.ToString();
            }
        }
        #endregion

        public ActionResult Rule()
        {
            recalculate();
            return View(_context.Rule.Include(x => x.IdGejalaNavigation).Include(x => x.IdPenyakitNavigation).Include(x => x.Kategori).ToList());
        }

        private void recalculate()
        {
            var data = _context.RekamMedik.GroupBy(x => x.IdPenyakit);
            _context.Rule.RemoveRange(_context.Rule);
            foreach (var item in data)
            {
                foreach (var items in item.Select(x => x.IdGejala).Distinct())
                {
                    _context.Rule.Add(new Rule { IdPenyakit = item.Key, IdGejala = items, KategoriId = item.FirstOrDefault().KategoriId });
                }
            }

            _context.SaveChanges();
        }

        #region Hash
        private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
        private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits

        /* =======================
         * HASHED PASSWORD FORMATS
         * =======================
         * 
         * Version 0:
         * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
         * (See also: SDL crypto guidelines v5.1, Part III)
         * Format: { 0x00, salt, subkey }
         */

        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            // Produce a version 0 (see comment above) text hash.
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }

            var outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);
            return Convert.ToBase64String(outputBytes);
        }

        // hashedPassword must be of the format of HashWithPassword (salt + Hash(salt+input)
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) text hash.

            if (hashedPasswordBytes.Length != (1 + SaltSize + PBKDF2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
            {
                // Wrong length or version header.
                return false;
            }

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            var storedSubkey = new byte[PBKDF2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2IterCount))
            {
                generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }
            return ByteArraysEqual(storedSubkey, generatedSubkey);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
        #endregion
    }
}
