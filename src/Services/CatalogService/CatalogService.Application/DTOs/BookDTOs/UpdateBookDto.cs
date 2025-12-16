using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.DTOs.BookDTOs
{
    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
    }
}
