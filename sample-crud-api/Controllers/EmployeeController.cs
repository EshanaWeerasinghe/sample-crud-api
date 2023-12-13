using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sample_crud_api.Models;

namespace sample_crud_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _employeeContext;

        public EmployeeController(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        // HTTP GET method to retrieve a collection of all employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            // Asynchronously retrieve all employees from the database
            var employees = await _employeeContext.Employees.ToListAsync();

            // Check if the collection is empty
            if (employees == null || employees.Count == 0)
            {
                // Return 204 No Content status if the collection is empty
                return NoContent();
            }

            // Return the non-empty collection of employees
            return employees;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var employee = await _employeeContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound(); // Return 404 Not Found status if the employee with the specified ID is not found
            }

            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _employeeContext.Employees.Add(employee);
            await _employeeContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployees), new { id = employee.ID }, employee);
        }

        // HTTP PUT method to update an employee by ID
        [HttpPut("{id}")]
        public async Task<ActionResult> PutEmployee(int id, Employee employee)
        {
            // Check if the provided ID matches the employee's ID
            if (id != employee.ID)
            {
                return BadRequest("The provided ID does not match the employee's ID.");
            }

            // Set the state of the employee entity to Modified for database update
            _employeeContext.Entry(employee).State = EntityState.Modified;

            try
            {
                // Save changes to the database asynchronously
                await _employeeContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues - check if the employee with the given ID exists
                if (!_employeeContext.Employees.Any(e => e.ID == id))
                {
                    return NotFound("Employee not found.");
                }
                else
                {
                    // Propagate other database update concurrency exceptions
                    throw;
                }
            }

            // Return 204 No Content status on successful update
            return NoContent();
        }

        // HTTP DELETE method to delete an employee by ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            // Check if there are any employees in the database
            if (!_employeeContext.Employees.Any())
            {
                return NotFound();
            }

            // Find the employee by ID asynchronously
            var employee = await _employeeContext.Employees.FindAsync(id);

            // Check if the employee with the given ID exists
            if (employee == null)
            {
                return NotFound();
            }

            // Remove the employee from the context
            _employeeContext.Employees.Remove(employee);

            // Save changes to the database asynchronously
            await _employeeContext.SaveChangesAsync();

            // Return a 200 OK status indicating successful deletion
            return Ok();
        }
    }
}