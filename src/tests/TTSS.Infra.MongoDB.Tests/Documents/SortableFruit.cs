using TTSS.Infra.Data.MongoDB.Models;

namespace TTSS.Infra.Data.MongoDB.Documents;

internal class SortableFruit : MongoDbDocumentBase
{
    public string Name { get; set; }
    public int Price { get; set; }
}
