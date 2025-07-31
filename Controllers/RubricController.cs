using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using AIS_RubricFeedbackGenerator.Data;
using AIS_RubricFeedbackGenerator.Models;
using AIS_RubricFeedbackGenerator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AIS_RubricFeedbackGenerator.Controllers
{
    public class RubricController : Controller
    {
        private readonly AIS_RubricFeedbackGeneratorContext _context;
        private readonly UserManager<AIS_RubricFeedbackGeneratorUser> _userManager;

        public RubricController(
            AIS_RubricFeedbackGeneratorContext context,
            UserManager<AIS_RubricFeedbackGeneratorUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Rubric/Index?taskId=T250001
        public IActionResult Index(string taskId)
        {
            var rubrics = _context.Rubrics
                .Where(r => r.TaskId == taskId)
                .ToList();

            ViewBag.TaskId = taskId;
            return View(rubrics);
        }

        // GET: Rubric/Create/T250001
        [HttpGet("Rubric/Create/{taskId}")]
        public IActionResult Create(string taskId)
        {
            var model = new RubricFormViewModel
            {
                TaskId = taskId
            };

            ViewData["Title"] = "Create Marking Rubric";
            SetBreadcrumb(taskId, isEdit: false);

            return View(model);
        }

        // POST: Rubric/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RubricFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetBreadcrumb(model.TaskId ?? "", isEdit: false);
                return View(model);
            }

            try
            {
                string yearPrefix = DateTime.Now.ToString("yy");

                // Generate Rubric ID
                var lastRubric = await _context.Rubrics
                    .Where(r => r.RubricId.StartsWith($"R{yearPrefix}"))
                    .OrderByDescending(r => r.RubricId)
                    .FirstOrDefaultAsync();

                int lastRubricNum = lastRubric != null ? int.Parse(lastRubric.RubricId.Substring(3)) : 0;
                string newRubricId = $"R{yearPrefix}{(lastRubricNum + 1):D4}";

                double maxScore = model.ScoreDefinitions?.Any() == true ? model.ScoreDefinitions.Max(sd => sd.ScoreValue)
    : 0;
                double totalMark = (model.Criteria?.Count ?? 0) * maxScore;


                var rubric = new Rubric
                {
                    RubricId = newRubricId,
                    TaskId = model.TaskId!,
                    Title = model.RubricTitle ?? "Untitled Rubric",
                    Description = model.Description,
                    TotalMark = totalMark,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userManager.GetUserId(User)
                };

                _context.Rubrics.Add(rubric);

                //Update Task TotalMark
                var task = await _context.Tasks.FindAsync(model.TaskId);
                if (task == null)
                {
                    return NotFound();
                }
                task.TotalMark += totalMark;
                _context.Tasks.Update(task);

                // Create Score Definitions
                var scoreDefinitions = new List<ScoreDefinition>();
                var lastSd = await _context.ScoreDefinitions
                    .Where(s => s.ScoreDefinitionId.StartsWith($"SD{yearPrefix}"))
                    .OrderByDescending(s => s.ScoreDefinitionId)
                    .FirstOrDefaultAsync();
                int lastSdNum = lastSd != null ? int.Parse(lastSd.ScoreDefinitionId.Substring(4)) + 1 : 1;

                foreach (var def in model.ScoreDefinitions)
                {
                    var sd = new ScoreDefinition
                    {
                        ScoreDefinitionId = $"SD{yearPrefix}{lastSdNum:D4}",
                        RubricId = newRubricId,
                        ScoreName = def.ScoreName,
                        ScoreValue = def.ScoreValue,
                        CreatedAt = DateTime.Now
                    };

                    _context.ScoreDefinitions.Add(sd);
                    scoreDefinitions.Add(sd);
                    lastSdNum++;
                }

                // Create Criteria and Score Levels
                var lastCriterion = await _context.Criteria
                    .Where(c => c.CriterionId.StartsWith($"C{yearPrefix}"))
                    .OrderByDescending(c => c.CriterionId)
                    .FirstOrDefaultAsync();
                int lastCriterionNum = lastCriterion != null ? int.Parse(lastCriterion.CriterionId.Substring(3)) + 1 : 1;

                var lastSl = await _context.ScoreLevels
                    .Where(sl => sl.ScoreLevelId.StartsWith($"SL{yearPrefix}"))
                    .OrderByDescending(sl => sl.ScoreLevelId)
                    .FirstOrDefaultAsync();
                int lastSlNum = lastSl != null ? int.Parse(lastSl.ScoreLevelId.Substring(4)) + 1 : 1;

                for (int i = 0; i < model.Criteria.Count; i++)
                {
                    var criterionId = $"C{yearPrefix}{lastCriterionNum:D4}";
                    var criterion = new Criterion
                    {
                        CriterionId = criterionId,
                        RubricId = newRubricId,
                        Title = model.Criteria[i].Title,
                        SortOrder = i + 1,
                        CreatedAt = DateTime.Now
                    };

                    _context.Criteria.Add(criterion);
                    lastCriterionNum++;

                    for (int j = 0; j < scoreDefinitions.Count; j++)
                    {
                        var scoreLevel = new ScoreLevel
                        {
                            ScoreLevelId = $"SL{yearPrefix}{lastSlNum:D4}",
                            CriterionId = criterionId,
                            ScoreDefinitionId = scoreDefinitions[j].ScoreDefinitionId,
                            Description = model.ScoreLevelDescriptions[i][j],
                            CreatedAt = DateTime.Now
                        };

                        _context.ScoreLevels.Add(scoreLevel);
                        lastSlNum++;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Task", new { id = model.TaskId });
            }
            //catch (DbUpdateException dbEx)
            //{
            //    var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            //    ModelState.AddModelError("", "Database Error: " + innerMessage);
            //    return View(model);
            //}
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the rubric.");
                ModelState.AddModelError("", "General error: " + ex.Message);
                SetBreadcrumb(model.TaskId ?? "", isEdit: false);
                return View(model);
            }
        }

        // GET: Rubric/Edit
        [HttpGet("Rubric/Edit/{id}")]
        public IActionResult Edit(string id)
        {
            var rubric = _context.Rubrics.Find(id);
            if (rubric == null)
                return NotFound();

            var criteria = _context.Criteria
                .Where(c => c.RubricId == id)
                .ToList();

            var scoreDefinitions = _context.ScoreDefinitions
                .Where(sd => sd.RubricId == id)
                .OrderByDescending(sd => sd.ScoreValue)
                .ToList();

            var scoreLevelDescriptions = criteria.Select(criterion =>
                scoreDefinitions.Select(scoreDef =>
                    _context.ScoreLevels
                        .Where(sl => sl.CriterionId == criterion.CriterionId && sl.ScoreDefinitionId == scoreDef.ScoreDefinitionId)
                        .Select(sl => sl.Description)
                        .FirstOrDefault() ?? "" // fallback to empty string if not found
                ).ToList()
            ).ToList();

            var viewModel = new RubricFormViewModel
            {
                TaskId = rubric.TaskId,
                TaskTitle = _context.Tasks.FirstOrDefault(t => t.TaskId == rubric.TaskId)?.Title,
                RubricTitle = rubric.Title,
                Description = rubric.Description,
                Criteria = criteria.Select(c => new CriterionInputModel
                {
                    Title = c.Title
                }).ToList(),
                ScoreDefinitions = scoreDefinitions.Select(sd => new ScoreDefinitionInputModel
                {
                    ScoreName = sd.ScoreName,
                    ScoreValue = sd.ScoreValue
                }).ToList(),
                ScoreLevelDescriptions = scoreLevelDescriptions
            };
            SetBreadcrumb(rubric.TaskId, isEdit: true);
            return View(viewModel);
        }

        // POST: Rubric/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public async Task<IActionResult> Edit(string id, RubricFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetBreadcrumb(model.TaskId ?? "", isEdit: true);
                return View(model);
            }

            var rubric = await _context.Rubrics.FindAsync(id);
            if (rubric == null)
                return NotFound();

            try
            {
                string yearPrefix = DateTime.Now.ToString("yy");

                // Remove old related data
                var oldCriteria = _context.Criteria.Where(c => c.RubricId == id).ToList();
                var oldScoreDefinitions = _context.ScoreDefinitions.Where(sd => sd.RubricId == id).ToList();
                var oldScoreLevels = _context.ScoreLevels.Where(sl => oldCriteria.Select(c => c.CriterionId).Contains(sl.CriterionId)).ToList();

                _context.ScoreLevels.RemoveRange(oldScoreLevels);
                _context.ScoreDefinitions.RemoveRange(oldScoreDefinitions);
                _context.Criteria.RemoveRange(oldCriteria);

                // Update Rubric
                rubric.Title = model.RubricTitle ?? "Untitled Rubric";
                rubric.Description = model.Description;

                // Generate Score Definitions
                var scoreDefinitions = new List<ScoreDefinition>();
                var lastSd = await _context.ScoreDefinitions
                    .Where(s => s.ScoreDefinitionId.StartsWith($"SD{yearPrefix}"))
                    .OrderByDescending(s => s.ScoreDefinitionId)
                    .FirstOrDefaultAsync();
                int lastSdNum = lastSd != null ? int.Parse(lastSd.ScoreDefinitionId.Substring(4)) + 1 : 1;

                foreach (var def in model.ScoreDefinitions)
                {
                    var sd = new ScoreDefinition
                    {
                        ScoreDefinitionId = $"SD{yearPrefix}{lastSdNum:D4}",
                        RubricId = id,
                        ScoreName = def.ScoreName,
                        ScoreValue = def.ScoreValue,
                        CreatedAt = DateTime.Now
                    };

                    _context.ScoreDefinitions.Add(sd);
                    scoreDefinitions.Add(sd);
                    lastSdNum++;
                }

                // Generate Criteria and ScoreLevels
                var lastCriterion = await _context.Criteria
                    .Where(c => c.CriterionId.StartsWith($"C{yearPrefix}"))
                    .OrderByDescending(c => c.CriterionId)
                    .FirstOrDefaultAsync();
                int lastCriterionNum = lastCriterion != null ? int.Parse(lastCriterion.CriterionId.Substring(3)) + 1 : 1;

                var lastSl = await _context.ScoreLevels
                    .Where(sl => sl.ScoreLevelId.StartsWith($"SL{yearPrefix}"))
                    .OrderByDescending(sl => sl.ScoreLevelId)
                    .FirstOrDefaultAsync();
                int lastSlNum = lastSl != null ? int.Parse(lastSl.ScoreLevelId.Substring(4)) + 1 : 1;

                double maxScore = model.ScoreDefinitions?.Any() == true ? model.ScoreDefinitions.Max(sd => sd.ScoreValue) : 0;
                double totalMark = (model.Criteria?.Count ?? 0) * maxScore;

                for (int i = 0; i < model.Criteria.Count; i++)
                {
                    var criterionId = $"C{yearPrefix}{lastCriterionNum:D4}";
                    var criterion = new Criterion
                    {
                        CriterionId = criterionId,
                        RubricId = id,
                        Title = model.Criteria[i].Title,
                        SortOrder = i + 1,
                        CreatedAt = DateTime.Now
                    };

                    _context.Criteria.Add(criterion);
                    lastCriterionNum++;

                    for (int j = 0; j < scoreDefinitions.Count; j++)
                    {
                        var scoreLevel = new ScoreLevel
                        {
                            ScoreLevelId = $"SL{yearPrefix}{lastSlNum:D4}",
                            CriterionId = criterionId,
                            ScoreDefinitionId = scoreDefinitions[j].ScoreDefinitionId,
                            Description = model.ScoreLevelDescriptions[i][j],
                            CreatedAt = DateTime.Now
                        };

                        _context.ScoreLevels.Add(scoreLevel);
                        lastSlNum++;
                    }
                }

                rubric.TotalMark = totalMark;

                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Task", new { id = rubric.TaskId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the rubric.");
                ModelState.AddModelError("", "General error: " + ex.Message);
                SetBreadcrumb(model.TaskId ?? "", isEdit: true);
                return View(model);
            }
        }

        // POST: Rubric/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var rubric = await _context.Rubrics
                .FindAsync(id);
            
            if (rubric == null)
                return NotFound();

            var criteria = _context.Criteria
                .Where(c => c.RubricId == id)
                .ToList();
            var scoreDefinitions = _context.ScoreDefinitions
                .Where(sd => sd.RubricId == id)
                .ToList();
            var scoreLevels = _context.ScoreLevels
                .Include(sl => sl.Criterion)
                .Where(sl => sl.Criterion.RubricId == id)
                .ToList();

            _context.ScoreLevels.RemoveRange(scoreLevels);
            _context.ScoreDefinitions.RemoveRange(scoreDefinitions);
            _context.Criteria.RemoveRange(criteria);
            _context.Rubrics.Remove(rubric);

            await _context.SaveChangesAsync();
            TempData["TaskMessage"] = "Rubric deleted successfully!";
            return RedirectToAction("Edit", "Task", new { id = rubric.TaskId });
        }

        // Reusable breadcrumb builder
        private void SetBreadcrumb(string taskId, bool isEdit)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
            var taskTitle = task != null ? task.Title : $"Task {taskId}";

            ViewBag.Breadcrumb = new[]
            {
                new { Title = "Home", Url = Url.Action("Index", "Home") },
                new { Title = "Tasks", Url = Url.Action("Index", "Task") },
                new { Title = taskTitle, Url = Url.Action("Edit", "Task", new { id = taskId }) },
                new { Title = isEdit ? "Edit Rubric" : "Create Rubric", Url = "" }
            };
        }
    }
}