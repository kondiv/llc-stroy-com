using LLCStroyCom.Domain.Exceptions;

namespace LLCStroyCom.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Project> Projects { get; private set; } = [];
    
    public virtual ICollection<ApplicationUser> Employees { get; private set; } = [];

    public void AddEmployee(ApplicationUser employee)
    {
        if (Employees.Any(e => e.Id == employee.Id))
        {
            throw AlreadyWorks.InCompany(Id);
        }
        
        Employees.Add(employee);
    }

    public void RemoveEmployee(Guid employeeId)
    {
        var employeeToRemove = Employees.FirstOrDefault(e => e.Id == employeeId)
            ?? throw CouldNotFindUser.WithId(employeeId);
        
        Employees.Remove(employeeToRemove);
    }

    public void AddProject(Project project)
    {
        if (Projects.Any(p => p.Id == project.Id))
        {
            throw new AlreadyExists("Project already exists");
        }
        
        Projects.Add(project);
    }

    public void RemoveProject(Guid projectId)
    {
        var projectToRemove = Projects.FirstOrDefault(p => p.Id == projectId)
            ?? throw CouldNotFindProject.WithId(projectId);
        
        Projects.Remove(projectToRemove);
    }
}