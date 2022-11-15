
using System.Collections.Generic;

namespace ManageEmployee.Models
{
  public class AuthRoleModel
    {
        public string Name { get; set; }
        public IList<string> Roles { get; set; }
    }
}