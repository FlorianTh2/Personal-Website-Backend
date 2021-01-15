// using System;
// using System.Collections.Generic;
// using System.Linq;
// using PersonalWebsiteBackend.Domain;
//
// namespace PersonalWebsiteBackend.Services
// {
//     public class ProjectServiceInMemory : IProjectService
//     {
//         private readonly List<Project> _projects;
//
//         public ProjectServiceInMemory()
//         {
//             _projects = new List<Project>();
//
//             for (var i = 0; i < 5; i++)
//                 _projects.Add(new Project
//                 {
//                     Id = Guid.NewGuid(),
//                     name = "test name " + i
//                 });
//         }
//
//         public List<Project> GetProjects()
//         {
//             return _projects;
//         }
//
//         public Project GetProjectId(Guid projectId)
//         {
//             return _projects.SingleOrDefault(a => a.Id == projectId);
//         }
//
//         public bool UpdateProject(Project projectToUpdate)
//         {
//             var exists = GetProjectId(projectToUpdate.Id) != null;
//             if (!exists)
//                 return false;
//
//             var index = _projects.FindIndex(a => a.Id == projectToUpdate.Id);
//             _projects[index] = projectToUpdate;
//             return true;
//         }
//
//         public bool DeleteProject(Guid projectId)
//         {
//             var project = GetProjectId(projectId);
//
//             if (project == null) return false;
//
//             _projects.Remove(project);
//             return true;
//         }
//     }
// }