using TTSS.Infra.Data.MongoDB.Models;

namespace TTSS.Infra.Data.MongoDB.Documents;

internal class Person : MongoDbDocumentBase
{
    public string Name { get; set; }
}