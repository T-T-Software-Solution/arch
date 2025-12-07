using TTSS.Core.Models;
using TTSS.Infra.Data.MongoDB.Models;

namespace TTSS.Infra.Data.MongoDB.Documents;

internal class OrderablePerson : MongoDbDocumentBase, IHaveOrderNumber
{
    public string Name { get; set; }
    public int OrderNo { get; set; }
}
