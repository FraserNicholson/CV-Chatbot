# Chunk Embeddings Console App

Console app to retrieve and store chunk embeddings as a json file, to be used in the API as an in memory store.

Intended to be run locally when there are any changes to the CVChunks.

## Running locally

In order to run locally, you must have a Google Gemini AI API key. This, along with the output filepath (likely the location of `chunkEmbeddings.json` in the API project) need to be added to .NET user secrets as follows:

```json
{
  "Gemini:ApiKey": "your-api-key",
  "Output:FilePath": "filepath-to-output-json"
}
```

To ensure the console app doesn't run accidentally, an `Enabled` flag must be set to true in `appsettings.json` for it to execute when run.