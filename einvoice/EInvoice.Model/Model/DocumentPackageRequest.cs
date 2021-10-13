using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Model
{
    public class DocumentPackageRequest
    {
        public string type { get; set; } = "Full";
        public string format { get; set; } = "JSON";
        public QueryParameters queryParameters { get; set; }
    }
}
