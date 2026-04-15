using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/todoitems - Get all tasks (with user and category names)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {
            var todoItems = await _context.TodoItems
                .Include(t => t.User)
                .Include(t => t.Category)
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    UserName = t.User.Name,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();

            return Ok(todoItems);
        }

        // GET: api/todoitems/{id} - Get single task
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems
                .Include(t => t.User)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound($"Todo item with ID {id} not found");
            }

            return Ok(new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                DueDate = todoItem.DueDate,
                UserId = todoItem.UserId,
                CategoryId = todoItem.CategoryId,
                UserName = todoItem.User.Name,
                CategoryName = todoItem.Category.Name
            });
        }

        // GET: api/todoitems/user/{userId} - Get all tasks for a specific user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTasksByUser(int userId)
        {
            var todoItems = await _context.TodoItems
                .Include(t => t.User)
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    UserName = t.User.Name,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();

            return Ok(todoItems);
        }

        // POST: api/todoitems - Create new task
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> CreateTodoItem(CreateTodoItemDto createDto)
        {
            // Check if user exists
            var user = await _context.Users.FindAsync(createDto.UserId);
            if (user == null)
            {
                return BadRequest($"User with ID {createDto.UserId} does not exist");
            }

            // Check if category exists
            var category = await _context.Categories.FindAsync(createDto.CategoryId);
            if (category == null)
            {
                return BadRequest($"Category with ID {createDto.CategoryId} does not exist");
            }

            var todoItem = new TodoItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                IsCompleted = createDto.IsCompleted,
                DueDate = createDto.DueDate,
                UserId = createDto.UserId,
                CategoryId = createDto.CategoryId
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            var todoItemDto = new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                DueDate = todoItem.DueDate,
                UserId = todoItem.UserId,
                CategoryId = todoItem.CategoryId,
                UserName = user.Name,
                CategoryName = category.Name
            };

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItemDto);
        }

        // PUT: api/todoitems/{id} - Update task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, CreateTodoItemDto updateDto)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound($"Todo item with ID {id} not found");
            }

            // Check if user exists
            var user = await _context.Users.FindAsync(updateDto.UserId);
            if (user == null)
            {
                return BadRequest($"User with ID {updateDto.UserId} does not exist");
            }

            // Check if category exists
            var category = await _context.Categories.FindAsync(updateDto.CategoryId);
            if (category == null)
            {
                return BadRequest($"Category with ID {updateDto.CategoryId} does not exist");
            }

            todoItem.Title = updateDto.Title;
            todoItem.Description = updateDto.Description;
            todoItem.IsCompleted = updateDto.IsCompleted;
            todoItem.DueDate = updateDto.DueDate;
            todoItem.UserId = updateDto.UserId;
            todoItem.CategoryId = updateDto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo item updated successfully" });
        }

        // GET: api/todoitems?completed=true&userId=1&categoryId=1&search=project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems(
            [FromQuery] bool? completed = null,
            [FromQuery] int? userId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string search = null,
            [FromQuery] string sortBy = "dueDate",
            [FromQuery] bool sortDescending = false)
        {
            var query = _context.TodoItems
                .Include(t => t.User)
                .Include(t => t.Category)
                .AsQueryable();

            // Filter by completion status
            if (completed.HasValue)
            {
                query = query.Where(t => t.IsCompleted == completed.Value);
            }

            // Filter by user
            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            // Search by title or description
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    t.Description.Contains(search));
            }

            // Sorting
            query = sortBy.ToLower() switch
            {
                "title" => sortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "dueDate" => sortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "isCompleted" => sortDescending ? query.OrderByDescending(t => t.IsCompleted) : query.OrderBy(t => t.IsCompleted),
                _ => sortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate)
            };

            var todoItems = await query
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    UserName = t.User.Name,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();

            return Ok(todoItems);
        }

        // GET: api/todoitems/statistics - Get task statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var totalTasks = await _context.TodoItems.CountAsync();
            var completedTasks = await _context.TodoItems.CountAsync(t => t.IsCompleted);
            var pendingTasks = totalTasks - completedTasks;
            var overdueTasks = await _context.TodoItems.CountAsync(t => t.DueDate < DateTime.Today && !t.IsCompleted);

            var tasksByUser = await _context.TodoItems
                .GroupBy(t => t.User.Name)
                .Select(g => new { User = g.Key, Count = g.Count() })
                .ToListAsync();

            var tasksByCategory = await _context.TodoItems
                .GroupBy(t => t.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                OverdueTasks = overdueTasks,
                CompletionRate = totalTasks > 0 ? (completedTasks * 100 / totalTasks) : 0,
                TasksByUser = tasksByUser,
                TasksByCategory = tasksByCategory
            });
        }

        // DELETE: api/todoitems/{id} - Delete task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound($"Todo item with ID {id} not found");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo item deleted successfully" });
        }
    }
}