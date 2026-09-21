# CV Chatbot

A CV Chatbot to answer questions on Fraser Nicholson's CV.

The Deployed API URL is:

https://cv-chatbot-api.livelypebble-4c0a2abd.uksouth.azurecontainerapps.io

The Web UI URL is:

https://salmon-flower-0c0a18f0f.4.azurestaticapps.net

## Updating CV information

Details on updating CV information can be found in the [Chunk Embeddings Console App README](./backend/CVChatbotApi/ChunkEmbeddingsConsoleApp/README.md)

## Infrastructure

Terraform state is stored in a manually provisioned storage container, and container images are pushed to a public docker registry, with all other infrastructure provisioned and managed using terraform. See the `./infra` folder for more detail.

### Updating infrastructure

For any infrastructure changes, due to their being no CI checks on them, please run `terraform plan` BEFORE merging changes.

Once the changes are in main, then run `terraform apply`. You will also need to provide the Gemini API key when running either of these terraform commands, which can be done by setting the TF_VAR_gemini_api_key env variable. e.g. in powershell:

```sh
$env:TF_VAR_gemini_api_key = 'api-key'
```

## Manually building docker image

To manually push a docker image, run the following commands whilst in the `./backend/CVChatbotApi` folder:

```sh
docker build -f CVChatbotApi/Dockerfile -t frasernicholson/cv-chatbot-api:latest .

docker push frasernicholson/cv-chatbot-api:latest
```

To build a local image, and test it, add an env file with an `Gemini__ApiKey` entry, and run the following commands (in the same directory as above):

```sh
docker build -f CVChatbotApi/Dockerfile -t cv-chatbot-api:local .

docker run -p 8080:8080 --env-file .env cv-chatbot-api:local
```
