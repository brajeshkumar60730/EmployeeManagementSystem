using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;

public class EmployeeController : Controller
{
    private readonly IEmployeeRepository _repo;
    private readonly IWebHostEnvironment _env;

    public EmployeeController(IEmployeeRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    public IActionResult Index(string search, int page = 1)
    {
        int pageSize = 5;
        int totalCount;

        var data = _repo.GetPaged(search, page, pageSize, out totalCount);
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.Search = search;

        return View(data);
    }

    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public IActionResult Create(Employee emp, IFormFile ImageFile)
    {
        // Set default before validation
        emp.ProfileImage = "default.png";

        // Handle file upload
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            string folderPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            using var fs = new FileStream(filePath, FileMode.Create);
            ImageFile.CopyTo(fs);

            emp.ProfileImage = fileName;
        }

        // If model is invalid, show form again
        if (!ModelState.IsValid)
        {
            PopulateDropdowns();
            return View(emp);
        }

        emp.EmployeeId = _repo.Add(emp);

        TempData["success"] = $"Employee created successfully. ID: {emp.EmployeeId}";
        return RedirectToAction("Index");
    }


    public IActionResult Edit(int id)
    {
        var emp = _repo.GetById(id);
        if (emp == null)
            return NotFound();

        PopulateDropdowns();
        return View(emp);
    }

    [HttpPost]
    public IActionResult Edit(Employee emp, IFormFile ImageFile)
    {
        // Load existing employee from DB to preserve image if none uploaded
        var existingEmp = _repo.GetById(emp.EmployeeId);
        if (existingEmp == null)
            return NotFound();

        // If a new file was uploaded
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            string folderPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            using var fs = new FileStream(filePath, FileMode.Create);
            ImageFile.CopyTo(fs);

            // Update ProfileImage field if new file uploaded
            emp.ProfileImage = fileName;
        }
        else
        {
            // Keep old image if no new file was uploaded
            emp.ProfileImage = existingEmp.ProfileImage;
        }

        // Validate updated model
        if (!ModelState.IsValid)
        {
            PopulateDropdowns();
            return View(emp);
        }

        // Save to database
        _repo.Update(emp);

        TempData["success"] = "Employee updated successfully";
        return RedirectToAction("Index");
    }

    public IActionResult DeleteConfirm(int id)
    {
        return View(_repo.GetById(id));
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int EmployeeId)
    {
        _repo.Delete(EmployeeId);
        TempData["success"] = "Employee deleted successfully";
        return RedirectToAction("Index");
    }

    private void PopulateDropdowns()
    {
        ViewBag.Genders = new List<string> { "Male", "Female", "Other" };
        ViewBag.Countries = new List<string> { "USA", "UK", "India" };
    }
}
