using System.ComponentModel.DataAnnotations;

namespace API.Resources
{
  public class CategoryResource
  {
    public int Id { get; set; }

    [MinLength(5)]
    public string Name { get; set; }
    [MinLength(8)]
    public string Describition { get; set; }
  }
}
