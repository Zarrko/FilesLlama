namespace FilesLlama.Contractsv2.Query;

public record QueryRequest(string UserQuery, int K = 2);