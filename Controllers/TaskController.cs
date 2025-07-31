using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using AIS_RubricFeedbackGenerator.Data;
using AIS_RubricFeedbackGenerator.Models;
using AIS_RubricFeedbackGenerator.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace AIS_RubricFeedbackGenerator.Controllers
{
    public class TaskController : Controller
    {
        private readonly AIS_RubricFeedbackGeneratorContext context;
        private readonly UserManager<AIS_RubricFeedbackGeneratorUser> _userManager;

        public TaskController(
            AIS_RubricFeedbackGeneratorContext context,
            UserManager<AIS_RubricFeedbackGeneratorUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        // GET: Task
        //[Authorize]
        public ActionResult Index()
        {
            var tasks = context.Tasks.OrderByDescending(r => r.TaskId).ToList();
            var userIds = tasks.Select(t => t.CreatedBy).Distinct().ToList();
            var users = context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName);

            // Updated code to handle potential null reference for 'task.CreatedBy'
            foreach (var task in tasks)
            {
                if (!string.IsNullOrEmpty(task.CreatedBy) && users.TryGetValue(task.CreatedBy, out var name))
                {
                    task.CreatedBy = name;
                }
                else
                {
                    task.CreatedBy = "Unknown";
                }
            }
            return View(tasks);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            var viewModel = new TaskFormViewModel();

            ViewBag.Breadcrumb = new[] {
                new { Title = "Home", Url = Url.Action("Index", "Home") },
                new { Title = "Tasks", Url = Url.Action("Index", "Task") },
                new { Title = "Create Task", Url = "" }
            };
            return View(viewModel);
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public async Task<IActionResult> Create(TaskFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                string yearPrefix = DateTime.Now.ToString("yy");
                var lastTask = await context.Tasks
                    .Where(t => t.TaskId.StartsWith("T" + yearPrefix))
                    .OrderByDescending(t => t.TaskId)
                    .FirstOrDefaultAsync();

                int lastTaskNum = lastTask != null
                    ? int.Parse(lastTask.TaskId.Substring(3))
                    : 0;

                string newTaskId = $"T{yearPrefix}{(lastTaskNum + 1):D4}";

                var newTask = new Models.Task
                {
                    TaskId = newTaskId,
                    Title = model.Title,
                    Description = model.Description,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userManager.GetUserId(User)
                };

                context.Tasks.Add(newTask);
                await context.SaveChangesAsync();

                TempData["TaskMessage"] = "Task created successfully!";
                return RedirectToAction(nameof(Edit), new { id = newTaskId });
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving the task.");
                return View(model);
            }
        }

        // GET: Task/Edit
        [HttpGet("Task/Edit/{id}")]
        public IActionResult Edit(string id)
        {
            var task = context.Tasks.Find(id);
            var rubrics = context.Rubrics
                .Where(r => r.TaskId == id)
                .OrderByDescending(r => r.RubricId)
                .ToList();
            if (task == null)
            {
                return NotFound();
            }
            var viewModel = new TaskFormViewModel
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                Rubrics = rubrics
            };

            ViewBag.Breadcrumb = new[] {
                new { Title = "Home", Url = Url.Action("Index", "Home") },
                new { Title = "Tasks", Url = Url.Action("Index", "Task") },
                new { Title = task.Title, Url = "" }
            };
            return View(viewModel);
        }

        // POST: Task/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public async Task<IActionResult> Edit(TaskFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload rubrics in case of error
                model.Rubrics = context.Rubrics
                    .Where(r => r.TaskId == model.TaskId)
                    .OrderByDescending(r => r.RubricId)
                    .ToList();

                return View(model);
            }

            var task = await context.Tasks.FindAsync(model.TaskId);
            if (task == null)
            {
                return NotFound();
            }
            task.Title = model.Title;
            task.Description = model.Description;

            context.Update(task);
            await context.SaveChangesAsync();

            TempData["TaskMessage"] = "Task updated successfully!";
            return RedirectToAction(nameof(Edit), new { id = model.TaskId });
        }

        // POST: Task/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var task = await context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}