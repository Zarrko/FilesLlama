using ErrorOr;

namespace FilesLlama.Application.Common.Interfaces;

public interface IEmbeddingsGenerator
{
    Task<IEnumerable<ErrorOr<double[]>>> EmbedContentObjects(List<string> contentObjects);
}