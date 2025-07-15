using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.ViewModels
{
    public class RoleVM
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
